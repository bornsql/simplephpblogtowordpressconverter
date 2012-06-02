using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ITS.Tools.SimplePHPBlogToWordPressConverter.Entities;
using System.Web;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter
{
	public enum Parser
	{
		BlogEntry,
		Comment,
		Static
	}

	public class Converter
	{
		private const string DefaultCategory = "1";

		private StringBuilder m_fileContent;
		private List<PhpStaticEntity> m_staticEntries;
		private List<PhpBlogEntity> m_blogEntries;
		private List<PhpCommentEntity> m_commentEntries;
		private List<WpPostEntity> m_postEntities;
		private List<string> m_statics;
		private List<string> m_blogs;
		private List<string> m_comments;
		private List<PhpCategoryEntity> m_categories;

		private string WpPostTable { get; set; }
		private string WpCommentTable { get; set; }
		private string WpMetaTable { get; set; }
		private string WpTermTable { get; set; }
		private string WpTermRelationshipTable { get; set; }
		private string WpTermTaxonomyTable { get; set; }

		public string HttpRoot { get; set; }
		public string CategoryList { get; set; }

		/// <summary>
		/// Converts the blog to SQL.
		/// </summary>
		/// <param name="sourcePath">The source path.</param>
		/// <param name="targetPath">The target path.</param>
		public void ConvertBlogToSql(string sourcePath, string targetPath)
		{
			// Create the categories
			m_categories = ParseCategories(CategoryList);

			// Go through the folder structure, looking for txt and GZ files
			// There are three levels under content:
			// Year (usually YY)
			// Month (usually MM)

			var fileSystemHelper = new FileSystemHelper();

			WpPostTable = "wp_posts";
			WpCommentTable = "wp_comments";
			WpMetaTable = "wp_postmeta";
			WpTermTable = "wp_terms";
			WpTermRelationshipTable = "wp_term_relationships";
			WpTermTaxonomyTable = "wp_term_taxonomy";

			var di = new DirectoryInfo(sourcePath); // the content folder

			foreach (var yearFolder in di.GetDirectories())
			{
				if (yearFolder.ToString().Trim().ToLower() == "counter" || yearFolder.ToString().Trim().ToLower() == "static")
				{
					// Static Pages
					if (yearFolder.ToString().Trim().ToLower() == "static")
					{
						foreach (var file in yearFolder.GetFiles())
						{
							PrepareFiles(fileSystemHelper, file, Parser.Static);
						}
					}
				}
				else
				{
					// Blog entries
					foreach (var monthFolder in yearFolder.GetDirectories())
					{
						foreach (var file in monthFolder.GetFiles())
						{
							PrepareFiles(fileSystemHelper, file, Parser.BlogEntry);

							var commentFolderName = string.Format("{0}\\{1}\\comments", file.DirectoryName,
																  file.Name.Replace(".gz", string.Empty).Replace(".txt",
																												 string.
																													 Empty));

							if (Directory.Exists(commentFolderName) && Directory.GetFiles(commentFolderName).Length > 0)
							{
								var directoryInfo = new DirectoryInfo(commentFolderName);

								foreach (var fileInfo in directoryInfo.GetFiles())
								{
									m_comments = new List<string>();
									m_commentEntries = new List<PhpCommentEntity>();
									PrepareFiles(fileSystemHelper, fileInfo, Parser.Comment);
									// Add comment to appropriate blog entry

									foreach (var b in m_blogEntries)
									{
										if (b.FileName ==
											file.Name.Replace(".gz", string.Empty).Replace(".txt", string.Empty))
										{
											foreach (var c in m_commentEntries)
											{
												if (b.Comments == null)
												{
													b.Comments = new List<PhpCommentEntity>();
												}
												var phpCommentEntity =
													b.Comments.Find(
														item =>
														item.Date == c.Date && item.Content == c.Content &&
														item.Name == c.Name);
												if (phpCommentEntity == null)
												{
													b.Comments.Add(c);
												}
											}
											break;
										}
									}
								}
							}
						}
					}
				}
			}

			// Empty lists for GC
			if (m_statics != null)
			{
				m_statics.Clear();
			}
			if (m_blogs != null)
			{
				m_blogs.Clear();
			}
			if (m_comments != null)
			{
				m_comments.Clear();
			}

			// Now go through the list of BlogEntities and convert them
			m_postEntities = new List<WpPostEntity>();

			if (m_blogEntries == null)
			{
				m_blogEntries = new List<PhpBlogEntity>();
			}
			foreach (var b in m_blogEntries)
			{
				m_postEntities.Add(ConvertBlogEntity(b));
			}

			if (m_staticEntries == null)
			{
				m_staticEntries = new List<PhpStaticEntity>();
			}
			foreach (var s in m_staticEntries)
			{
				m_postEntities.Add(ConvertStaticEntity(s));
			}

			// Write to the SQL StringBuilder
			var sb = GenerateSql(m_postEntities);

			// Write to the SQL file
			fileSystemHelper.FileWrite(sb, targetPath);
		}

		/// <summary>
		/// Prepares the files.
		/// </summary>
		/// <param name="fileSystemHelper">The file system helper.</param>
		/// <param name="file">The file.</param>
		/// <param name="parser">The parser.</param>
		private void PrepareFiles(FileSystemHelper fileSystemHelper, FileInfo file, Parser parser)
		{
			List<string> files = new List<string>();

			switch (parser)
			{
				case Parser.BlogEntry:
					files = m_blogs;
					break;
				case Parser.Comment:
					files = m_comments;
					break;
				case Parser.Static:
					files = m_statics;
					break;
			}

			if (files == null)
			{
				files = new List<string>();
			}

			if (!files.Contains(fileSystemHelper.GetFileNameFromFileInfo(file)))
			{
				FileInfo unzippedFile;
				if (!file.Extension.Contains(".gz"))
					unzippedFile = file;
				else
				{
					var fileName = fileSystemHelper.GetFileNameFromFileInfo(file).Replace(".gz", string.Empty);
					unzippedFile = !File.Exists(fileName) ? fileSystemHelper.UnzipFile(file) : file;
				}

				bool result;

				switch (parser)
				{
					case Parser.Comment:
						result = ParseComment(unzippedFile);
						break;
					case Parser.Static:
						result = ParseStaticPage(unzippedFile);
						break;
					//case Parser.File:
					default:
						result = ParseBlogEntry(unzippedFile);
						break;
				}

				if (result)
				{
					if (!files.Contains(fileSystemHelper.GetFileNameFromFileInfo(unzippedFile)))
					{
						files.Add(fileSystemHelper.GetFileNameFromFileInfo(unzippedFile));
					}
				}
			}

			switch (parser)
			{
				case Parser.Static:
					m_statics = files;
					break;
				case Parser.Comment:
					m_comments = files;
					break;
				default:
					m_blogs = files;
					break;
			}
		}

		/// <summary>
		/// Converts the Simple PHP Blog entity to a WordPress Post entity.
		/// </summary>
		/// <param name="blogEntity">The blog entity.</param>
		/// <returns></returns>
		private WpPostEntity ConvertBlogEntity(PhpBlogEntity blogEntity)
		{
			var excerpt = string.Empty;
			if (blogEntity.Content.Contains("<br />"))
			{
				excerpt = blogEntity.Content.Substring(0, blogEntity.Content.IndexOf("<br />"));
			}

			var postEntity = new WpPostEntity
								 {
									 ID = null,
									 PostAuthor = 1,
									 PostDate = blogEntity.DateConverted,
									 PostDateGmt = blogEntity.DateConverted.ToUniversalTime(),
									 PostContent = blogEntity.Content,
									 PostTitle = blogEntity.Subject,
									 PostExcerpt = excerpt,
									 PostStatus = "publish",
									 CommentStatus = "open",
									 PingStatus = "open",
									 PostPassword = string.Empty,
									 PostName = blogEntity.Subject,
									 ToPing = string.Empty,
									 Pinged = string.Empty,
									 PostModified = blogEntity.DateConverted,
									 PostModifiedGmt = blogEntity.DateConverted.ToUniversalTime(),
									 PostContentFiltered = string.Empty,
									 PostParent = 0,
									 Guid = Guid.NewGuid().ToString().ToUpper(),
									 MenuOrder = 0,
									 PostType = "post",
									 PostMimeType = string.Empty,
									 RelatedLink = blogEntity.RelatedLink,
									 CommentCount = (blogEntity.Comments ?? new List<PhpCommentEntity>()).Count
								 };
			// Comments
			if (blogEntity.Comments != null)
			{
				foreach (var c in blogEntity.Comments)
				{
					if (postEntity.Comments == null)
					{
						postEntity.Comments = new List<WpCommentEntity>();
					}
					var comment = ConvertCommentEntity(c);

					var commentEntity =
						postEntity.Comments.Find(
							item =>
							item.Date == comment.Date && item.Author == comment.Author &&
							item.Content == comment.Content);

					if (commentEntity == null)
					{
						postEntity.Comments.Add(comment);
					}
				}
			}

			// Categories
			var cats = new List<WpCategoryEntity>();

			if (blogEntity.CategoryList == null)
			{
				blogEntity.CategoryList = new List<PhpCategoryEntity>();
			}

			if (postEntity.Categories == null)
			{
				postEntity.Categories = new List<WpCategoryEntity>();
			}

			if (m_categories != null)
			{
				if (!string.IsNullOrEmpty(blogEntity.Categories))
				{
					var categories = blogEntity.Categories.Split(',');
					foreach (var category in categories)
					{
						var phpCategoryEntity = m_categories.Find(item => item.ID == category);
						if (phpCategoryEntity != null)
						{
							var wpCategoryEntity = ConvertCategoryEntity(phpCategoryEntity);

							var categoryEntity =
								postEntity.Categories.Find(item => item.Description == wpCategoryEntity.Description);

							if (categoryEntity == null)
							{
								cats.Add(wpCategoryEntity);
							}
							postEntity.Categories = cats;
						}
					}
				}
				else
				{
					var phpCategoryEntity = m_categories.Find(item => item.Description == "General");
					if (phpCategoryEntity != null)
					{
						var wpCategoryEntity = ConvertCategoryEntity(phpCategoryEntity);

						var categoryEntity =
							postEntity.Categories.Find(item => item.Description == wpCategoryEntity.Description);

						if (categoryEntity == null)
						{
							cats.Add(wpCategoryEntity);
						}
						postEntity.Categories = cats;
					}
				}
			}

			if (blogEntity.CategoryList != null && blogEntity.CategoryList.Count > 0)
			{
				postEntity.PostCategory = Convert.ToInt32(DefaultCategory);
			}
			else if (!string.IsNullOrEmpty(blogEntity.Categories))
			{
				if ((blogEntity.Categories ?? string.Empty).Contains(","))
				{
					var categories = (blogEntity.Categories ?? string.Empty).Split(',');
					postEntity.PostCategory = Convert.ToInt32(categories[0]);
				}
				else
				{
					postEntity.PostCategory = Convert.ToInt32(blogEntity.Categories);
					if (postEntity.PostCategory == 0)
					{
						postEntity.PostCategory = Convert.ToInt32(DefaultCategory);
					}
				}
			}
			else
			{
				postEntity.PostCategory = Convert.ToInt32(DefaultCategory);
			}

			return postEntity;
		}

		/// <summary>
		/// Converts the static entity.
		/// </summary>
		/// <param name="staticEntity">The static entity.</param>
		/// <returns></returns>
		private WpPostEntity ConvertStaticEntity(PhpStaticEntity staticEntity)
		{
			var excerpt = string.Empty;
			if (staticEntity.Content.Contains("<br />"))
			{
				excerpt = staticEntity.Content.Substring(0, staticEntity.Content.IndexOf("<br />"));
			}

			var postEntity = new WpPostEntity
			{
				ID = null,
				PostAuthor = 1,
				PostDate = staticEntity.DateConverted,
				PostDateGmt = staticEntity.DateConverted.ToUniversalTime(),
				PostContent = staticEntity.Content,
				PostTitle = staticEntity.Subject,
				PostExcerpt = excerpt,
				PostStatus = "publish",
				CommentStatus = "open",
				PingStatus = "open",
				PostPassword = string.Empty,
				PostName = staticEntity.Subject,
				ToPing = string.Empty,
				Pinged = string.Empty,
				PostModified = staticEntity.DateConverted,
				PostModifiedGmt = staticEntity.DateConverted.ToUniversalTime(),
				PostContentFiltered = string.Empty,
				PostParent = 0,
				Guid = Guid.NewGuid().ToString().ToUpper(),
				MenuOrder = 0,
				PostType = "page",
				PostMimeType = string.Empty,
				RelatedLink = string.Empty,
				CommentCount = 0
			};

			return postEntity;
		}

		/// <summary>
		/// Converts the category entity.
		/// </summary>
		/// <param name="entity">The entity.</param>
		/// <returns></returns>
		private WpCategoryEntity ConvertCategoryEntity(PhpCategoryEntity entity)
		{
			var wpCategoryEntity = new WpCategoryEntity();
			try
			{
				wpCategoryEntity.ID = Convert.ToInt32(entity.ID);
				wpCategoryEntity.Description = entity.Description;
				wpCategoryEntity.ParentID = Convert.ToInt32(entity.ParentID);
			}
			catch
			{
				wpCategoryEntity.ID = 1;
				wpCategoryEntity.Description = "General";
				wpCategoryEntity.ParentID = 0;
			}
			return wpCategoryEntity;
		}

		/// <summary>
		/// Converts the Simple PHP Comment entity to a WordPress Comment entity.
		/// </summary>
		/// <param name="commentEntity">The comment entity.</param>
		/// <returns></returns>
		private WpCommentEntity ConvertCommentEntity(PhpCommentEntity commentEntity)
		{
			var comment = new WpCommentEntity
							  {
								  ID = null,
								  Agent = string.Empty,
								  Approved = 1,
								  Author = commentEntity.Name,
								  Content = commentEntity.Content,
								  Date = commentEntity.DateConverted,
								  DateGmt = commentEntity.DateConverted.ToUniversalTime(),
								  Karma = 0,
								  Email = commentEntity.Email,
								  IP = commentEntity.IpAddress,
								  PostID = null,
								  Type = "reply",
								  Url = commentEntity.Url,
								  UserID = null,
								  ParentID = null
							  };
			return comment;
		}

		/// <summary>
		/// Parses the content.
		/// </summary>
		/// <param name="content">The content.</param>
		/// <returns></returns>
		private string ParseContent(string content)
		{
			// HTML formatting
			var htmlTags = new List<string>
                               {
                                   "[b]",
                                   "[/b]",
                                   "[i]",
                                   "[/i]",
                                   "[u]",
                                   "[/u]",
                                   "[center]",
                                   "[/center]",
                                   "[pre]",
                                   "[/pre]",
                                   "[blockquote]",
                                   "[/blockquote]",
                                   "[code]",
                                   "[/code]",
                                   "[strong]",
                                   "[/strong]",
                                   "[em]",
                                   "[/em]",
                                   "[h1]",
                                   "[/h1]",
                                   "[h2]",
                                   "[/h2]",
                                   "[h3]",
                                   "[/h3]",
                                   "[h4]",
                                   "[/h4]",
                                   "[h5]",
                                   "[/h5]",
                                   "[h6]",
                                   "[/h6]",                             
                                   "[del]",
                                   "[/del]",
                                   "[ins]",
                                   "[/ins]",
                                   "[strike]",
                                   "[/strike]",
                               };


            while (content.Contains("[html]"))
            {
                int start = content.IndexOf("[html]");
                int end = content.IndexOf("[/html]", start, StringComparison.InvariantCultureIgnoreCase);

                var html = content.Substring(start, end - start + 7);

                // Get the content in the html tag
                var htmlContent = content.Substring(start + 6, end - start - 6);

                // Strip newlines from html
                htmlContent = htmlContent.Replace(Environment.NewLine, string.Empty);

                // Decode the contents to html
                var newHtml = HttpUtility.HtmlDecode(htmlContent);

                content = content.Replace(html, newHtml);
            }

			content = content.Replace("[more]", "<!--more-->");
			// content = content.Replace("&#039;", "'");

			content = content.Replace("&lt;hr&gt;", "<hr>");
			content = content.Replace("&lt;hr/&gt;", "<hr/>");
			content = content.Replace("�", "&#039;");
			content = content.Replace("’", "&#039;");

			foreach (var tag in htmlTags)
			{
				var replacement = tag.Replace("[", "<").Replace("]", ">");
				content = content.Replace(tag, replacement);
			}

			// [img=images/pope_1.png popup=false]
			// <img src="images/pope_1.png">

			while (content.Contains("[img"))
			{
				int start = content.IndexOf("[img");
				int end = content.IndexOf("]", start, StringComparison.InvariantCultureIgnoreCase);
				var img = content.Substring(start, end - start + 1);
				var newImg = img;
				var imgSrc = string.Format("<img src=\"{0}", HttpRoot);
				newImg = newImg.Replace("[img=", imgSrc);
				newImg = newImg.Replace(" popup=true", string.Empty);
				newImg = newImg.Replace(" popup=false", string.Empty);
				newImg = newImg.Replace("]", "\">");
				content = content.Replace(img, newImg);
			}

			while (content.Contains("[url"))
			{
				int start = content.IndexOf("[url");
				int end = content.IndexOf("/url]", start, StringComparison.InvariantCultureIgnoreCase);
				var url = content.Substring(start, end - start + 5);
				var newUrl = url;
				newUrl = newUrl.Replace("[url=", "<a href=\"");
				newUrl = newUrl.Replace("[/url]", "</a>");
				newUrl = newUrl.Replace(" new=true]", "\" target=\"_blank\">");
				newUrl = newUrl.Replace("]", "\">");
				content = content.Replace(url, newUrl);
			}
			return content;
		}

		/// <summary>
		/// Parses the file.
		/// </summary>
		/// <param name="file">The file.</param>
		private bool ParseBlogEntry(FileInfo file)
		{
			if (ValidateFile(file, Parser.BlogEntry))
			{
				var content = m_fileContent.ToString().Split('|');
				var blogEntity = new PhpBlogEntity();

				for (int i = 0; i < content.Length; i++)
				{
					blogEntity.FileName = file.Name.Replace(".gz", string.Empty).Replace(".txt", string.Empty);
					switch (content[i])
					{
						case "VERSION":
							try { blogEntity.Version = content[i + 1]; }
							catch { blogEntity.Version = string.Empty; }
							break;
						case "SUBJECT":
							try { blogEntity.Subject = ParseContent(content[i + 1]); }
							catch { blogEntity.Subject = string.Empty; }
							break;
						case "CONTENT":
							try { blogEntity.Content = ParseContent(content[i + 1]); }
							catch { blogEntity.Content = string.Empty; }
							blogEntity.Content = blogEntity.Content.Replace(Environment.NewLine, "<br />");
							break;
						case "CATEGORIES":
							try { blogEntity.Categories = content[i + 1]; }
							catch { blogEntity.Categories = string.Empty; }
							break;
						case "relatedlink":
							try { blogEntity.RelatedLink = content[i + 1]; }
							catch { blogEntity.RelatedLink = string.Empty; }
							break;
						case "IP-ADDRESS":
							try { blogEntity.IpAddress = content[i + 1]; }
							catch { blogEntity.IpAddress = string.Empty; }
							break;
						case "DATE":
							try { blogEntity.Date = Convert.ToInt64(content[i + 1]); }
							catch { blogEntity.Date = 0; }
							break;
						case "CREATEDBY":
							try { blogEntity.CreatedBy = content[i + 1]; }
							catch { blogEntity.CreatedBy = string.Empty; }
							break;
					}
				}

				// Categories
				if (string.IsNullOrEmpty(blogEntity.Categories))
				{
					blogEntity.Categories = "1";
				}

				if (m_blogEntries == null)
				{
					m_blogEntries = new List<PhpBlogEntity>();
				}

				var phpBlogEntity =
					m_blogEntries.Find(
						item =>
						item.Date == blogEntity.Date && item.Subject == blogEntity.Subject &&
						item.FileName == blogEntity.FileName);

				if (phpBlogEntity == null)
				{
					m_blogEntries.Add(blogEntity);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parses the static page.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <returns></returns>
		private bool ParseStaticPage(FileInfo file)
		{
			if (ValidateFile(file, Parser.Static))
			{
				var content = m_fileContent.ToString().Split('|');
				var staticEntity = new PhpStaticEntity();

				for (int i = 0; i < content.Length; i++)
				{
					staticEntity.FileName = file.Name.Replace(".gz", string.Empty).Replace(".txt", string.Empty);
					switch (content[i])
					{
						case "VERSION":
							try { staticEntity.Version = content[i + 1]; }
							catch { staticEntity.Version = string.Empty; }
							break;
						case "SUBJECT":
							try { staticEntity.Subject = ParseContent(content[i + 1]); }
							catch { staticEntity.Subject = string.Empty; }
							break;
						case "CONTENT":
							try { staticEntity.Content = ParseContent(content[i + 1]); }
							catch { staticEntity.Content = string.Empty; }
							staticEntity.Content = staticEntity.Content.Replace(Environment.NewLine, "<br />");
							break;
						case "MENU_VISIBLE":
							try { staticEntity.MenuVisible = content[i + 1]; }
							catch { staticEntity.MenuVisible = string.Empty; }
							break;
						case "DATE":
							try { staticEntity.Date = Convert.ToInt64(content[i + 1]); }
							catch { staticEntity.Date = 0; }
							break;
					}
				}

				if (m_staticEntries == null)
				{
					m_staticEntries = new List<PhpStaticEntity>();
				}
				var phpStaticEntity =
					m_staticEntries.Find(
						item =>
						item.Date == staticEntity.Date && item.Subject == staticEntity.Subject &&
						item.FileName == staticEntity.FileName);

				if (phpStaticEntity == null)
				{
					m_staticEntries.Add(staticEntity);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Parses the categories.
		/// </summary>
		/// <param name="categoryList">The category list.</param>
		/// <returns></returns>
		private List<PhpCategoryEntity> ParseCategories(string categoryList)
		{
			var list = new List<PhpCategoryEntity>();

			if (categoryList.Trim().Contains("|"))
			{
				var categories = categoryList.Split('|');
				if ((categories.Length) % 3 == 0)
				{
					int i = 0;
					while (i + 3 <= categories.Length)
					{
						// ID | Description | Depth
						//1|General|0|
						//2|News|0|
						//6|Announcements|1|
						//7|Events|1|
						//8|Miscellaneous|1|
						//3|Humour|0|
						//9|General Humour|1|
						//10|Tasteless Humour|1|
						//4|Technology|0|
						//5|Writing|0|
						//11|Quotations|1|
						//13|Rantings|1|
						//12|Science Stuff|0|

						var categoryEntity = new PhpCategoryEntity
												 {
													 ID = categories[i],
													 Description = categories[i + 1],
													 Depth = Convert.ToInt32(categories[i + 2])
												 };
						list.Add(categoryEntity);
						i = i + 3;
					}
				}
				else
				{
					CreateCategoryList(list);
				}
			}
			else
			{
				CreateCategoryList(list);
			}

			var newList = new List<PhpCategoryEntity>();

			for (int i = 0; i < list.Count; i++)
			{
				var entity = new PhpCategoryEntity
								 {
									 ID = list[i].ID,
									 Description = list[i].Description
								 };

				// Depth calculator
				int currentDepth;
				int previousDepth;
				if (i == 0)
				{
					previousDepth = currentDepth = 0;
				}
				else
				{
					previousDepth = list[i - 1].Depth;
					currentDepth = list[i].Depth;
				}
				if (previousDepth < currentDepth)
				{
					entity.ParentID = list[i - 1].ID;
				}
				else if (previousDepth == currentDepth && currentDepth != 0)
				{
					entity.ParentID = newList[i - 1].ParentID;
				}
				else
				{
					entity.ParentID = "0";
				}
				newList.Add(entity);
			}

			return newList;
		}

		/// <summary>
		/// Creates the category list if input is invalid.
		/// </summary>
		/// <param name="list">The list.</param>
		private void CreateCategoryList(List<PhpCategoryEntity> list)
		{
			var categoryEntity = new PhpCategoryEntity
									 {
										 ID = "1",
										 Description = "General",
										 ParentID = "0"
									 };
			list.Add(categoryEntity);
		}

		/// <summary>
		/// Parses the comment file.
		/// </summary>
		/// <param name="file">The comment file.</param>
		private bool ParseComment(FileInfo file)
		{
			if (ValidateFile(file, Parser.Comment))
			{
				var content = m_fileContent.ToString().Split('|');
				var commentEntity = new PhpCommentEntity();

				for (int i = 0; i < content.Length; i++)
				{
					switch (content[i])
					{
						case "VERSION":
							try
							{
								commentEntity.Version = content[i + 1];
							}
							catch
							{
								commentEntity.Version = string.Empty;
							}
							break;
						case "NAME":
							try
							{
								commentEntity.Name = ParseContent(content[i + 1]);
							}
							catch
							{
								commentEntity.Name = string.Empty;
							}
							break;
						case "DATE":
							try
							{
								commentEntity.Date = Convert.ToInt64(content[i + 1]);
							}
							catch
							{
								commentEntity.Date = 0;
							}
							break;
						case "CONTENT":
							try
							{
								commentEntity.Content = ParseContent(content[i + 1]);
							}
							catch
							{
								commentEntity.Content = string.Empty;
							}
							commentEntity.Content = commentEntity.Content.Replace(Environment.NewLine, "<br />");
							break;
						case "EMAIL":
							try
							{
								commentEntity.Email = content[i + 1];
							}
							catch
							{
								commentEntity.Email = string.Empty;
							}
							break;
						case "URL":
							try
							{
								commentEntity.Url = content[i + 1];
							}
							catch
							{
								commentEntity.Url = string.Empty;
							}
							break;
						case "IP-ADDRESS":
							try
							{
								commentEntity.IpAddress = content[i + 1];
							}
							catch
							{
								commentEntity.IpAddress = string.Empty;
							}
							break;
						case "MODERATIONFLAG":
							try
							{
								commentEntity.ModerationFlag = content[i + 1];
							}
							catch
							{
								commentEntity.ModerationFlag = string.Empty;
							}
							break;
					}
				}
				if (m_commentEntries == null)
				{
					m_commentEntries = new List<PhpCommentEntity>();
				}

				var phpCommentEntity =
					m_commentEntries.Find(
						item =>
						item.Date == commentEntity.Date && item.Name == commentEntity.Name &&
						item.Content == commentEntity.Content);

				if (phpCommentEntity == null)
				{
					m_commentEntries.Add(commentEntity);
				}
				return true;
			}
			return false;
		}

		/// <summary>
		/// Validates the file.
		/// </summary>
		/// <param name="file">The file.</param>
		/// <param name="parser">The parser.</param>
		/// <returns></returns>
		private bool ValidateFile(FileInfo file, Parser parser)
		{
			var fileSystemHelper = new FileSystemHelper();
			m_fileContent = new StringBuilder();
			var sr = new StreamReader(fileSystemHelper.GetFileNameFromFileInfo(file), Encoding.Default);
			m_fileContent.Append(sr.ReadToEnd());

			return parser == Parser.Comment
					   ? m_fileContent.ToString().StartsWith("VERSION|") &&
						 m_fileContent.ToString().Contains("|NAME|") &&
						 m_fileContent.ToString().Contains("|CONTENT|")
					   : m_fileContent.ToString().StartsWith("VERSION|") &&
						 m_fileContent.ToString().Contains("|SUBJECT|") &&
						 m_fileContent.ToString().Contains("|CONTENT|");
		}

		/// <summary>
		/// Generates the SQL from the WordPress entities.
		/// </summary>
		/// <param name="postEntities">The post entities.</param>
		/// <returns></returns>
		private StringBuilder GenerateSql(List<WpPostEntity> postEntities)
		{
			var sb = new StringBuilder();
			if (WpPostTable != null)
			{
				// Truncate tables
				sb.AppendFormat("TRUNCATE TABLE {0}", WpTermTaxonomyTable);
				sb.AppendLine(";");
				sb.AppendFormat("TRUNCATE TABLE {0}", WpTermTable);
				sb.AppendLine(";");
				sb.AppendFormat("TRUNCATE TABLE {0}", WpMetaTable);
				sb.AppendLine(";");
				sb.AppendFormat("TRUNCATE TABLE {0}", WpCommentTable);
				sb.AppendLine(";");
				sb.AppendFormat("TRUNCATE TABLE {0}", WpPostTable);
				sb.AppendLine(";");
				sb.AppendFormat("TRUNCATE TABLE {0}", WpTermRelationshipTable);
				sb.AppendLine(";");

				// Categories
				foreach (var c in m_categories)
				{
					sb.AppendFormat(
						"INSERT INTO {0} (term_id, name, slug, term_group) SELECT {1}, '{2}', '{3}', {4}",
						WpTermTable, c.ID, c.Description, c.Description.ToLower().Replace(" ", string.Empty), 0);
					sb.AppendLine(";");
				}

				foreach (var c in m_categories)
				{
					sb.AppendFormat(
						"INSERT INTO {0} (term_taxonomy_id, term_id, taxonomy, description, parent, count) SELECT {1}, {2}, '{3}', '{4}', {5}, {6}",
						WpTermTaxonomyTable, "null", c.ID, "category", c.Description, c.ParentID, 0);
					sb.AppendLine(";");
				}

				foreach (var p in postEntities)
				{
					// Posts
					sb.AppendFormat(
                        "INSERT INTO {0} (ID, post_author, post_date, post_date_gmt, post_content, post_title, post_excerpt, post_status, comment_status, ping_status, post_password, post_name, to_ping, pinged, post_modified, post_modified_gmt, post_content_filtered, post_parent, guid, menu_order, post_type, post_mime_type, comment_count) SELECT {1}, '{2}', '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}', '{16}', '{17}', '{18}', '{19}', '{20}', '{21}', '{22}', '{23}'",
						WpPostTable, "null", p.PostAuthor, p.PostDate, p.PostDateGmt, p.PostContent, p.PostTitle,
						p.PostExcerpt, p.PostStatus, p.CommentStatus, p.PingStatus, p.PostPassword,
						p.PostName, p.ToPing, p.Pinged, p.PostModified, p.PostModifiedGmt, p.PostContentFiltered,
						p.PostParent, p.Guid, p.MenuOrder, p.PostType, p.PostMimeType, p.CommentCount);
					sb.AppendLine(";");

					// Get post ID
					sb.AppendLine("SET @postId = LAST_INSERT_ID();");

					// Categories
					if (p.Categories != null)
					{
						foreach (WpCategoryEntity wpCategoryEntity in p.Categories)
						{
							sb.AppendFormat("SET @categoryId = (SELECT term_taxonomy_id FROM {0} WHERE taxonomy = 'category' and description = '{1}')",
													 WpTermTaxonomyTable, wpCategoryEntity.Description);
							sb.AppendLine(";");

							sb.AppendFormat(
								"INSERT IGNORE INTO {0} (object_id, term_taxonomy_id, term_order) SELECT {1}, {2}, {3}",
								WpTermRelationshipTable, "@postId", "@categoryId", "0");
							sb.AppendLine(";");
						}
					}

					// Related Links
					if (!string.IsNullOrEmpty(p.RelatedLink))
					{
						sb.AppendFormat(
							"INSERT INTO {0} (meta_id, post_id, meta_key, meta_value) SELECT {1}, {2}, '{3}', '{4}'",
							WpMetaTable, "null", "@postId", "relatedlink", p.RelatedLink);
						sb.AppendLine(";");
					}

					// Comments
					if (p.Comments != null)
					{
						if (p.Comments.Count > 0)
						{
							sb.AppendLine("SET @postId = LAST_INSERT_ID();");
						}

						foreach (var c in p.Comments)
						{
							sb.AppendFormat(
								"INSERT INTO {0} (comment_ID, comment_post_ID, comment_author, comment_author_email, comment_author_url, comment_author_IP, comment_date, comment_date_gmt, comment_content, comment_karma, comment_approved, comment_agent, comment_type, comment_parent, user_id) SELECT {1}, {2}, '{3}', '{4}', '{5}', '{6}', '{7}', '{8}', '{9}', '{10}', '{11}', '{12}', '{13}', '{14}', '{15}'",
								WpCommentTable, "null", "@postId", c.Author, c.Email, c.Url, c.IP, c.Date, c.DateGmt,
								c.Content, c.Karma,
								c.Approved, c.Agent, c.Type, c.ParentID, c.UserID);
							sb.AppendLine(";");
						}
					}
				}

				// Categories Counter
				foreach (var c in m_categories)
				{
					sb.AppendFormat(
						"SET @categoryId = (SELECT term_taxonomy_id FROM {0} WHERE taxonomy = 'category' and description = '{1}')",
						WpTermTaxonomyTable, c.Description);
					sb.AppendLine(";");
					sb.AppendFormat(
						"SET @categoryCount = (SELECT count(term_taxonomy_id) FROM {0} WHERE term_taxonomy_id = @categoryId)",
						WpTermRelationshipTable);
					sb.AppendLine(";");
					sb.AppendFormat(
						"UPDATE  `{0}` SET `count` = @categoryCount WHERE `{0}`.`term_taxonomy_id` = @categoryId;",
						WpTermTaxonomyTable);
					sb.AppendLine(";");
				}
			}
			return sb;
		}
	}
}
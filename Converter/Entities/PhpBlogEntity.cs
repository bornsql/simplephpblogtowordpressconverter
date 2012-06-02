using System;
using System.Collections.Generic;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter.Entities
{
    internal class PhpBlogEntity
    {
        private string m_fileName;
        private string m_version;
        private string m_subject;
        private string m_content;
        private string m_categories;
        private string m_relatedLink;
        private string m_ipAddress;
        private long? m_date;
        private string m_createdBy;
        private List<PhpCategoryEntity> m_categoryList;

        /// <summary>
        /// Gets or sets the name of the file excluding extension, e.g. entry090218-132852.
        /// </summary>
        /// <value>The name of the file.</value>
        public string FileName
        {
            get { return m_fileName ?? string.Empty; }
            set { m_fileName = value ?? string.Empty; }
        }

        public string Version
        {
            get { return m_version ?? string.Empty; }
            set { m_version = value ?? string.Empty; }
        }

        public string Subject
        {
            get { return m_subject ?? string.Empty; }
            set { m_subject = value ?? string.Empty; }
        }

        public string Content
        {
            get { return m_content ?? string.Empty; }
            set { m_content = value ?? string.Empty; }
        }

        public string RelatedLink
        {
            get { return m_relatedLink ?? string.Empty; }
            set { m_relatedLink = value ?? string.Empty; }
        }

        public string IpAddress
        {
            get { return m_ipAddress ?? string.Empty; }
            set { m_ipAddress = value ?? string.Empty; }
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        /// <value>The date in PHP strtotime() format (UTC, number of seconds since 1970/01/01).</value>
        public long? Date
        {
            get { return m_date ?? 0; }
            set { m_date = value ?? 0; }
        }

        /// <summary>
        /// Gets the converted date.
        /// </summary>
        /// <value>The converted date in local time.</value>
        public DateTime DateConverted
        {
            get
            {
                // Starting point is January 1 1970 00:00:00 UTC
                DateTime startingPoint = new DateTime(1970, 1, 1);
                var x = startingPoint.AddSeconds(Date.Value);
                return x.ToLocalTime();
            }
        }

        public string CreatedBy
        {
            get { return m_createdBy ?? string.Empty; }
            set { m_createdBy = value ?? string.Empty; }
        }

        public string Categories
        {
            get { return m_categories ?? string.Empty; }
            set { m_categories = value ?? string.Empty; }
        }

        public List<PhpCategoryEntity> CategoryList
        {
            get { return m_categoryList ?? new List<PhpCategoryEntity>(); }
            set { m_categoryList = value ?? new List<PhpCategoryEntity>(); }
        }

        /// <summary>
        /// Gets or sets the comments.
        /// </summary>
        /// <value>The comments.</value>
        public List<PhpCommentEntity> Comments { get; set; }
    }
}

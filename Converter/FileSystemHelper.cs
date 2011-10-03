using System;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter
{
    internal class FileSystemHelper
    {
        private readonly string m_fileName = string.Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="FileSystemHelper"/> class.
        /// </summary>
        public FileSystemHelper()
        {
            m_fileName = string.Format("sphpb_wp_convert_{0}.sql", DateTime.Now.ToString("yyyyMMddhhmmss"));
        }

        /// <summary>
        /// Writes the SQL file.
        /// </summary>
        /// <param name="sqlString">The SQL string.</param>
        /// <param name="targetFolder">The target folder.</param>
        internal void FileWrite(StringBuilder sqlString, string targetFolder)
        {
            string fileName = string.Format(@"{0}\{1}", targetFolder, m_fileName);
            FileInfo fi = new FileInfo(fileName);
            if (fi.Exists)
            {
                fi.Delete();
            }
            File.WriteAllText(fileName, sqlString.ToString());
        }

        /// <summary>
        /// Gets the file name from file info.
        /// </summary>
        /// <param name="fileInfo">The file info.</param>
        /// <returns></returns>
        internal string GetFileNameFromFileInfo(FileInfo fileInfo)
        {
            return string.Format(@"{0}\{1}", fileInfo.DirectoryName, fileInfo.Name);
        }

        /// <summary>
        /// Unzips the file.
        /// </summary>
        /// <param name="compressedFile">The compressed file.</param>
        /// <returns></returns>
        internal FileInfo UnzipFile(FileInfo compressedFile)
        {
            FileStream fsSource = new FileStream(GetFileNameFromFileInfo(compressedFile), FileMode.Open, FileAccess.Read, FileShare.Read);
            GZipStream gz = new GZipStream(fsSource, CompressionMode.Decompress, true);
            byte[] data = new byte[4];
            fsSource.Position = (int)fsSource.Length - 4;
            fsSource.Read(data, 0, 4);
            fsSource.Position = 0;

            int bufferLength = BitConverter.ToInt32(data, 0);

            byte[] buffer = new byte[bufferLength + 100];
            int readOffset = 0, totalBytes = 0;

            while (true)
            {
                int bytesRead = gz.Read(buffer, readOffset, 100);
                if (bytesRead == 0)
                {
                    break;
                }
                readOffset += bytesRead;
                totalBytes += bytesRead;
            }

            string fileName = GetFileNameFromFileInfo(compressedFile).Replace(compressedFile.Extension, ".txt").Replace(".txt.txt", ".txt");

            FileStream fsDestination = new FileStream(GetFileNameFromFileInfo(compressedFile).Replace(compressedFile.Extension, ".txt").Replace(".txt.txt", ".txt"),
                                                      FileMode.Create);
            fsDestination.Write(buffer, 0, totalBytes);

            fsSource.Close();
            fsSource.Dispose();
            gz.Close();
            gz.Dispose();
            fsDestination.Close();
            fsDestination.Dispose();

            return new FileInfo(GetFileNameFromFileInfo(compressedFile).Replace(compressedFile.Extension, ".txt").Replace(".txt.txt", ".txt"));
        }
    }
}

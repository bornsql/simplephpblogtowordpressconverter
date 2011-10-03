using System;

namespace ITS.Tools.SimplePHPBlogToWordPressConverter.Entities
{
    internal class PhpCommentEntity
    {
        private string m_version;
        private string m_name;
        private long? m_date;
        private string m_content;
        private string m_email;
        private string m_url;
        private string m_ipAddress;
        private string m_moderationFlag;

        public string Version
        {
            get { return m_version ?? string.Empty; }
            set { m_version = value ?? string.Empty; }
        }

        public string Name
        {
            get { return m_name ?? string.Empty; }
            set { m_name = value ?? string.Empty; }
        }

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

        public string Content
        {
            get { return m_content ?? string.Empty; }
            set { m_content = value ?? string.Empty; }
        }

        public string Email
        {
            get { return m_email ?? string.Empty; }
            set { m_email = value ?? string.Empty; }
        }

        public string Url
        {
            get { return m_url ?? string.Empty; }
            set { m_url = value ?? string.Empty; }
        }

        public string IpAddress
        {
            get { return m_ipAddress ?? string.Empty; }
            set { m_ipAddress = value ?? string.Empty; }
        }

        public string ModerationFlag
        {
            get { return m_moderationFlag ?? string.Empty; }
            set { m_moderationFlag = value ?? string.Empty; }
        }
    }
}

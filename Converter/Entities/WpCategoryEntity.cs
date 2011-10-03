namespace ITS.Tools.SimplePHPBlogToWordPressConverter.Entities
{
    public class WpCategoryEntity
    {
        public int ID { get; set; }
        public int ParentID { get; set; }
        public string Description { get; set; }
    }
}

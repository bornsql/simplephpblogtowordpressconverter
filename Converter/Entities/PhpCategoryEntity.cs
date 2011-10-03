namespace ITS.Tools.SimplePHPBlogToWordPressConverter.Entities
{
    internal class PhpCategoryEntity
    {
        public string ID { get; set; }
        public string Description { get; set; }
        public int Depth { get; set; }
        public string ParentID { get; set; }
    }
}

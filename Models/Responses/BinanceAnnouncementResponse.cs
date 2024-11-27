namespace crraut.Models.Responses
{
    public class BinanceAnnouncementResponse
    {
        public string Code { get; set; }
        public object Message { get; set; }
        public object MessageDetail { get; set; }
        public Info Data { get; set; }
        public bool Success { get; set; }
    }

    public class Info
    {
        public List<Catalog> Catalogs { get; set; }
    }

    public class Catalog
    {
        public int CatalogId { get; set; }
        public object ParentCatalogId { get; set; }
        public string Icon { get; set; }
        public string CatalogName { get; set; }
        public object Description { get; set; }
        public int CatalogType { get; set; }
        public int Total { get; set; }
        public List<Article> Articles { get; set; }
        public List<object> Catalogs { get; set; }
    }

    public class Article
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Title { get; set; }
        public int Type { get; set; }
        public long ReleaseDate { get; set; }
    }
}

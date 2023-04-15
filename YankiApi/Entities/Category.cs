namespace YankiApi.Entities
{
    public class Category : BaseEntity
    {
        public string? Name { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        public string Image { get; internal set; }
    }
}

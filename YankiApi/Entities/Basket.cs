namespace YankiApi.Entities
{
    public class Basket : BaseEntity
    {
        public int? ProductId { get; set; }
        public Product? Product { get; set; }
        public int Price { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public int Count { get; set; }
    }
}

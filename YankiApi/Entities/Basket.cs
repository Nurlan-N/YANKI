namespace YankiApi.Entities
{
    /// <summary>
    /// User Cart
    /// </summary>
    public class Basket : BaseEntity
    {
        /// <summary>
        /// Cart item Id
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Product
        /// </summary>
        public Product? Product { get; set; }
        /// <summary>
        /// Item Price
        /// </summary>
        public int Price { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// User
        /// </summary>
        public AppUser? User { get; set; }
        /// <summary>
        /// Item Count
        /// </summary>
        public int Count { get; set; }
    }
}

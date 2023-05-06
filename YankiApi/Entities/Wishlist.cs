namespace YankiApi.Entities
{
    /// <summary>
    /// User Wishlist
    /// </summary>
    public class Wishlist : BaseEntity
    {
        /// <summary>
        /// Product id
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Product
        /// </summary>
        public Product? Product { get; set; }
        /// <summary>
        /// User id
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

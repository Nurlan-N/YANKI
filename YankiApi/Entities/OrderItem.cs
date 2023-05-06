namespace YankiApi.Entities
{
    /// <summary>
    /// Order Items
    /// </summary>
    public class OrderItem : BaseEntity
    {
        /// <summary>
        /// Item price
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Item Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Item Id
        /// </summary>
        public int? ProductId { get; set; }
        /// <summary>
        /// Order Product
        /// </summary>
        public Product? Product { get; set; }
        /// <summary>
        /// Order Id
        /// </summary>
        public int OrderId { get; set; }
    }
}

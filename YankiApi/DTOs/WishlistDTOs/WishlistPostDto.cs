namespace YankiApi.DTOs.WishlistDTOs
{   
    /// <summary>
    /// Wishlist Post
    /// </summary>
    public class WishlistPostDto
    {
        /// <summary>
        /// Product Id
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Product Image
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// Product Title
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Product Price
        /// </summary>
        public double? Price { get; set; }
        /// <summary>
        /// Product Count
        /// </summary>
        public int? Count { get; internal set; }
    }
}

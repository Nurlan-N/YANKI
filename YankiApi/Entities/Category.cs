using System.ComponentModel.DataAnnotations.Schema;

namespace YankiApi.Entities
{
    /// <summary>
    /// Category
    /// </summary>
    public class Category : BaseEntity
    {
        /// <summary>
        /// Category Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Category Products
        /// </summary>
        public IEnumerable<Product>? Products { get; set; }
        /// <summary>
        /// Category Image
        /// </summary>
        public string? Image { get; internal set; }
        /// <summary>
        /// Image
        /// </summary>
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
    }
}

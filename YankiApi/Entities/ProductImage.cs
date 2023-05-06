using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    /// <summary>
    /// Product Images
    /// </summary>
    public class ProductImage : BaseEntity
    {
        /// <summary>
        /// Image
        /// </summary>
        public string Image { get; set; }
        /// <summary>
        /// Product id
        /// </summary>
        public int ProductId { get; set; }
    }
}

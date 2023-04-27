using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace YankiApi.Entities
{
    public class Product : BaseEntity
    {
        /// <summary>
        /// Product Name
        /// </summary>
        public string? Title { get; set; }
        /// <summary>
        /// Product price
        /// </summary>
        public double Price { get; set; }
        /// <summary>
        /// Product Discount Price
        /// </summary>
        public double DiscountedPrice { get; set; }
        /// <summary>
        /// Product Extax
        /// </summary>
        public double ExTax { get; set; }
        /// <summary>
        /// Product Count
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// Product Description
        /// </summary>
        public string? Description { get; set; }
        /// <summary>
        /// Product Long Description
        /// </summary>
        public string? LongDescription { get; set; }
        /// <summary>
        /// Product Seria
        /// </summary>
        public string? Seria { get; set; }
        /// <summary>
        /// Product Code
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// Product Image
        /// </summary>
        public string? Image { get; set; }
        /// <summary>
        /// Product Image File
        /// </summary>
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        /// <summary>
        /// Product Reviews
        /// </summary>
        public IEnumerable<Review>? Reviews { get; set; }
        /// <summary>
        /// Product Imagess
        /// </summary>
        public List<ProductImage>? ProductImages { get; set; }
        /// <summary>
        /// Product Images Files
        /// </summary>
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        /// <summary>
        /// Product Category Id
        /// </summary>
        public int CategoryId { get; set; }
        /// <summary>
        /// Product Category
        /// </summary>
        public Category? Category { get; set; }
    }
}

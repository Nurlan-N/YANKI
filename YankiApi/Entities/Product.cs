using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace YankiApi.Entities
{
    public class Product : BaseEntity
    {
        public string? Title { get; set; }
        public double Price { get; set; }
        public double DiscountedPrice { get; set; }
        public double ExTax { get; set; }
        public int Count { get; set; }
        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public string? Seria { get; set; }
        public int Code { get; set; }
        public string? Image { get; set; }
        [NotMapped]
        public IEnumerable<IFormFile>? Files { get; set; }
        public IEnumerable<Basket>? Baskets { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        [NotMapped]
        public IFormFile? ImageFile { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

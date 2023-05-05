using System.ComponentModel.DataAnnotations.Schema;
using YankiApi.Entities;

namespace YankiApi.DTOs.ProductDTOs
{
    public class ProductGetDto
    {
        public int Id { get; set; }
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
        
        public IEnumerable<Basket>? Baskets { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }
        public List<ProductImage>? ProductImages { get; set; }
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
    }
}

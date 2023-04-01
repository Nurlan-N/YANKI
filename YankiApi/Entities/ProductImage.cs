using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    public class ProductImage : BaseEntity
    {
        public string Image { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}

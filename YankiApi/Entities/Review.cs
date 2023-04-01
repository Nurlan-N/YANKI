using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    public class Review : BaseEntity
    {
        public int ProductId { get; set; }
        public Product? Product { get; set; }
        public string? UserId { get; set; }
        public AppUser? User { get; set; }
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Description { get; set; }
       
    }
}

using System.ComponentModel.DataAnnotations;
using YankiApi.Enums;

namespace YankiApi.Entities
{
    public class Order : BaseEntity
    {
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
        public int No { get; set; }
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? State { get; set; }
        public OrderType Status { get; set; }
        public string? Commet { get; set; }
    }
}

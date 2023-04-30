using System.ComponentModel.DataAnnotations;
using YankiApi.Enums;

namespace YankiApi.Entities
{   
    /// <summary>
    /// 
    /// </summary>
    public class Order : BaseEntity
    {   
        /// <summary>
        /// 
        /// </summary>
        public AppUser? User { get; set; }
        /// <summary>
        /// 
        /// </summary>
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
        public double? TotalPrice { get; set; }
        public string? State { get; set; }
        public OrderType Status { get; set; }
        public string? Commet { get; set; }
    }
}

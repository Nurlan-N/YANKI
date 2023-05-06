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
        /// <summary>
        /// Order No
        /// </summary>
        public int No { get; set; }
        /// <summary>
        /// Order Items
        /// </summary>
        public IEnumerable<OrderItem>? OrderItems { get; set; }
        /// <summary>
        /// User Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// User SurName
        /// </summary>
        public string? SurName { get; set; }
        /// <summary>
        /// User Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// User Phone
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// User Country
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// User City
        /// </summary>
        public string? City { get; set; }
        /// <summary>
        /// User Address
        /// </summary>
        public string? Address { get; set; }
        /// <summary>
        /// User PostalCode
        /// </summary>
        public string? PostalCode { get; set; }
        /// <summary>
        /// Order Total Price
        /// </summary>
        public double? TotalPrice { get; set; }
        /// <summary>
        /// User State
        /// </summary>
        public string? State { get; set; }
        /// <summary>
        /// Order Status
        /// </summary>
        public OrderType Status { get; set; }
        /// <summary>
        /// Order Comment
        /// </summary>
        public string? Commet { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace YankiApi.Entities
{
    /// <summary>
    /// User
    /// </summary>
    public class AppUser : IdentityUser
    {
        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Surname
        /// </summary>
        public string? SurName { get; set; }
        /// <summary>
        /// Online
        /// </summary>
        public Nullable<DateTime> LastOnline { get; set; }
        /// <summary>
        /// Address
        /// </summary>
        public IEnumerable<Address>? Addresses { get; set; }
        /// <summary>
        /// Orders
        /// </summary>
        public IEnumerable<Order>? Orders { get; set; }
        /// <summary>
        /// Review
        /// </summary>
        public IEnumerable<Review>? Reviews { get; set; }
        /// <summary>
        /// Basket
        /// </summary>
        public List<Basket>? Baskets { get; set; }
        /// <summary>
        /// Wishlist
        /// </summary>
        public List<Wishlist>? Wishlist { get; set; }
    }
}

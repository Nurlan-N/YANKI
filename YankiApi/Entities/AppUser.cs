using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Net;

namespace YankiApi.Entities
{
    public class AppUser : IdentityUser
    {

        public string? Name { get; set; }
        [StringLength(20)]
        public string? SurName { get; set; }
        public Nullable<DateTime> LastOnline { get; set; }
        public IEnumerable<Address>? Addresses { get; set; }
        public IEnumerable<Order>? Orders { get; set; }
        public IEnumerable<Review>? Reviews { get; set; }

        public List<Basket>? Baskets { get; set; }
        public List<Wishlist>? Wishlist { get; set; }
    }
}

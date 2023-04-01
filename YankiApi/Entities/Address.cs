using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    public class Address : BaseEntity
    {
        public AppUser? User { get; set; }
        public string? UserId { get; set; }
        public string? Country { get; set; }
        public string? State { get; set; }
        public string? City { get; set; }
        public string? PostalCode { get; set; }
        public bool IsMain { get; set; }
        public string? Phone { get; set; }
    }
}

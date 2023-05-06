using System.ComponentModel.DataAnnotations;

namespace YankiApi.Entities
{
    /// <summary>
    /// Adress
    /// </summary>
    public class Address : BaseEntity
    {
        /// <summary>
        /// App User
        /// </summary>
        public AppUser? User { get; set; }
        /// <summary>
        /// User Id
        /// </summary>
        public string? UserId { get; set; }
        /// <summary>
        /// User Country
        /// </summary>
        public string? Country { get; set; }
        /// <summary>
        /// User State
        /// </summary>
        public string? State { get; set; }
        /// <summary>
        /// User City
        /// </summary>
        public string? City { get; set; }
        /// <summary>
        /// User Postal Code
        /// </summary>
        public string? PostalCode { get; set; }
        /// <summary>
        /// Main Address
        /// </summary>
        public bool IsMain { get; set; }
        /// <summary>
        /// User Phone
        /// </summary>
        public string? Phone { get; set; }
    }
}

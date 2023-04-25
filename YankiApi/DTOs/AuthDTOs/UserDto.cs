using System.ComponentModel.DataAnnotations;
using YankiApi.Entities;

namespace YankiApi.DTOs.AuthDTOs
{
    public class UserDto
    {
        [StringLength(20)]
        public string? Name { get; set; }
        [StringLength(20)]
        public string? SurName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string UserName { get; set; }
        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        public string? OldPassword { get; set; }

        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        public string? Password { get; set; }
        [Required, StringLength(20)]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string? ConfirimPassword { get; set; }
        /// <summary>
        /// User Phone
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// User Country
        /// </summary>
        public string? Country { get; set; }
        public string? PostalCode { get; set; }
    }
}

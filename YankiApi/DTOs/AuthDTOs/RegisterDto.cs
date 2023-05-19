using FluentValidation;
using System.ComponentModel.DataAnnotations;
using YankiApi.Entities;

namespace YankiApi.DTOs.AuthDTOs
{
    /// <summary>
    /// Register
    /// </summary>
    public class RegisterDto
    {
        /// <summary>
        /// Name
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// Last Name
        /// </summary>
        public string? SurName { get; set; }
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// User Phone
        /// </summary>
        public string? Phone { get; set; }
        /// <summary>
        /// Nick
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }
        /// <summary>
        /// User Role
        /// </summary>
        public string? RoleName { get; set; }
    }
    /// <summary>
    /// Validator
    /// </summary>
    public class RegisterDtoValidator : AbstractValidator<RegisterDto> 
    {
        /// <summary>
        /// Check
        /// </summary>
        public RegisterDtoValidator()
        {
            RuleFor(r => r.Email)
                .EmailAddress().WithMessage("Emailda Sehv")
                .NotEmpty().WithMessage("Email Bos ola bilmez");
            RuleFor(r => r.Name)
                .NotEmpty().WithMessage("Name Bos ola bilmez");
            RuleFor(r => r.SurName)
                .NotEmpty().WithMessage("Surname Bos ola bilmez");
            RuleFor(r => r.UserName)
                .NotEmpty().WithMessage("UserName Bos ola bilmez");
            RuleFor(r => r.Password)
               .NotEmpty().WithMessage("UserName Bos ola bilmez");
          

        }

    }
}

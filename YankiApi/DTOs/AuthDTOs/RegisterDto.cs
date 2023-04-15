using FluentValidation;
using System.ComponentModel.DataAnnotations;
using YankiApi.Entities;

namespace YankiApi.DTOs.AuthDTOs
{
    public class RegisterDto
    {
        public string? Name { get; set; }
        public string? SurName { get; set; }
        public string Email { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class RegisterDtoValidator : AbstractValidator<RegisterDto> 
    {
    
        public RegisterDtoValidator()
        {
            RuleFor(r => r.Email)
                .EmailAddress().WithMessage("Emailda Sehv")
                .NotEmpty().WithMessage("Email Bos ola bilmez");
        }

    }
}

using FluentValidation;

namespace YankiApi.DTOs.AuthDTOs
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {

        public LoginDtoValidator()
        {
            RuleFor(r => r.Email)
                .EmailAddress().WithMessage("Emailda Sehv")
                .NotEmpty().WithMessage("Email Bos ola bilmez");
            RuleFor(r => r.Password)
                .NotEmpty() .WithMessage("Password bos ola bilmez");
        }

    }
}

using FluentValidation;

namespace YankiApi.DTOs.AuthDTOs
{
    /// <summary>
    /// Log in
    /// </summary>
    public class LoginDto
    {
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
        /// <summary>
        /// Password
        /// </summary>
        public string? Password { get; set; }
    }
    /// <summary>
    /// Validator
    /// </summary>
    public class LoginDtoValidator : AbstractValidator<LoginDto>
    {
        /// <summary>
        /// Yoxlamalar
        /// </summary>
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

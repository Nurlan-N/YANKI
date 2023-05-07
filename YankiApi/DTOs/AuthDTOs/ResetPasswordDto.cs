using FluentValidation;

namespace YankiApi.DTOs.AuthDTOs
{
    /// <summary>
    /// Forgot Password
    /// </summary>
    public class ResetPasswordDto
    {
        /// <summary>
        /// Email
        /// </summary>
        public string? Email { get; set; }
    }

    public class ResetPasswordDtoValidator : AbstractValidator<ResetPasswordDto>
    {
        public ResetPasswordDtoValidator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }

}

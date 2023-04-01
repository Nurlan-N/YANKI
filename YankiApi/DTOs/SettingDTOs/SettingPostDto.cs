using FluentValidation;
using YankiApi.DataAccessLayer;

namespace YankiApi.DTOs.SettingDTOs
{
    /// <summary>
    /// Create Setting
    /// </summary>
    public class SettingPostDto
    {
        /// <summary>
        /// Setting Key
        /// </summary>
        public string? Key { get; set; }
        /// <summary>
        /// Setting Value
        /// </summary>
        public string? Value { get; set; }
    }

    public class SettingPostDtoValidator : AbstractValidator<SettingPostDto> 
    {
        public SettingPostDtoValidator(AppDbContext context)
        {
            RuleFor(r => r.Key)
                .MaximumLength(50).WithMessage("Max 50 simvol")
                .NotEmpty().WithMessage("Mecburidir");
            RuleFor(r => r).Custom(async (r, validate) =>
            {
                if(context.Settings.Any(s => s.Key.ToLower() == r.Key.ToLower()))
                {
                    validate.AddFailure("Eyni Adda Setting Movcuddur");
                }
            });

            RuleFor(r => r.Value)
                .MaximumLength(200).WithMessage("Max 200 simvol")
                .NotEmpty().WithMessage("Mecburidir");
        }
    }
}

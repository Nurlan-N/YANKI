using FluentValidation;
using YankiApi.DataAccessLayer;

namespace YankiApi.DTOs.SettingDTOs
{
    public class SettingUpdateDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public class SettingUpdateDtoValidation : AbstractValidator<SettingUpdateDto>
        {
            public SettingUpdateDtoValidation(AppDbContext context)
            {
                RuleFor(r => r.Key)
                    .MaximumLength(50).WithMessage("Max 50 simvol");
                RuleFor(r => r).Custom(async (r, validate) =>
                {
                    if (context.Settings.Any(s => s.Key.ToLower() == r.Key.ToLower()))
                    {
                        validate.AddFailure("Eyni Adda Setting Movcuddur");
                    }
                });

                RuleFor(r => r.Value)
                    .MaximumLength(200).WithMessage("Max 200 simvol");
            }
        }
    }
}

using AutoMapper;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using YankiApi.DataAccessLayer;
using YankiApi.Entities;

namespace YankiApi.DTOs.SettingDTOs
{
    public class SettingUpdateDto
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
        public class SettingUpdateDtoValidation : AbstractValidator<SettingUpdateDto>
        {
            public SettingUpdateDtoValidation()
            {
                RuleFor(r => r.Value)
                    .MaximumLength(200).WithMessage("Max 200 simvol");
                RuleFor(r => r.Key)
                    .MaximumLength(50).WithMessage("Max 50 simvol");
                
            }
        }
    }
}

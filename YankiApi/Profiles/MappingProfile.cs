﻿using AutoMapper;
using YankiApi.DTOs.ProductDTOs;
using YankiApi.DTOs.SettingDTOs;
using YankiApi.Entities;

namespace YankiApi.Profiles
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<SettingPostDto, Setting>();
            CreateMap<SettingUpdateDto, Setting>();
            CreateMap<Setting, SettingGetDto>();
            CreateMap<ProductPostDto, Product>();
        }
    }
}

using AutoMapper;
using RegLab_Test.Mongodb.UserSettings.Entity;

namespace RegLab_Test.Contracts.MappingProfiles
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<UserSettings, SettingsDTO>()
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings))
                .ReverseMap()
                .ForMember(dest => dest.Settings, opt => opt.MapFrom(src => src.Settings));

            CreateMap<UserSettings.Setting, SettingsDTO.SettingDTO>().ReverseMap();
        }
    }
}


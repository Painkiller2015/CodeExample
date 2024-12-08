using AutoMapper;
using System.Xml;
using RegLab_Test.MongoDB.Entity;

namespace RegLab_Test.Contracts.MappingProfiles
{
    public class SettingsMappingProfile : Profile
    {
        public SettingsMappingProfile()
        {
            CreateMap<UserSettings, SettingsDTO>().ReverseMap();
        }
    }
}


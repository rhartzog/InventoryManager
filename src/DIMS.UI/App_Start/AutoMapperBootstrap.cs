using AutoMapper;
using DIMS.Core.Entities;
using DIMS.UI.Models.Edit;

namespace DIMS.UI
{
    public class AutoMapperBootstrap
    {
        public static void ConfigureMaps()
        {
            Mapper.CreateMap<Box, BoxForm>()
                .ForMember(dest => dest.SelectedCategory, opt => opt.MapFrom(src => src.BoxCategory.DisplayName));
        }
    }
}
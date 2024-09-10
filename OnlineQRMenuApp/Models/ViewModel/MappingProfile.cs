
using AutoMapper;
using OnlineQRMenuApp.Models;
using OnlineQRMenuApp.Models.ViewModel;

namespace OnlineQRMenuApp.Models.ViewModel
{
    public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<MenuItem, MenuItemsModel>()
            .ForMember(dest => dest.CustomizationGroups, opt => opt.MapFrom(src => src.CustomizationGroups));

        CreateMap<CustomizationGroup, CustomizationModelGroup>()
            .ForMember(dest => dest.Customizations, opt => opt.MapFrom(src => src.Customizations));

        CreateMap<MenuItemCustomization, MenuItemCustomizationModel>();
    }
}

}
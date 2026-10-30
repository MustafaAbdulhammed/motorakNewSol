// motorak.bll/Mapper/ServiceMapping/ServiceProfile.cs
using AutoMapper;
using Motorak.BLL.ModelVM.Service;
using Motorak.DAL.Entites;
using Motorak.DAL.Enums.SeviceEnums;

namespace Motorak.BLL.Mapper.ServiceMapping
{
    public class ServiceProfile : Profile
    {
        public ServiceProfile()
        {
            CreateMap<Service, ServiceDTO>()
                .ForMember(dest => dest.ServiceId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.MechanicId, opt => opt.MapFrom(src => src.MechanicId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.ServiceType, opt => opt.MapFrom(src => src.ServiceType))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<CreateServiceVM, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.Now))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.ServiceReviews, opt => opt.Ignore());

            CreateMap<UpdateServiceVM, Service>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ServiceId));
        }
    }
}
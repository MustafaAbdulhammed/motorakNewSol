using AutoMapper;
using motorak.dal.Entites;
using Motorak.BLL.ModelVM.Customer;
using Motorak.BLL.ModelVM.Service;
using Motorak.DAL.Entites;
using System.Collections.Immutable;

namespace Motorak.BLL.Mapper.CustomerMapping
{
    public class CustomerProfile : Profile
    {
        public CustomerProfile()
        {
            CreateMap<Customer, CustomerListModel>()
                .ForMember(c => c.Id, opt => opt.MapFrom(x => x.Id.ToString()))
                .ForMember(c => c.Name, opt => opt.MapFrom(x => x.User.Name))
                .ForMember(c => c.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(c => c.Phone, opt => opt.MapFrom(x => x.User.PhoneNumber))
                .ForMember(c => c.PurchasedCarsCount, opt => opt.MapFrom(x => x.Purchases != null ? x.Purchases.Count : 0))
                .ForMember(c => c.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt))
                .ForMember(c => c.IsDeleted, opt => opt.MapFrom(x => x.IsDeleted))
                .ForMember(c => c.DeletedAt, opt => opt.MapFrom(x => x.DeletedAt))
                .ForMember(c => c.IsUpdated, opt => opt.MapFrom(x => x.IsUpdated))
                .ForMember(c => c.UpdatedAt, opt => opt.MapFrom(x => x.UpdatedAt));

            CreateMap<Customer, CustomerDetailsModel>()
                .ForMember(c => c.Id, opt => opt.MapFrom(x => x.Id))
                .ForMember(c => c.Name, opt => opt.MapFrom(x => x.User.Name))
                .ForMember(c => c.Email, opt => opt.MapFrom(x => x.User.Email))
                .ForMember(c => c.PurchasedCarsCount, opt => opt.MapFrom(x => x.Purchases != null ? x.Purchases.Count : 0))
                .ForMember(c => c.PhoneNumber, opt => opt.MapFrom(x => x.User.PhoneNumber))
                .ForMember(c => c.CreatedAt, opt => opt.MapFrom(x => x.CreatedAt))
                .ForMember(c => c.IsDeleted, opt => opt.MapFrom(x => x.IsDeleted))
                .ForMember(c => c.DeletedAt, opt => opt.MapFrom(x => x.DeletedAt))
                .ForMember(c => c.IsUpdated, opt => opt.MapFrom(x => x.IsUpdated))
                .ForMember(c => c.UpdatedAt, opt => opt.MapFrom(x => x.UpdatedAt));

            CreateMap<CreateCustomerModel, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserId, opt => opt.Ignore())
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.IsDeleted, opt => opt.MapFrom(src => src.IsDeleted))
                .ForMember(dest => dest.DeletedAt, opt => opt.MapFrom(src => src.DeletedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
                .ForMember(dest => dest.IsUpdated, opt => opt.Ignore())
                .ForMember(dest => dest.Purchases, opt => opt.Ignore())
                .ForMember(dest => dest.Rents, opt => opt.Ignore())
                .ForMember(dest => dest.Services, opt => opt.Ignore())
                .ForMember(dest => dest.ServiceReviews, opt => opt.Ignore())
                .ForMember(dest => dest.Cars, opt => opt.Ignore());

            CreateMap<CreateCustomerModel, User>()
                .ForMember(dest => dest.Id, opt => opt.Ignore())
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.NormalizedEmail, opt => opt.MapFrom(src => src.Email.Normalize().ToUpper()))
                .ForMember(dest => dest.NormalizedUserName, opt => opt.MapFrom(src => src.Email.Normalize().ToUpper()))
                .ForMember(dest => dest.ImagePath, opt => opt.Ignore())
                .ForMember(dest => dest.PasswordHash, opt => opt.Ignore());

            CreateMap<EditCustomerModel, Customer>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 

            CreateMap<EditCustomerModel, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber))
                .ForMember(dest => dest.Id, opt => opt.Ignore()); 

        }
    }
}
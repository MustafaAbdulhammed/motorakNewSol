using AutoMapper;
using motorak.dal.Entites;
using Motorak.BLL.ModelVM.Car;

namespace Motorak.BLL.Mapper.CarMapping
{
    public class CarProfile : Profile
    {
        public CarProfile()
        {
            // Car to CarListModel mapping
            CreateMap<Car, CarListModel>();

            // Car to CarDetailsModel mapping
            CreateMap<Car, CarDetailsModel>()
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.OwnerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.User.Name : null));

            // CreateCarModel to Car mapping
            CreateMap<CreateCarModel, Car>()
                .ConstructUsing(c => new Car(
                    c.Brand,
                    c.Model,
                    c.Year,
                    c.Color,
                    c.Mileage,
                    c.Type,
                    c.Transmission,
                    c.Condition,
                    c.Price,
                    c.DailyRentPrice,
                    c.Category,
                    c.ImagePath
                ));

            CreateMap<EditCarModel, Car>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) 
                .AfterMap((src, dest) =>
                {
                    dest.Edit(
                        src.Brand,
                        src.Model,
                        src.Year,
                        src.Color,
                        src.Mileage,
                        src.Type,
                        src.Transmission,
                        src.Condition,
                        src.Status,
                        src.Category,
                        src.Price,
                        src.DailyRentPrice,
                        src.ImagePath
                    );
                    dest.Update(); 
                });

            CreateMap<Car, EditCarModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Model))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Year))
                .ForMember(dest => dest.Color, opt => opt.MapFrom(src => src.Color))
                .ForMember(dest => dest.Mileage, opt => opt.MapFrom(src => src.Mileage))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Transmission, opt => opt.MapFrom(src => src.Transmission))
                .ForMember(dest => dest.Condition, opt => opt.MapFrom(src => src.Condition))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.DailyRentPrice, opt => opt.MapFrom(src => src.DailyRentPrice))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.ImagePath));
        }
    }
}
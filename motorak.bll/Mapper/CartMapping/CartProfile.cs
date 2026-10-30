using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using motorak.bll.ModelVM.ShoppingCart;
using Motorak.DAL.Entites;

namespace motorak.bll.Mapper.CartMapping
{
    public class CartProfile : Profile
    {
        public CartProfile()
        {
            CreateMap<CartItem, ShoppingCartVM>()
                .ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Car.Brand))
                .ForMember(dest => dest.Model, opt => opt.MapFrom(src => src.Car.Model))
                .ForMember(dest => dest.Year, opt => opt.MapFrom(src => src.Car.Year))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Car.Price))
                .ForMember(dest => dest.DailyRentPrice, opt => opt.MapFrom(src => src.Car.DailyRentPrice))
                .ForMember(dest => dest.ImagePath, opt => opt.MapFrom(src => src.Car.ImagePath))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Car.Category))
                .ForMember(dest => dest.Count, opt => opt.MapFrom(src => src.Count))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.ItemType, opt => opt.MapFrom(src => src.ItemType))
                .ForMember(dest => dest.RentStartDate, opt => opt.MapFrom(src => src.RentStartDate))
                .ForMember(dest => dest.RentEndDate, opt => opt.MapFrom(src => src.RentEndDate));
        }
    }
}

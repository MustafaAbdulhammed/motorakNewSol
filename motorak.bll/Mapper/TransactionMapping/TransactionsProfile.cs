using AutoMapper;
using motorak.dal.Entities;
using Motorak.BLL.ModelVM.Purchases;
using Motorak.BLL.ModelVM.Rents;
using Motorak.BLL.ModelVM.Transactions;
using Motorak.DAL.Entities;

namespace Motorak.BLL.Mapper.TransactionMapping
{
    public class TransactionsProfile : Profile
    {
        public TransactionsProfile()
        {
            // Transaction mappings
            CreateMap<Transactions, TransactionCreateDto>();
            //CreateMap<TransactionCreateDto, Transactions>();
            CreateMap<Transactions, TransactionUpdateDto>();
            //CreateMap<TransactionUpdateDto, Transactions>();
            CreateMap<Transactions, TransactionReadDto>();

            //// Rent mappings - EXPLICIT mapping for all properties
            //CreateMap<Rent, RentReadDto>()
            //    .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            //    .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
            //    .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
            //    .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
            //    .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
            //    .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            //    .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
            //    .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
            //    .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));

            //CreateMap<RentCreateDto, Rent>()
            //    .ForMember(dest => dest.Id, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.CreatedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.UpdatedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.IsDeleted, opt => opt.Ignore())
            //    .ForMember(dest => dest.DeletedAt, opt => opt.Ignore())
            //    .ForMember(dest => dest.DeletedBy, opt => opt.Ignore())
            //    .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => DateTime.Now));

            // ── RENT MAPPINGS (CORRECTED) ─────────────────────────────────────

            // Read: Entity → ReadDto (used in GetAllAsync, GetByIdAsync)
            CreateMap<Rent, RentReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.TransactionDate, opt => opt.MapFrom(src => src.TransactionDate));

            // Read: Entity → UpdateDto (used in RentController.Edit GET)
            CreateMap<Rent, RentUpdateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // ⚠️ NO MAPPING FROM RentCreateDto OR RentUpdateDto TO Rent
            // Reason: Rent has private constructor and private setters.
            // Creation is done manually in RentService.CreateAsync using new Rent(...)
            // Update is done manually in RentService.UpdateAsync using existing.UpdateTransaction(...)

            // SAFEGUARD: If anyone accidentally tries to map CreateDto → Rent, throw clear error.
            CreateMap<RentCreateDto, Rent>().ConvertUsing((src, dest, ctx) =>
            {
                throw new InvalidOperationException(
                    "Do not map RentCreateDto -> Rent via AutoMapper. " +
                    "Use the Rent constructor directly: new Rent(startDate, endDate, paymentMethod, totalPrice, customerId, carId)");
            });

            // SAFEGUARD: If anyone accidentally tries to map UpdateDto → Rent, throw clear error.
            CreateMap<RentUpdateDto, Rent>().ConvertUsing((src, dest, ctx) =>
            {
                throw new InvalidOperationException(
                    "Do not map RentUpdateDto -> Rent via AutoMapper. " +
                    "Use the UpdateTransaction method on the existing entity.");
            });

            // ── PURCHASE MAPPINGS (CORRECTED) ─────────────────────────────

            // Read: Entity → DTO (used in GetAllAsync, GetByIdAsync)
            CreateMap<Purchase, PurchaseReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.SellerName))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));
            // Optional: add CustomerId, CarId if needed in DTO

            // Update GET: Entity → UpdateDto (used in PurchaseController.Edit GET)
            CreateMap<Purchase, PurchaseUpdateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // Optional: Entity → CreateDto (if ever needed, but not used currently)
            CreateMap<Purchase, PurchaseCreateDto>()
                .ForMember(dest => dest.SellerName, opt => opt.MapFrom(src => src.SellerName))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CarId, opt => opt.MapFrom(src => src.CarId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            // ⚠️ NO MAPPING FROM PurchaseCreateDto OR PurchaseUpdateDto TO Purchase
            // Reason: Purchase has private constructor and private setters.
            // Creation is done manually in PurchaseService.CreateAsync using new Purchase(...)
            // Update is done manually in PurchaseService.UpdateAsync using existing.UpdateTransaction(...)

            // SAFEGUARD: If anyone accidentally tries to map UpdateDto → Purchase, throw clear error.
            CreateMap<PurchaseUpdateDto, Purchase>().ConvertUsing((src, dest, ctx) =>
            {
                throw new InvalidOperationException(
                    "Do not map PurchaseUpdateDto -> Purchase via AutoMapper. " +
                    "Use the domain method UpdateTransaction on the existing entity.");
            });
        }
    }
}
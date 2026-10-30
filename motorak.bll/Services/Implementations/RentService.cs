using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Motorak.BLL.ModelVM.Rents;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAl.Repo.Abstractions;
using Motorak.DAL.Entities;
using Motorak.DAL.Repo.Abstractions;
using System.Security.Claims;

namespace Motorak.BLL.Services.Implementations
{
    public class RentService : IRentService
    {
        private readonly IRentRepo _repo;
        private readonly ICarRebo _carRepo;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RentService(IRentRepo repo, ICarRebo carRepo, IMapper mapper, IHttpContextAccessor httpContextAccessor)
        {
            _repo = repo;
            _carRepo = carRepo;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
        }

        private string GetCurrentUser()
        {
            return _httpContextAccessor.HttpContext?.User?.Identity?.Name ?? "System";
        }

        private string GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public async Task<List<RentReadDto>> GetAllAsync()
        {
            var entities = await _repo.GetAllAsync();
            return _mapper.Map<List<RentReadDto>>(entities);
        }

        public async Task<RentReadDto?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<RentReadDto>(entity);
        }

        public async Task<int> CreateAsync(RentCreateDto dto)
        {
            // Validate car existence
            var car = await _carRepo.GetByIdAsync(dto.CarId);
            if (car == null)
                throw new InvalidOperationException("Car not found.");

            // Validate dates
            if (dto.StartDate >= dto.EndDate)
                throw new InvalidOperationException("End date must be after start date.");

            if (dto.StartDate < DateTime.Today)
                throw new InvalidOperationException("Start date cannot be in the past.");

            // Check availability using repository
            bool isAvailable = await _repo.IsCarAvailableForDates(dto.CarId, dto.StartDate, dto.EndDate);
            if (!isAvailable)
                throw new InvalidOperationException("Car is not available for the selected dates.");

            // Create rent WITHOUT customer validation here
            // The ownership check should be done in the controller
            var rent = new Rent(dto.StartDate, dto.EndDate, dto.PaymentMethod, dto.TotalPrice, dto.CustomerId, dto.CarId);

            // Set audit field - simpler approach
            rent.CreatedBy = GetCurrentUser();

            await _repo.AddAsync(rent);
            return rent.Id;
        }

        public async Task UpdateAsync(RentUpdateDto dto)
        {
            var existing = await _repo.GetByIdAsync(dto.Id);
            if (existing == null)
                throw new InvalidOperationException("Rent not found.");

            // Only admin can update
            //if (!IsAdmin())
            //    throw new UnauthorizedAccessException("Only administrators can update rentals.");

            existing.UpdateTransaction(dto.PaymentMethod, dto.TotalPrice, dto.Status, GetCurrentUser());

            await _repo.UpdateAsync(existing);
        }

        public async Task DeleteAsync(int id)
        {
            var existing = await _repo.GetByIdAsync(id);
            if (existing == null)
                throw new InvalidOperationException("Rent not found.");

            //if (!IsAdmin())
            //    throw new UnauthorizedAccessException("Only administrators can delete rentals.");

            await _repo.DeleteAsync(id);
        }

        private bool IsAdmin()
        {
            return _httpContextAccessor.HttpContext?.User?.IsInRole("Admin") ?? false;
        }
    }
}
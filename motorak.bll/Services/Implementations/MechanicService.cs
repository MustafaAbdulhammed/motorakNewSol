using AutoMapper;
using Microsoft.AspNetCore.Identity;
using motorak.dal.Entites;
using Motorak.BLL.ModelVM.Mechanic;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAL.Entites;
using Motorak.DAL.Repo.Abstractions;
using Motorak.Utility;

namespace Motorak.BLL.Services.Implementations
{
    public class MechanicService : IMechanicService
    {
        private readonly IMechanicRebo _mechanicRepo;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;

        public MechanicService(
            IMechanicRebo mechanicRepo,
            UserManager<User> userManager,
            IMapper mapper)
        {
            _mechanicRepo = mechanicRepo;
            _userManager = userManager;
            _mapper = mapper;
        }

        public async Task<(bool status, string message)> CreateMechanicAsync(CreateMechanicModel model)
        {
            try
            {
                var existingUser = await _userManager.FindByEmailAsync(model.Email);
                if (existingUser != null)
                {
                    return (false, "A user with this email already exists.");
                }

                var user = _mapper.Map<User>(model);
                user.EmailConfirmed = true;

                var result = await _userManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return (false, $"Failed to create user: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, Seed.Role_Mechanic);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var roleErrors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to assign mechanic role: {roleErrors}");
                }

                var mechanic = _mapper.Map<Mechanic>(model);
                mechanic.UserId = user.Id;

                await _mechanicRepo.AddAsync(mechanic);
                await _mechanicRepo.SaveChangesAsync();

                return (true, "Mechanic created successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool status, string message, List<MechanicListModel>? mechanics)> GetAllMechanicsAsync()
        {
            try
            {
                var mechanics = await _mechanicRepo.GetAllAsync();
                var mechanicModels = _mapper.Map<List<MechanicListModel>>(mechanics);
                return (true, "Mechanics retrieved successfully.", mechanicModels);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message, MechanicDetailsModel? mechanic)> GetMechanicByIdAsync(int id)
        {
            try
            {
                var mechanic = await _mechanicRepo.GetByIdAsync(id);
                if (mechanic == null)
                {
                    return (false, "Mechanic not found.", null);
                }

                var mechanicModel = _mapper.Map<MechanicDetailsModel>(mechanic);
                return (true, "Mechanic retrieved successfully.", mechanicModel);
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message)> EditMechanicAsync(EditMechanicModel model)
        {
            try
            {
                var mechanic = await _mechanicRepo.GetByIdAsync(model.Id);
                if (mechanic == null)
                {
                    return (false, "Mechanic not found.");
                }

                if (mechanic.IsDeleted)
                {
                    return (false, "Cannot edit a deleted mechanic.");
                }

                mechanic.UpdateMechanicInfo(model.Name, model.WorkHours, model.Status);

                if (!string.IsNullOrEmpty(model.PhoneNumber))
                {
                    mechanic.User.PhoneNumber = model.PhoneNumber;
                }

                if (!string.IsNullOrEmpty(model.ImagePath))
                {
                    mechanic.User.ImagePath = model.ImagePath;
                }

                mechanic.IsUpdated = true;
                mechanic.UpdatedAt = DateTime.Now;

                _mechanicRepo.Update(mechanic);
                await _mechanicRepo.SaveChangesAsync();

                return (true, "Mechanic updated successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> DeleteMechanicAsync(int id)
        {
            try
            {
                var mechanic = await _mechanicRepo.GetByIdAsync(id);
                if (mechanic == null)
                {
                    return (false, "Mechanic not found.");
                }

                if (mechanic.IsDeleted)
                {
                    return (false, "Mechanic is already deleted.");
                }

                _mechanicRepo.Delete(mechanic);
                await _mechanicRepo.SaveChangesAsync();

                return (true, "Mechanic deleted successfully.");
            }
            catch (Exception ex)
            {
                return (false, $"An error occurred: {ex.Message}");
            }
        }
    }
}
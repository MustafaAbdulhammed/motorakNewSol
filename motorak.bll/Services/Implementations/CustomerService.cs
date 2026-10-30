using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using motorak.dal.Entites;
using Motorak.BLL.ModelVM.Customer;
using Motorak.BLL.ModelVM.Service;
using Motorak.BLL.Services.Abstractions;
using Motorak.DAl.Repo.Abstractions;
using Motorak.DAl.Repo.Implementations;
using Motorak.DAL.Entites;

namespace Motorak.BLL.Services.Implementations
{
    using Microsoft.AspNetCore.Identity;
    using Motorak.Utility;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerRebo _customerRepo;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly UserManager<User> _userManager;

        public CustomerService(ICustomerRebo customerRepo, IMapper mapper, IPasswordHasher<User> passwordHasher,UserManager<User> userManager)
        {
            _customerRepo = customerRepo;
            _mapper = mapper;
            _passwordHasher = passwordHasher;
            _userManager = userManager;
        }
   

        public async Task<(bool status, string message, List<CustomerListModel>)> GetAllCustomersAsync()
        {
            try
            {
                var result = await _customerRepo.GetAllAsync();
                var resultList = _mapper.Map<List<CustomerListModel>>(result);
                return (true, "Customers retrieved successfully!", resultList);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to retrieve customers: {ex.Message}", new List<CustomerListModel>());
            }
        }

        public async Task<(bool status, string message, CustomerDetailsModel?)> GetCustomerByIdAsync(int id)
        {
            try
            {
                var result = _customerRepo.GetByIdAsync(id);
                if (result == null)
                {
                    return (false, "Customer not found", null);
                }

                return (true, "Customer found", _mapper.Map<CustomerDetailsModel>(await result));
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message, CustomerDetailsModel?)> GetCustomerByUsernameAsync(string username)
        {
            try
            {
                var customer = await _customerRepo.GetByUsernameAsync(username);
                if (customer == null) return (false, "Customer Not Found!!", null);

                var result = _mapper.Map<CustomerDetailsModel>(customer);
                return (true, "Customer retrieved successfully!", result);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to retrieve customer: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message)> CreateCustomerAsync(CreateCustomerModel customerModel)
        {
            try
            {
                if (customerModel == null)
                    return (false, "Customer model is null");

                // Check if email already exists
                var existingCustomer = await _userManager.FindByEmailAsync(customerModel.Email);
                if (existingCustomer != null)
                    return (false, "Email already exists");

                // Check if phone number already exists
                var existingPhone = await _customerRepo.GetByPhoneAsync(customerModel.PhoneNumber);
                if (existingPhone != null)
                    return (false, "Phone number already exists");

                var user = new User
                {
                    UserName = customerModel.Email,
                    Email = customerModel.Email,
                    PhoneNumber = customerModel.PhoneNumber,
                    Name = customerModel.Name,
                    EmailConfirmed = true,
                    CreatedAt = DateTime.Now
                };

                var createUserResult = await _userManager.CreateAsync(user, customerModel.Password);
                if (!createUserResult.Succeeded)
                {
                    var errors = string.Join(", ", createUserResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to create user: {errors}");
                }

                var roleResult = await _userManager.AddToRoleAsync(user, Seed.Role_Customer);
                if (!roleResult.Succeeded)
                {
                    await _userManager.DeleteAsync(user);
                    var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    return (false, $"Failed to assign customer role: {errors}");
                }

                var customer = new Customer
                {
                    UserId = user.Id,
                    User = user,
                    CreatedAt = DateTime.Now,
                };

                await _customerRepo.CreateAsync(customer);
                await _customerRepo.SaveChangesAsync();

                return (true, "Customer Created Successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Error Occurred: {ex.Message}");
            }
        }

        public async Task<(bool status, string message)> EditCustomerAsync(EditCustomerModel customerModel)
        {
            try
            {
                var result = await _customerRepo.GetByIdAsync(customerModel.Id);
                if (result == null)
                    return (false, "Customer not found");

                _mapper.Map(customerModel, result);

                if (result.User != null)
                    _mapper.Map(customerModel, result.User);

                result.UpdateCustomerInfo(customerModel.Name, customerModel.PhoneNumber);

                _customerRepo.Update(result);
                await _customerRepo.SaveChangesAsync();

                return (true, "Customer updated successfully");
            }
            catch (Exception ex)
            {
                return (false, $"Error Occurred: {ex.Message}");
            }
        }



        public async Task<(bool status, string message)> DeleteCustomerAsync(int id)
        {
            try
            {
                var result = await _customerRepo.GetByIdAsync(id);
                if (result == null) return (false, "Customer Not Found!!");

                _customerRepo.Delete(result);
                await _customerRepo.SaveChangesAsync();
                return (true, "Customer Deleted Successfully!");
            }
            catch (Exception ex)
            {
                return (false, $"Failed to delete: {ex.Message}");
            }
        }

        public async Task<(bool status, string message, CustomerDetailsModel?)> IsEmailExistsAsync(string email)
        {
            try
            {
                var customer = await _customerRepo.GetByEmailAsync(email);
                if (customer == null) return (false, "Customer Not Found!!", null);

                var result = _mapper.Map<CustomerDetailsModel>(customer);
                return (true, "Customer retrieved successfully!", result);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to find Email: {ex.Message}", null);
            }
        }

        public async Task<(bool status, string message, CustomerDetailsModel?)> IsPhoneNumberExistsAsync(string phoneNumber)
        {
            try
            {
                var customer = await _customerRepo.GetByPhoneAsync(phoneNumber);
                if (customer == null) return (false, "Phone Number Not Found!!", null);

                var result = _mapper.Map<CustomerDetailsModel>(customer);
                return (true, "Customer retrieved successfully!", result);
            }
            catch (Exception ex)
            {
                return (false, $"Failed to find Phone Number: {ex.Message}", null);
            }
        }
    }
}
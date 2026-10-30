// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using motorak.dal.DataTemp;
using motorak.dal.Entites;
using motorak.DAL.DataBase;
using Motorak.BLL.ModelVM.Customer;
using Motorak.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading;
using System.Threading.Tasks;
namespace motorak.pll.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<User> _userStore;
        private readonly IUserEmailStore<User> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSender _emailSender;
        private readonly MotorakDbContext _context;

        public RegisterModel(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            IUserStore<User> userStore,
            SignInManager<User> signInManager,
            ILogger<RegisterModel> logger,
            IEmailSender emailSender,
            MotorakDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _context = context;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Phone Number")]
            [DataType(DataType.PhoneNumber)]
            public string PhoneNumber { get; set; }

            public string? Role { get; set; }

            [ValidateNever]
            public IEnumerable<SelectListItem> RoleList { get; set; }

            [Display(Name = "Working Hours")]
            public string? WorkHours { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            // Create roles if they don't exist
            if (!_roleManager.RoleExistsAsync(Seed.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(Seed.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Seed.Role_Mechanic)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(Seed.Role_Admin)).GetAwaiter().GetResult();
            }

            Input = new()
            {
                RoleList = _roleManager.Roles.Where(r => r.Name != Seed.Role_Admin).Select(x => x.Name).Select(i => new SelectListItem
                {
                    Text = i,
                    Value = i
                })
            };
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }

        //public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        //{
        //    returnUrl ??= Url.Content("~/");
        //    ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

        //    // Recreate RoleList in case of validation errors
        //    Input.RoleList = _roleManager.Roles.Where(r => r.Name != Seed.Role_Admin).Select(x => x.Name).Select(i => new SelectListItem
        //    {
        //        Text = i,
        //        Value = i
        //    });

        //    // Custom validation for mechanic
        //    if (Input.Role == Seed.Role_Mechanic && string.IsNullOrEmpty(Input.WorkHours))
        //    {
        //        ModelState.AddModelError("Input.WorkHours", "Working hours are required for mechanics.");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        using var transaction = await _context.Database.BeginTransactionAsync();
        //        try
        //        {
        //            var user = CreateUser();
        //            user.Name = Input.FullName;
        //            user.PhoneNumber = Input.PhoneNumber;
        //            user.CreatedAt = DateTime.Now;

        //            await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
        //            await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

        //            var result = await _userManager.CreateAsync(user, Input.Password);

        //            if (result.Succeeded)
        //            {
        //                _logger.LogInformation($"User created successfully with ID: {user.Id}");

        //                // Add role
        //                string roleToAssign = !string.IsNullOrEmpty(Input.Role) ? Input.Role : Seed.Role_Customer;
        //                var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

        //                if (!roleResult.Succeeded)
        //                {
        //                    _logger.LogError($"Failed to assign role {roleToAssign} to user {user.Id}");
        //                    // Don't fail the entire process for role assignment issues
        //                }

        //                // Create specific entity based on role
        //                if (Input.Role == "Customer")
        //                {
        //                    var customer = new Customer
        //                    {
        //                        UserId = user.Id,
        //                        User = user,
        //                        CreatedAt = DateTime.Now
        //                    };

        //                    _context.Customers.Add(customer);
        //                }
        //                else if (Input.Role == "Mechanic")
        //                {
        //                    var mechanic = new Mechanic
        //                    {
        //                        UserId = user.Id,
        //                        User = user,
        //                        WorkHours = Input.WorkHours ?? "1"
        //                    };
        //                    _context.Mechanics.Add(mechanic);
        //                }

        //                // Save changes first
        //                await _context.SaveChangesAsync();

        //                // Commit transaction before sending email
        //                await transaction.CommitAsync();

        //                _logger.LogInformation($"User {user.Id} and related entities saved successfully");

        //                // Generate confirmation email AFTER committing the transaction
        //                var userId = await _userManager.GetUserIdAsync(user);
        //                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        //                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));

        //                var callbackUrl = Url.Page(
        //                    "/Account/ConfirmEmail",
        //                    pageHandler: null,
        //                    values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
        //                    protocol: Request.Scheme);

        //                try
        //                {
        //                    var emailBody = EmailTemplate.GetEmailConfirmationTemplate(callbackUrl);
        //                    await _emailSender.SendEmailAsync(Input.Email, "Confirm your Motorak account", emailBody);
        //                    _logger.LogInformation($"Confirmation email sent to {Input.Email}");
        //                }
        //                catch (Exception emailEx)
        //                {
        //                    _logger.LogError(emailEx, $"Failed to send confirmation email to {Input.Email}");
        //                    // Don't fail the registration if email sending fails
        //                }

        //                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
        //            }
        //            else
        //            {
        //                await transaction.RollbackAsync();
        //                foreach (var error in result.Errors)
        //                {
        //                    _logger.LogError($"User creation error: {error.Description}");
        //                    ModelState.AddModelError(string.Empty, error.Description);
        //                }
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            await transaction.RollbackAsync();
        //            _logger.LogError(ex, "Error occurred during user registration");
        //            ModelState.AddModelError(string.Empty, "An error occurred during registration. Please try again.");
        //        }
        //    }

        //    return Page();
        //}
        //simpler version without email verfication 
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            // Recreate RoleList in case of validation errors
            Input.RoleList = _roleManager.Roles.Where(r => r.Name != Seed.Role_Admin).Select(x => x.Name).Select(i => new SelectListItem
            {
                Text = i,
                Value = i
            });

            // Custom validation for mechanic
            if (Input.Role == Seed.Role_Mechanic && string.IsNullOrEmpty(Input.WorkHours))
            {
                ModelState.AddModelError("Input.WorkHours", "Working hours are required for mechanics.");
            }

            if (ModelState.IsValid)
            {
                var user = CreateUser();
                user.Name = Input.FullName;
                user.PhoneNumber = Input.PhoneNumber;
                user.CreatedAt = DateTime.Now;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    _logger.LogInformation($"User created successfully with ID: {user.Id}");

                    // Add role
                    string roleToAssign = !string.IsNullOrEmpty(Input.Role) ? Input.Role : Seed.Role_Customer;
                    var roleResult = await _userManager.AddToRoleAsync(user, roleToAssign);

                    if (!roleResult.Succeeded)
                    {
                        _logger.LogError($"Failed to assign role {roleToAssign} to user {user.Id}");
                    }

                    // Create specific entity based on role
                    if (Input.Role == "Customer")
                    {
                        var customer = new Customer
                        {
                            UserId = user.Id,
                            User = user,
                            CreatedAt = DateTime.Now
                        };
                        _context.Customers.Add(customer);
                    }
                    else if (Input.Role == "Mechanic")
                    {
                        var mechanic = new Mechanic
                        {
                            UserId = user.Id,
                            User = user,
                            WorkHours = Input.WorkHours ?? "1"
                        };
                        _context.Mechanics.Add(mechanic);
                    }

                    await _context.SaveChangesAsync();

                    // SIGN IN IMMEDIATELY - NO EMAIL CONFIRMATION NEEDED
                    await _signInManager.SignInAsync(user, isPersistent: false);

                    return LocalRedirect(returnUrl);
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        _logger.LogError($"User creation error: {error.Description}");
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }

            return Page();
        }

        private User CreateUser()
        {
            try
            {
                return Activator.CreateInstance<User>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(User)}'. " +
                    $"Ensure that '{nameof(User)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<User> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<User>)_userStore;
        }
    }
}
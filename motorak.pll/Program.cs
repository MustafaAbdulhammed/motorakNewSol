using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.EntityFrameworkCore;
using motorak.bll.Mapper.CartMapping;
using motorak.bll.Services.Abstractions;
using motorak.bll.Services.Implementations;
using motorak.dal.DataTemp;
using motorak.dal.Entites;
using motorak.dal.Repo.Abstractions;
using motorak.dal.Repo.Implementations;
using motorak.DAL.DataBase;
using motorak.pll.Language;
using Motorak.BLL.Mapper.CarMapping;
using Motorak.BLL.Mapper.CustomerMapping;
using Motorak.BLL.Mapper.MechanicMappin;
using Motorak.BLL.Mapper.ServiceMapping;
using Motorak.BLL.Mapper.ServiceReviewMapping;
using Motorak.BLL.Mapper.TransactionMapping;
using Motorak.BLL.Services;
using Motorak.BLL.Services.Abstractions;
using Motorak.BLL.Services.Implementations;
using Motorak.DAl.Repo.Abstractions;
using Motorak.DAl.Repo.Implementations;
using Motorak.DAL.Entites;
using Motorak.DAL.Repo.Abstractions;
using Motorak.DAL.Repo.Implementations;
using Motorak.Utility;
using System.Globalization;

namespace motorak.pll
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("defaultConnection");
            builder.Services.AddDbContext<MotorakDbContext>(options =>
                options.UseSqlServer(connectionString));

            

            builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.SignIn.RequireConfirmedAccount = false;
                options.SignIn.RequireConfirmedEmail = false;
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
                options.Password.RequiredUniqueChars = 1;

                options.User.RequireUniqueEmail = true;

                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(30);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<MotorakDbContext>()
            .AddDefaultTokenProviders()
            .AddDefaultUI();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(2);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });
            builder.Services.AddHttpContextAccessor();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Identity/Account/Login";
                options.LogoutPath = "/Identity/Account/Logout";
                options.AccessDeniedPath = "/Identity/Account/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromHours(12);
                options.SlidingExpiration = true;
                options.Cookie.HttpOnly = true;
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
            });

            builder.Services.AddHttpContextAccessor();
            builder.Services.AddRazorPages();

            //emailsender
            builder.Services.AddTransient<IEmailSender, EmailSender>();

            // Add services to the container.
            builder.Services.AddControllersWithViews().AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix)
            .AddDataAnnotationsLocalization(options =>
            {
                options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(SharedResource));
            }); ;

            //Repos
            builder.Services.AddScoped<ICarRebo, CarRebo>();
            builder.Services.AddScoped<ICustomerRebo, CustomerRebo>();
            builder.Services.AddScoped<IMechanicRebo, MechanicRebo>();
            builder.Services.AddScoped<ITransactionRepo, TransactionRepo>();
            builder.Services.AddScoped<IPurchaseRepo, PurchaseRepo>();
            builder.Services.AddScoped<IRentRepo, RentRepo>();
            builder.Services.AddScoped<IServiceReviewRepo, ServiceReviewRepo>();
            builder.Services.AddScoped<IServiceRepo, ServiceRepo>();
            builder.Services.AddScoped<ICartRepo,CartRepo>();

            //Services
            builder.Services.AddScoped<ICarServicecs, CarService>();
            builder.Services.AddScoped<ICustomerService, CustomerService>();
            builder.Services.AddScoped<IMechanicService, MechanicService>();
            builder.Services.AddScoped<ITransactionService, TransactionService>();
            builder.Services.AddScoped<IPurchaseService, PurchaseService>();
            builder.Services.AddScoped<IRentService, RentService>();
            builder.Services.AddScoped<IServiceReviewService, ServiceReviewService>();
            builder.Services.AddScoped<IServiceServicecs, ServiceServices>();
            builder.Services.AddScoped<ICartService, CartService>();
            builder.Services.AddScoped<IReminderService, ReminderService>();

            builder.Services.AddHangfire(x => x.UseSqlServerStorage(connectionString));
            builder.Services.AddHangfireServer();


            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile(new CarProfile());
                cfg.AddProfile(new CustomerProfile());
                cfg.AddProfile(new MechanicProfile());
                cfg.AddProfile(new ServiceProfile());
                cfg.AddProfile(new ServiceReviewProfile());
                cfg.AddProfile(new TransactionsProfile());
                cfg.AddProfile(new CartProfile());
            });

            builder.Services.AddAuthentication()
            .AddGoogle(options =>
            {
                options.ClientId = "367275960270-k61vvo1n6opfak2b94km9ki769d23o90.apps.googleusercontent.com";
                options.ClientSecret = "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";
            });
            builder.Services.AddAuthentication()
            .AddFacebook(options =>
            {
                options.AppId = "866662105870365";
                options.AppSecret = "1273d4869397bd9f897cc371af7cc752";
            });



            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<MotorakDbContext>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                    var userManager = services.GetRequiredService<UserManager<User>>();

                    await context.Database.EnsureCreatedAsync();

                    await SeedRoles(roleManager);

                    await AdminLogins.SeedAdminAsync(userManager, roleManager);
                    

                    //await DbSeeder.SeedAsync(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            var supportedCultures = new[] {
                      new CultureInfo("ar-EG"),
                      new CultureInfo("en-US"),
                };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en-US"),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures,
                RequestCultureProviders = new List<IRequestCultureProvider>
                {
                new QueryStringRequestCultureProvider(),
                new CookieRequestCultureProvider()
                }
            });

            

            app.UseHangfireDashboard("/Mustafa");
            RecurringJob.AddOrUpdate<ReminderService>(
                            x => x.ShowReminder(),
                            Cron.Minutely);
            app.Run();
        }

        private static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            string[] roles = { Seed.Role_Admin, Seed.Role_Customer, Seed.Role_Mechanic };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}

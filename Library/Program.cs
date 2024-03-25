using Boutique.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;


namespace Boutique
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure services
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlite("Data Source=Flowers.db");
                });
            builder.Services.AddIdentity<IdentityUser, IdentityRole>()
                        .AddDefaultTokenProviders().AddDefaultUI()
                        .AddEntityFrameworkStores<ApplicationDbContext>();

            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(Options =>
            {
                Options.IdleTimeout = TimeSpan.FromMinutes(10);
                Options.Cookie.HttpOnly = true;
                Options.Cookie.IsEssential = true;
            });
            // Build the app
            var app = builder.Build();
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                try
                {
                    var userManager = services.GetRequiredService<UserManager<IdentityUser>>();
                    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                    Task.WaitAll(new Task[] {
                        CreateAdminRole(roleManager),
                        CreateAdminUser(userManager)
                    });
                }
                catch (Exception ex)
                {
                    // Log the error
                }
            }
            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
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

            app.Run();
        }
        private static async Task CreateAdminRole(RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(ShopConstants.Admin))
            {
                await roleManager.CreateAsync(new IdentityRole(ShopConstants.Admin));
            }
        }

        private static async Task CreateAdminUser(UserManager<IdentityUser> userManager)
        {
            string adminEmail = ShopConstants.AdminEmail;
            string adminPassword = ShopConstants.AdminPassword;

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdminUser = new IdentityUser { UserName = adminEmail, Email = adminEmail };
                await userManager.CreateAsync(newAdminUser, adminPassword);
                await userManager.AddToRoleAsync(newAdminUser, ShopConstants.Admin);
            }
        }
    }
}

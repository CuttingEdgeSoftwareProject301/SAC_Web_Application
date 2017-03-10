using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SAC_Web_Application.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SAC_Web_Application.Data
{
    public static class RolesData
    {
        public static async Task SeedRoles(IServiceProvider serviceProvider)
        {
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

                // create user roles

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                if(!await roleManager.RoleExistsAsync("Member"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Member"));
                }

                if (!await roleManager.RoleExistsAsync("RegisteredUser"))
                {
                    await roleManager.CreateAsync(new IdentityRole("RegisteredUser"));
                }

                ApplicationUser user = await userManager.FindByEmailAsync("paull1068@gmail.com");
                if (user != null)
                {
                    await userManager.AddToRolesAsync(user, new string[] { "Admin" });
                }
        }
    }
}

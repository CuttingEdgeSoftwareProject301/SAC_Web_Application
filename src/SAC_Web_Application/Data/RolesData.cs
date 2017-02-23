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

                if (!await roleManager.RoleExistsAsync("Coach"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Coach"));
                }
                // assign roles to users

                ApplicationUser user1 = await userManager.FindByEmailAsync("Dave1633@live.com");
                if (user1 != null)
                {
                    await userManager.AddToRolesAsync(user1, new string[] { "Admin" });
                }

                ApplicationUser user2 = await userManager.FindByEmailAsync("paull1068@gmail.com");
                if (user2 != null)
                {
                    await userManager.AddToRolesAsync(user2, new string[] { "Admin" });
                }
        }
    }
}

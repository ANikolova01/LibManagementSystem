using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using LibraryManagementSystem.Models;
using LibraryManagementSystem.Models.Enums;

namespace LibraryManagementSystem.Data
{


            public static class ContextSeed
            {
                public static async Task SeedRolesAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
                {
                    //Seed Roles
                    await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                    await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                    await roleManager.CreateAsync(new IdentityRole(Roles.Moderator.ToString()));
                    await roleManager.CreateAsync(new IdentityRole(Roles.Basic.ToString()));
                }

            public static async Task SeedSuperAdminAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
            {
                //Seed Default User
                var defaultUser = new ApplicationUser
                {
                    UserName = "superadmin",
                    Email = "superadmin@gmail.com",
                    FirstName = "Jane",
                    LastName = "Goodwyn",
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true
                };
                if (userManager.Users.All(u => u.Id != defaultUser.Id))
                {
                    var user = await userManager.FindByEmailAsync(defaultUser.Email);
                    if (user == null)
                    {
                        await userManager.CreateAsync(defaultUser, "123Pa$$word.");
                        await userManager.AddToRoleAsync(defaultUser, Roles.Basic.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.Moderator.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
                        await userManager.AddToRoleAsync(defaultUser, Roles.SuperAdmin.ToString());
                    }

                }
            }
    }

    }

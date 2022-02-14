using AtlasBlog.Data;
using AtlasBlog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AtlasBlog.Services
{
    public class DataService
    {
        //Calling a method or an instruction that executes the migration
        readonly ApplicationDbContext _dbContext;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<BlogUser> _userManager;

        public DataService(ApplicationDbContext dbContext,
                           RoleManager<IdentityRole> roleManager,
                           UserManager<BlogUser> userManager)
        {
            _dbContext = dbContext;
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SetupDbAsync()
        {
            //Run the Migrations async
            await _dbContext.Database.MigrateAsync();

            //Add a few Roles into the system
            await SeedRolesAsync();

            //Add a few Roles 
            await SeedUsersAsync();

        }




        private async Task SeedRolesAsync()
        {
            //Alternate
            //if (_roleManager.Roles.Any()) {return;} // if there are any roles, dont do anything
            if (_roleManager.Roles.Count() == 0)
            {
                await _roleManager.CreateAsync(new IdentityRole("Administrator"));
                await _roleManager.CreateAsync(new IdentityRole("Moderator"));
            }

        }

        private async Task SeedUsersAsync()
        {
            try
            {
                BlogUser user = new()
                {
                    UserName = "andrew.r.tudor@outlook.com",
                    Email = "andrew.r.tudor@outlook.com",
                    FirstName = "Andrew",
                    LastName = "Tudor",
                    DisplayName = "ART",
                    PhoneNumber = "3369443910",
                    EmailConfirmed = true
                };

                var newUser = await _userManager.FindByEmailAsync(user.Email);
                if (newUser is null)
                {
                    await _userManager.CreateAsync(user, "Abc&123!");
                    await _userManager.AddToRoleAsync(user, "Administrator");
                }

                user = new()
                {
                    UserName = "moderator@coderfoundry.com",
                    Email = "moderator@coderfoundry.com",
                    FirstName = "Coder",
                    LastName = "Foundry",
                    DisplayName = "CF Mod",
                    PhoneNumber = "5555555555",
                    EmailConfirmed = true
                };

                newUser = await _userManager.FindByEmailAsync(user.Email);
                if (newUser is null)
                {
                    await _userManager.CreateAsync(user, "Abc&123!");
                    await _userManager.AddToRoleAsync(user, "Moderator");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        public DateTime GetPostGresDate(DateTime datetime)
        {
            return DateTime.SpecifyKind(datetime, DateTimeKind.Utc);
        }
    }
}

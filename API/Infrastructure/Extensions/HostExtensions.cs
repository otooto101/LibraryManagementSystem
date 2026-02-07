using Microsoft.AspNetCore.Identity;
using Persistence;
using Persistence.Entities;

namespace API.Infrastructure.Extensions
{
    public static class HostExtensions
    {
        public static async Task SeedDatabaseAsync(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;

            try
            {
                var context = services.GetRequiredService<LibraryContext>();

                var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

                await LibraryContextSeed.SeedAsync(context, userManager, roleManager);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
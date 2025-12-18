// Labs.UI/Data/DbInit.cs - ОБНОВЛЕННЫЙ ФАЙЛ
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Labs.UI.Data
{
    public class DbInit
    {
        public static async Task SetupIdentityAdmin(UserManager<AppUser> userManager)
        {
            var user = await userManager.FindByEmailAsync("admin@gmail.com");
            if (user == null)
            {
                user = new AppUser
                {
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "123456");
                if (result.Succeeded)
                {
                    var claim = new Claim(ClaimTypes.Role, "admin");
                    await userManager.AddClaimAsync(user, claim);
                }
            }
        }
    }
}

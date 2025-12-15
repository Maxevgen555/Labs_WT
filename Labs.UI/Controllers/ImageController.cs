using Labs.UI.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Labs.UI.Controllers
{
    public class ImageController : Controller
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly IWebHostEnvironment _env;

        public ImageController(UserManager<AppUser> userManager, IWebHostEnvironment env)
        {
            _userManager = userManager;
            _env = env;
        }

        public async Task<IActionResult> GetAvatar()
        {
            var email = User.Identity?.Name;
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user?.Avatar != null && !string.IsNullOrEmpty(user.MimeType))
                {
                    return File(user.Avatar, user.MimeType);
                }
            }

            // Возвращаем изображение по умолчанию
            var defaultImagePath = Path.Combine(_env.WebRootPath, "images", "default-profile-picture.png");
            if (System.IO.File.Exists(defaultImagePath))
            {
                return PhysicalFile(defaultImagePath, "image/*");
            }

            return NotFound();
        }
    }
}

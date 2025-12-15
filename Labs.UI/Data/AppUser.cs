using Microsoft.AspNetCore.Identity;

namespace Labs.UI.Data
{
    public class AppUser : IdentityUser
    {
        public byte[]? Avatar { get; set; }
        public string MimeType { get; set; } = "image/*";
    }
}

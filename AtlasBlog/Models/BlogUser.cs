using Microsoft.AspNetCore.Identity;

namespace AtlasBlog.Models
{
    public class BlogUser : IdentityUser
    {
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string? DisplayName { get; set; }

    }
}

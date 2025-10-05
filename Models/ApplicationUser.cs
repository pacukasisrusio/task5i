using Microsoft.AspNetCore.Identity;

namespace penkta.Models
{
    public class ApplicationUser : IdentityUser
    {
        public DateTime? LastLoginAt { get; set; }
        public bool IsBlocked { get; set; }
    }
}

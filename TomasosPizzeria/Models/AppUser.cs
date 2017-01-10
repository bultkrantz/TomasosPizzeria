using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace TomasosPizzeria.Models
{
    public class AppUser : IdentityUser
    {
        public int CustomerId { get; set; }
    }
}

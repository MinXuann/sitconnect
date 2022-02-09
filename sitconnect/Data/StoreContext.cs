using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using sitconnect.Models;
using Microsoft.EntityFrameworkCore;

namespace sitconnect.Data
{
    public class StoreContext : IdentityDbContext<User, IdentityRole, string>
    {
        public StoreContext(DbContextOptions<StoreContext> options) : base(options)
        {
        }

    }
}
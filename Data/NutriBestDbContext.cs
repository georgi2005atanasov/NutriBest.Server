using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using NutriBest.Server.Data.Models;

namespace NutriBest.Server.Data
{
    public class NutriBestDbContext : IdentityDbContext<User>
    {
        public NutriBestDbContext(DbContextOptions<NutriBestDbContext> options)
            : base(options)
        {
        }
    }
}
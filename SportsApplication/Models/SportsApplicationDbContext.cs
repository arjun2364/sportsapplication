using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SportsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Sports_Application.Models
{
    public class SportsApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public SportsApplicationDbContext(DbContextOptions<SportsApplicationDbContext> options)
               : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        public DbSet<Result> Results { get; set; }
        public DbSet<Test> Tests { get; set; }
    }
}

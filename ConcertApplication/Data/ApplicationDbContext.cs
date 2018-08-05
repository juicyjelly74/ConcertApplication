using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ConcertApplication.Models;

namespace ConcertApplication.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ConcertModel>()
            .HasDiscriminator<string>(nameof(ConcertModel.Type))
            .HasValue<ClassicalConcertModel>(nameof(ClassicalConcertModel))
            .HasValue<OpenAirModel>(nameof(OpenAirModel))
            .HasValue<PartyModel>(nameof(PartyModel));

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<ConcertApplication.Models.ConcertModel> Concerts { get; set; }
        public DbSet<ClassicalConcertModel> ClassicalConcerts { get; set; }
        public DbSet<PartyModel> Parties { get; set; }
        public DbSet<OpenAirModel> OpenAirs { get; set; }
        public DbSet<TicketModel> Tickets { get; set; }
    }
}

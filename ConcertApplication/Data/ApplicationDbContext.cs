﻿using System;
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

            builder.Entity<Concert>()
            .HasDiscriminator<string>(nameof(Concert.Type))
            .HasValue<ClassicConcert>(nameof(ClassicConcert))
            .HasValue<OpenAir>(nameof(OpenAir))
            .HasValue<Party>(nameof(Party));

            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }
        public DbSet<ConcertApplication.Models.Concert> Concerts { get; set; }
        public DbSet<ClassicConcert> ClassicConcerts { get; set; }
        public DbSet<Party> Parties { get; set; }
        public DbSet<OpenAir> OpenAirs { get; set; }
        public DbSet<ConcertApplication.Models.Ticket> Tickets { get; set; }
    }
}

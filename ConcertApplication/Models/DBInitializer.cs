using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ConcertApplication.Data;
using Microsoft.EntityFrameworkCore;

namespace ConcertApplication.Models
{
    public class DBInitializer
    {
        public static async Task InitializeAsync(ApplicationDbContext context, IServiceProvider serviceProvider)
        {
            context.Database.EnsureCreated();
            RoleManager<IdentityRole> _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> _userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            string[] roleNames = { "Admin", "User" };
            IdentityResult roleResult;
            foreach (string role in roleNames)
            {
                bool roleExist = await _roleManager.RoleExistsAsync(role);
                if (!roleExist)
                {
                    roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            ApplicationUser admin = new ApplicationUser { UserName = "adminmail@gmail.com", Email = "adminmail@gmail.com" };
            var resultAdmin = await _userManager.CreateAsync(admin, "1234Qw_");
            if (resultAdmin.Succeeded)
            {
                await _userManager.AddToRoleAsync(admin, roleNames[0]);
            }
            ApplicationUser user = new ApplicationUser { UserName = "usermail@gmail.com", Email = "usermail@gmail.com" };
            var resultUser = await _userManager.CreateAsync(user, "4321Wq_");
            if (resultUser.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, roleNames[1]);
            }

            Party party = new Party
            {
                Performer = "DJ BlaBla",
                TicketsAmount = 10,
                TicketsLeft = 10,
                Date = new DateTime(2018, 08, 10),
                Place = "Minsk",
                Price = 10
            };

            Concert concert = await context.Concerts.FindAsync(party);

            if (concert == null)
            {
                context.Parties.Add(party);
                context.SaveChanges();
            }
               
        }
        
    }
}

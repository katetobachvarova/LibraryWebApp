using LibraryWebApp.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LibraryWebApp.Data
{
    public static class DbContextSeedData 
    {
        public static async void Seed(IApplicationBuilder app)
        {
            // Get an instance of the DbContext from the DI container
            using (var context = app.ApplicationServices.GetRequiredService<ApplicationDbContext>())
            {
                if ( context.Database.EnsureCreated() && !context.Users.Any() )
                {
                    //Add roles
                    var adminRole = new IdentityRole("Admin");
                    var libRole = new IdentityRole("Librarian");
                    var regRole = new IdentityRole("RegUser");
                    context.Add(adminRole);
                    context.Add(libRole);
                    context.Add(regRole);
                    context.SaveChanges();

                    //Add admin user
                    var user = new ApplicationUser { UserName = "admin@abv.bg", Email = "admin@abv.bg", Confirmed = true };
                    var userManager = app.ApplicationServices.GetRequiredService<Microsoft.AspNetCore.Identity.UserManager<ApplicationUser>>();
                    await userManager.CreateAsync(user, "Admin_123");
                    context.SaveChanges();

                    var role = new IdentityUserRole<string>();
                    role.RoleId = adminRole.Id;
                    role.UserId = user.Id;
                    context.Add(role);

                    //Add library items
                    var s = new Section() { Name = "Art literature", Description = "Art literature" };
                    var f = new Section() { Name = "Fantasy", Description = "Fantasy" };
                    context.Add(s);
                    context.Add(f);
                    var rb = new Title() { Name = "The Red and the Black", Section = s, Author = "Stendhal", Annotation = "historical psychological novel", Type = "novel", ISBN = "0-521-34982-6", Year = 1830, Publisher = "A.Levasseur" };
                    var g = new Title() { Name = "Guards!Guards!", Section = f, Author = "Terry Pratchett", Annotation = "Discworld", Type = "novel", ISBN = "0-575-04606-6", Year = 1989, Publisher = "Corgi" };
                    context.Add(rb);
                    context.Add(g);
                    var rbitem1 = new Item() {Condition = "Good", CurrentLocation = "National Library", Material = "Paper", Title = rb };
                    var rbitem2 = new Item() { Condition = "Bad", CurrentLocation = "National Library", Material = "Paper", Title = rb };
                    var gitem1 = new Item() { Condition = "Good", CurrentLocation = "National Library", Material = "Paper", Title = g };
                    var gitem2 = new Item() { Condition = "Good", CurrentLocation = "National Library", Material = "CD", Title = g };
                    context.Add(rbitem1);
                    context.Add(rbitem2);
                    context.Add(gitem1);
                    context.Add(gitem2);
                    context.SaveChanges();

                    var fav = new Favourite() {Comment = "admin's favourite book", User = user, ItemIndex = gitem1.ItemId.ToString()};
                    context.Add(fav);
                    var itemmov = new ItemMovement() {User = user, Librarian = user, Condition = "Good", Date = DateTime.Today, Deadline = new DateTime(2016,12,21), MovementType = Models.Enum.MovementType.Take.ToString(), Item = gitem1 };
                    context.Add(itemmov);

                    context.SaveChanges();
                    context.Dispose();

                }


            }
        }
    }
}

using System;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using PlatformService.Models;

namespace PlatformService.Data
{
    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.CreateScope())
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>());
            }
        }

        private static void SeedData(AppDbContext context) 
        {
            if (!context.Platforms.Any())
            {
                Console.WriteLine("--> Seeding Data...")
            
                context.Platforms.AddRange(
                    new Platform() {Name="Dot Net", Publisher="Microsoft", Cost="Free"},
                    new Platform() {Name="Dot Net 2", Publisher="Microsoft 2", Cost="Free 2"},
                    new Platform() {Name="Dot Net 3", Publisher="Microsoft 3", Cost="Free 3"}
                );
            }
            else
            {
                Console.WriteLine("--> We already have data");
            } 

        }
    }
}
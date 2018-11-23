using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new MovieAPIContext(
                serviceProvider.GetRequiredService<DbContextOptions<MovieAPIContext>>()))
            {
                // Look for any movies.
                if (context.MovieItem.Count() > 0)
                {
                    return;   // DB has been seeded
                }

                context.MovieItem.AddRange(
                    new MovieItem
                    {
                        Title = "Avengers",
                        Playtime = 90,
                        Genre = "Action",
                        Rating = "PG",
                        Trailer = "https://www.youtube.com/embed/eOrNdBpGMv8",
                        Url = "https://i.ytimg.com/vi/Sv_an8I2ME0/maxresdefault.jpg",
                        Uploaded = "07-10-18 4:20T18:25:43.511Z",
                        Width = "1280",
                        Height = "720"
                    }


                );
                context.SaveChanges();
            }
        }
    }
}

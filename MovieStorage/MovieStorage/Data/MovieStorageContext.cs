using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieStorage.Models
{
    public class MovieStorageContext : DbContext
    {
        public MovieStorageContext (DbContextOptions<MovieStorageContext> options)
            : base(options)
        {
        }

        public DbSet<MovieStorage.Models.MovieItem> MovieItem { get; set; }
    }
}

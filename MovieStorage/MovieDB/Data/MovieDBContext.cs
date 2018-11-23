using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieDB.Models
{
    public class MovieDBContext : DbContext
    {
        public MovieDBContext (DbContextOptions<MovieDBContext> options)
            : base(options)
        {
        }

        public DbSet<MovieDB.Models.MovieItem> MovieItem { get; set; }
    }
}

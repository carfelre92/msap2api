using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace MovieAPI.Models
{
    public class MovieAPIContext : DbContext
    {
        public MovieAPIContext (DbContextOptions<MovieAPIContext> options)
            : base(options)
        {
        }

        public DbSet<MovieAPI.Models.MovieItem> MovieItem { get; set; }
    }
}

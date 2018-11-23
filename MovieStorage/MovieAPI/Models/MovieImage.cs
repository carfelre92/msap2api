using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieAPI.Models
{
    public class MovieImage
    {
        public string Title { get; set; }
        public int Playtime { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
        public string Trailer { get; set; }
        public IFormFile Image { get; set; }
    }
}

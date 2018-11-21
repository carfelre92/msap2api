using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieStorage.Models
{
    public class MovieImage
    {
        public string Title { set; }
        public int Playtime { get; set; }
        public string Genre { get; set; }
        public string Rating { get; set; }
        public IFormFile Image { get; set; }
    }
}

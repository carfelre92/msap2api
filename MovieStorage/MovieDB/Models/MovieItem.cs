using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MovieDB.Models
{
    public class MovieItem
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Playtime { get; set; }
        public string Genre { get; set; }
        public string Rating { get; internal set; }
        public string Trailer { get; set; }
        public string Url { get; set; }
        public string Uploaded { get; set; }
        public string Width { get; set; }
        public string Height { get; set; }

    }
}

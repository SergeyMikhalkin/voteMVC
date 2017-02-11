using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class Rating
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Competition> Competitions { get; set; }

        public Rating()
        {
            Competitions = new List<Competition>();
        }
    }
}
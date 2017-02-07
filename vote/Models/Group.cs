using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class Group
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Competition> Competitions { get; set; }

        public Group()
        {
            Competitions = new List<Competition>();
        }
    }
}
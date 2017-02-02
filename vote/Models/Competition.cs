using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class Competition
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public string Date { get; set; }
        public string Place { get; set; }
        public string Kind { get; set; }
        public string Special { get; set; }
        public string EliteRating { get; set; }
        public string OldRating { get; set; }
        public string SprintRating { get; set; }
    }
}
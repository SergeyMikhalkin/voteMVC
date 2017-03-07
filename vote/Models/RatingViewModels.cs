using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class RatingViewModels
    {
        public int CompetitionId { get; set; }
        public string Name { get; set; }
        public string Date { get; set; }
        public string Kind { get; set; }
        public double Score { get; set; }
    }

    public class RatingVoteModel
    {
        public int Info { get; set; }
        public int Place { get; set; }
        public int Map { get; set; }
        public int Distance { get; set; }
        public int Sealed { get; set; }
        public int Print { get; set; }
        public int Start { get; set; }
        public int Finish { get; set; }
        public int Results { get; set; }
        public int Center { get; set; }
        public string GroupName { get; set; }
    }
    public class RatingTableModel
    {
        public int Info { get; set; }
        public int Place { get; set; }
        public int Map { get; set; }
        public int Distance { get; set; }
        public int Sealed { get; set; }
        public int Print { get; set; }
        public int Start { get; set; }
        public int Finish { get; set; }
        public int Results { get; set; }
        public int Center { get; set; }
        public int CountOfVoters { get; set; }
        public int Middle { get; set; }
    }
}
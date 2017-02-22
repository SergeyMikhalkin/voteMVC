using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class VoteViewModel
    {
        public int Id { get; set; }
        public int Info { get; set; }
        public int Place { get; set; }
        public int Map { get; set; }
        public int Print { get; set; }
        public int Sealed { get; set; }
        public int Distance { get; set; }
        public int Start { get; set; }
        public int Finish { get; set; }
        public int Results { get; set; }
        public int Center { get; set; }

        public string UserID { get; set; }
        public int CompetitionID { get; set; }
        public int GroupID { get; set; }

    }
}
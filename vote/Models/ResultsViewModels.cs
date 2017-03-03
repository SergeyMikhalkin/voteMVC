using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class ResultsViewModels
    {
        public class ResultVoteModel
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

        public class ResultTableModel
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

        public class ResultCommentModel
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
        }

        public class UserGroup
        {
            public string UserID { get; set; }
            public string GroupName { get; set; }
        }
    }
}
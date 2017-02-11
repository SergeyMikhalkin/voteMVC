using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class Comment
    {
        public int Id { get; set; }
        public string FieldName { get; set; }
        public string Text { get; set; }
        public int CompetitionId { get; set; }
        public Competition Competition { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
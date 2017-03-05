using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace vote.Models
{
    public class CommentViewModel
    {
        public string FieldName { get; set; }
        public string Text { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string UserId { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vote.Models;

namespace vote.Controllers
{
    public class CommentController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Comments for map
        public ActionResult Show(int id)
        {
            // Info about competition
            ViewBag.Competition = db.Competitions.Single(competition => competition.Id == id);

            // Get comments
            var comments = db.Comments.Where(comment => comment.CompetitionId == id).GroupBy(field => field.FieldName);

            return View(comments);
        }
    }
}
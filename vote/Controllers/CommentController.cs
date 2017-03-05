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
            IQueryable<IGrouping<string, CommentViewModel>> comments;

            try
            {
                // Info about competition
                ViewBag.Competition = db.Competitions.Single(competition => competition.Id == id);

                // Get comments
                comments = db.Comments.Where(comment => comment.CompetitionId == id).Select(x => new CommentViewModel()
                {
                    FieldName = x.FieldName,
                    Text = x.Text,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    UserId = x.User.Id
                }).GroupBy(field => field.FieldName);

            }
            catch (Exception)
            {
                return View("Error");
            }
            
            return View(comments);
        }
    }
}
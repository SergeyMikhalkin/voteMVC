using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vote.Models;

namespace vote.Controllers
{
    public class AdminController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Admin
        [Authorize(Roles ="Admin")]
        public ActionResult Competitions()
        {
            var competitions = db.Competitions;
            return View(competitions);
        }

        // Delete competition
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            try
            {
                Competition competition = db.Competitions.Single(c => c.Id == id);
                db.Competitions.Remove(competition);
                db.SaveChanges();
            }
            catch (Exception)
            {
                return View("Error");
            }
            return RedirectToAction("Competitions");
        }
    }
}
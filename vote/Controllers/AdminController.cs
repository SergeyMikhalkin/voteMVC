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
    }
}
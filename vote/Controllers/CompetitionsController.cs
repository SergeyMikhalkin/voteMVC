using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vote.Models;

namespace vote.Controllers
{
    public class CompetitionsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Competitions
        public ActionResult All()
        {
            return View(db.Competitions);
        }
    }
}
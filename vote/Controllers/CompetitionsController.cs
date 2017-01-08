using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vote.Controllers
{
    public class CompetitionsController : Controller
    {
        // GET: Competitions
        public ActionResult AllCompetitions()
        {
            return View();
        }
    }
}
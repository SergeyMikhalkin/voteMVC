using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace vote.Controllers
{
    public class RatingController : Controller
    {
        // GET: Rating
        public ActionResult FullRating()
        {
            return View();
        }
    }
}
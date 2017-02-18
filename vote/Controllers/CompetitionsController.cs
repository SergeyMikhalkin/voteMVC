using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
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
            string userName = User.Identity.Name;
            if(userName != string.Empty)
            {
                string userId = string.Empty;
                try
                {
                    userId = db.Users.Single(x => x.UserName == userName).Id;
                }
                catch (Exception)
                {
                    return View("Error");
                }

                Hashtable votedCompetitions = new Hashtable();
                FillVotedCompetitions(votedCompetitions, userId);
                ViewBag.VotedCompetitions = votedCompetitions;
            }
            
            Hashtable CountOfVoters = new Hashtable();
            FillCountOfVoters(CountOfVoters);
            ViewBag.CountOfVoters = CountOfVoters;

            ViewBag.CompetitionCount = db.Competitions.Count();
            ViewBag.Competition = db.Competitions.ToArray();

            return View();
        }

        // Voted competitions user's hashtable
        private void FillVotedCompetitions(Hashtable votedCompetitions, string id)
        {
            var _votedCompetitons = from c in db.Votes
                                    where c.UserId == id
                                    select c;
            foreach (var competition in _votedCompetitons)
            {
                votedCompetitions.Add(competition.Id, true);
            }
                                    
        }

        // fill hashtable 'competition' -> 'count of voters'
        private void FillCountOfVoters(Hashtable countOfVoters)
        {
            var votersGroupByMapID = from v in db.Votes
                                     group v by v.CompetitionID;

            foreach (var group in votersGroupByMapID)
            {
                countOfVoters.Add(group.Key, group.Count());
            }
        }
    }
}
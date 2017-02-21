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

        [Authorize]
        public ActionResult Vote(int? Id)
        {
            // get User ID
            string userId = getUserId(User.Identity.Name);

            if (userId == "Error") return View("Error");

            // list of groups for competition
            var groupsForCompetition = db.Groups.Where(x => x.Competitions.Any(c => c.Id == Id)).Select(r => r);
            ViewBag.Groups = new SelectList(groupsForCompetition, "Id", "Name");

            // User already vote check
            bool userVotedFor = db.Votes.Any(x => x.UserId == userId && x.CompetitionID == Id);
            return userVotedFor ? View("All") : View("Group", Id);
        }

        // find user ID by name
        private string getUserId(string userName)
        {

            string userId = string.Empty;
            if (userName != string.Empty)
            {
                userId = string.Empty;
                try
                {
                    userId = db.Users.Single(x => x.UserName == userName).Id;
                    return userId;
                }
                catch (Exception)
                {
                    return "Error";
                }
            }
            else
            {
                return "Error";
            }
        }
    }
}
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
            if (userName != string.Empty)
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
            ViewBag.Competition = db.Competitions.OrderByDescending(x => x.Date.Substring(3, 2)).ThenByDescending(x => x.Date.Substring(0, 2)).ToList();

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
                votedCompetitions.Add(competition.CompetitionID, true);
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
        public ActionResult Vote(int Id)
        {
            // get User ID
            string userId = getUserId(User.Identity.Name);

            if (userId == "Error") return View("Error");

            // list of groups for competition
            var groupsForCompetition = db.Groups.Where(x => x.Competitions.Any(c => c.Id == Id)).Select(r => r);
            ViewBag.Groups = new SelectList(groupsForCompetition, "Id", "Name");

            // User already vote check
            bool userVotedFor = db.Votes.Any(x => x.UserId == userId && x.CompetitionID == Id);

            // model for save answers
            VoteViewModel vote = new VoteViewModel();

            // fill competition id
            vote.CompetitionID = Id;

            try
            {
                vote.UserID = db.Users.Single(x => x.Id == userId).Id;
            }
            catch (Exception)
            {
                return View("Error");
            }

            return userVotedFor ? View("All") : View("Group", vote);
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

        [Authorize]
        [HttpPost]
        public ActionResult Group(int groupIdFromForm, VoteViewModel voteViewModel)
        {

            if (voteViewModel == null) return View("Error");

            voteViewModel.GroupID = groupIdFromForm;

            ModelState.Clear();

            return View("Info", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Info(int infoGrade, string commentAboutInfo, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutInfo != string.Empty)
            {
                if (AddComment(commentAboutInfo, "Info", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Info = infoGrade;
            ModelState.Clear();

            return View("Place", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Place(int placeGrade, string commentAboutPlace, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutPlace != string.Empty)
            {
                if (AddComment(commentAboutPlace, "Place", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Place = placeGrade;
            ModelState.Clear();

            return View("Map", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Map(int mapGrade, string commentAboutMap, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutMap != string.Empty)
            {
                if (AddComment(commentAboutMap, "Map", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Map = mapGrade;
            ModelState.Clear();

            return View("Print", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Print(int printGrade, string commentAboutPrint, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutPrint != string.Empty)
            {
                if (AddComment(commentAboutPrint, "Print", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Print = printGrade;
            ModelState.Clear();

            return View("Sealed", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Sealed(int sealedGrade, string commentAboutSealed, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutSealed != string.Empty)
            {
                if (AddComment(commentAboutSealed, "Sealed", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Sealed = sealedGrade;
            ModelState.Clear();

            return View("Distance", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Distance(int distanceGrade, string commentAboutDistance, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutDistance != string.Empty)
            {
                if (AddComment(commentAboutDistance, "Distance", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Distance = distanceGrade;
            ModelState.Clear();

            return View("Start", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Start(int startGrade, string commentAboutStart, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutStart != string.Empty)
            {
                if (AddComment(commentAboutStart, "Start", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Start = startGrade;
            ModelState.Clear();

            return View("Finish", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Finish(int finishGrade, string commentAboutFinish, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutFinish != string.Empty)
            {
                if (AddComment(commentAboutFinish, "Finish", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Finish = finishGrade;
            ModelState.Clear();

            return View("Results", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Results(int resultsGrade, string commentAboutResults, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutResults != string.Empty)
            {
                if (AddComment(commentAboutResults, "Results", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Results = resultsGrade;
            ModelState.Clear();

            return View("Center", voteViewModel);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Center(int centerGrade, string commentAboutCenter, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutCenter != string.Empty)
            {
                if (AddComment(commentAboutCenter, "Center", User.Identity.Name, voteViewModel.CompetitionID) == false) return View("Error");
            }

            voteViewModel.Center = centerGrade;

            int competitionId;

            if(SaveVote(voteViewModel, out competitionId) == false) return View("Error");
            
            // go to results
            return RedirectToRoute(new
            {
                controller = "Results",
                action = "Show",
                id = competitionId
            });
        }

        private bool SaveVote(VoteViewModel voteViewModel, out int competitionId)
        {
            // copy results for save
            Vote voteForSave = new Vote();

            voteForSave.Info = voteViewModel.Info;
            voteForSave.Place = voteViewModel.Place;
            voteForSave.Map = voteViewModel.Map;
            voteForSave.Print = voteViewModel.Print;
            voteForSave.Sealed = voteViewModel.Sealed;
            voteForSave.Distance = voteViewModel.Distance;
            voteForSave.Start = voteViewModel.Start;
            voteForSave.Finish = voteViewModel.Finish;
            voteForSave.Results = voteViewModel.Results;
            voteForSave.Center = voteViewModel.Center;

            try
            {
                ApplicationUser user = db.Users.Find(voteViewModel.UserID);
                Competition competition = db.Competitions.Find(voteViewModel.CompetitionID);
                Group group = db.Groups.Find(voteViewModel.GroupID);

                voteForSave.User = user;
                voteForSave.UserId = user.Id;
                voteForSave.Competition = competition;
                voteForSave.CompetitionID = competition.Id;
                voteForSave.Group = group;
                voteForSave.GroupID = group.Id;

                // save results
                db.Votes.Add(voteForSave);
                db.SaveChanges();
            }
            catch (Exception)
            {
                competitionId = -1;
                return false;
            }

            competitionId = voteForSave.CompetitionID;
            return true;
        }

        private bool AddComment(string comment, string fieldName, string userName, int competitionId)
        {
            // create comment
            Comment newComment = new Comment() { Text = comment, FieldName = fieldName };

            // get user
            ApplicationUser user = new ApplicationUser();
            if (getUserByName(userName, ref user) == "Error")
            {
                return false;
            }

            Competition competition = new Competition();
            if (getCompetitionById(competitionId, ref competition) == "Error")
            {
                return false;
            }

            // assign user and competition to comment
            newComment.User = user;
            newComment.Competition = competition;
            newComment.UserId = user.Id;
            newComment.CompetitionId = competition.Id;

            // create new comment in db
            db.Comments.Add(newComment);
            db.SaveChanges();

            return true;
        }

        private string getCompetitionById(int competitionId, ref Competition competition)
        {
            try
            {
                competition = db.Competitions.Single(c => c.Id == competitionId);
            }
            catch (Exception)
            {
                return "Error";
            }

            return "Ok";
        }

        private string getUserByName(string userName, ref ApplicationUser user)
        {
            try
            {
                user = db.Users.Single(u => u.UserName == userName);
            }
            catch (Exception)
            {
                return "Error";
            }

            return "Ok";
        }

    }
}
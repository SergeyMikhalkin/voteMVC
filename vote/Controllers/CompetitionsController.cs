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

        [HttpPost]
        public ActionResult Group(int groupIdFromForm, VoteViewModel voteViewModel)
        {

            if (voteViewModel == null) return View("Error");

            voteViewModel.GroupID = groupIdFromForm;

            ModelState.Clear();

            return View("Info", voteViewModel);
        }

        [HttpPost]
        public ActionResult Info(int infoGrade, string commentAboutInfo, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutInfo != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutInfo, FieldName = "Info" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Info = infoGrade;
            ModelState.Clear();
            return View("Place", voteViewModel);
        }

        [HttpPost]
        public ActionResult Place(int placeGrade, string commentAboutPlace, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutPlace != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutPlace, FieldName = "Place" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Place = placeGrade;
            ModelState.Clear();

            return View("Map", voteViewModel);
        }

        [HttpPost]
        public ActionResult Map(int mapGrade, string commentAboutMap, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutMap != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutMap, FieldName = "Map" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Map = mapGrade;
            ModelState.Clear();

            return View("Print", voteViewModel);
        }

        [HttpPost]
        public ActionResult Print(int printGrade, string commentAboutPrint, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutPrint != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutPrint, FieldName = "Print" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Print = printGrade;
            ModelState.Clear();

            return View("Sealed", voteViewModel);
        }

        [HttpPost]
        public ActionResult Sealed(int sealedGrade, string commentAboutSealed, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutSealed != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutSealed, FieldName = "Sealed" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Sealed = sealedGrade;
            ModelState.Clear();

            return View("Distance", voteViewModel);
        }

        [HttpPost]
        public ActionResult Distance(int distanceGrade, string commentAboutDistance, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutDistance != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutDistance, FieldName = "Distance" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Distance = distanceGrade;
            ModelState.Clear();

            return View("Start", voteViewModel);
        }

        [HttpPost]
        public ActionResult Start(int startGrade, string commentAboutStart, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutStart != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutStart, FieldName = "Start" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Start = startGrade;
            ModelState.Clear();

            return View("Finish", voteViewModel);
        }

        [HttpPost]
        public ActionResult Finish(int finishGrade, string commentAboutFinish, VoteViewModel voteViewModel)
        {
            if (voteViewModel == null) return View("Error");

            if (commentAboutFinish != string.Empty)
            {

                // create comment
                Comment newComment = new Comment() { Text = commentAboutFinish, FieldName = "Finish" };

                // get user
                ApplicationUser user = new ApplicationUser();
                if (getUserByName(User.Identity.Name, ref user) == "Error")
                {
                    return View("Error");
                }

                Competition competition = new Competition();
                if (getCompetitionById(voteViewModel.CompetitionID, ref competition) == "Error")
                {
                    return View("Error");
                }

                // assign user and competition to comment
                newComment.User = user;
                newComment.Competition = competition;
                newComment.UserId = user.Id;
                newComment.CompetitionId = competition.Id;

                // create new comment in db
                db.Comments.Add(newComment);
                db.SaveChanges();
            }

            voteViewModel.Finish = finishGrade;
            ModelState.Clear();

            return View("Results", voteViewModel);
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
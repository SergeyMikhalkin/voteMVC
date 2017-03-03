using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vote.Models;
using static vote.Models.ResultsViewModels;

namespace vote.Controllers
{
    public class ResultsController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Results by id
        public ActionResult Show(int id)
        {
            ViewBag.CompetitionId = id;

            var results = from v in db.Votes
                          join g in db.Groups on v.GroupID equals g.Id
                          where v.CompetitionID == id
                          select new ResultVoteModel()
                          {
                              Info = v.Info,
                              Place = v.Place,
                              Map = v.Map,
                              Distance = v.Distance,
                              Sealed = v.Sealed,
                              Print = v.Print,
                              Start = v.Start,
                              Finish = v.Finish,
                              Center = v.Center,
                              Results = v.Results,
                              GroupName = g.Name
                          };

            var groupType = db.Competitions.Where(x => x.Id == id).SelectMany(x => x.Groups);
            var comments = db.Competitions.Where(x => x.Id == id).SelectMany(x => x.Comments);

            var ListOfUsersGroups = db.Votes.Where(x => x.CompetitionID == id)
                                      .Select(x => new UserGroup()
            {
                UserID = x.UserId,
                GroupName = x.Group.Name
            }).ToList();

            string groupExample = string.Empty;
            foreach (var group in groupType)
            {
                groupExample = group.Name;
                break;
            }

            // Type: M, W
            // Type: ME, MA, MB, WE, WA, WB
            if (groupExample.Length <= 2)
            {
                

                ResultTableModel men = new ResultTableModel();
                ResultTableModel woman = new ResultTableModel();
                if (SplitByAge(results, men, woman) == false)
                {
                    return View("Error");
                }

                // Type: M, W
                // Type: ME, MA, MB, WE, WA, WB  = 2 (men, woman)
                ViewBag.CountOfTables = 2;

                ViewBag.MenResults = men;
                ViewBag.WomanResults = woman;

                ViewBag.TotalForCompetition = GetTotalForCompetition(men, woman);

                ResultCommentModel menComments = new ResultCommentModel();
                ResultCommentModel womanComments = new ResultCommentModel();
                if (CommentsByAge(comments, ListOfUsersGroups, menComments, womanComments) == false)
                {
                    return View("Error");
                }

                ViewBag.MenComments = menComments;
                ViewBag.WomanComments = womanComments;              
            }

            // Type: M10, W10, M21E, e.t.c.
            if (groupExample.Length > 2)
            {
                ResultTableModel resultsUnder21 = new ResultTableModel();
                ResultTableModel results21 = new ResultTableModel();
                ResultTableModel resultOver21 = new ResultTableModel();

                if (SplitByAge(results, resultsUnder21, results21, resultOver21) == false)
                {
                    return View("Error");
                }

                // Type: M10, W10, M21E, e.t.c. = 3 (<21, 21, >21)
                ViewBag.CountOfTables = 3;
                ViewBag.ResultsUnder21 = resultsUnder21;
                ViewBag.Results21 = results21;
                ViewBag.ResultsOver21 = resultOver21;

                ViewBag.TotalForCompetition = GetTotalForCompetition(resultsUnder21, results21, resultOver21);


                ResultCommentModel commentsUnder21 = new ResultCommentModel();
                ResultCommentModel comments21 = new ResultCommentModel();
                ResultCommentModel commentsOver21 = new ResultCommentModel();

                if (CommentsByAge(comments, ListOfUsersGroups, commentsUnder21, comments21, commentsOver21) == false)
                {
                    return View("Error");
                }

                ViewBag.CommentsUnder21 = commentsUnder21;
                ViewBag.Comments21 = comments21;
                ViewBag.CommentsOver21 = commentsOver21;
            }

            return View();
        }

        private bool SplitByAge(IQueryable<ResultVoteModel> results, ResultTableModel men, ResultTableModel woman)
        {
            foreach (var vote in results)
            {
                if(vote.GroupName[0] == 'М')
                {
                    addToResultTable(vote, men);
                }

                if(vote.GroupName[0] == 'Ж')
                {
                    addToResultTable(vote, woman);
                }                
            }

            getMiddle(men);
            getMiddle(woman);

            return true;
        }

        private bool SplitByAge(IQueryable<ResultVoteModel> results, ResultTableModel resultsUnder21, ResultTableModel results21, ResultTableModel resultOver21)
        {
            foreach (var vote in results)
            {
                // example: W21E -> 21
                int age = Convert.ToInt32(vote.GroupName.Substring(1, 2));
                if(age < 21)
                {
                    addToResultTable(vote, resultsUnder21);
                }
                if(age == 21)
                {
                    addToResultTable(vote, results21);
                }
                if (age > 21)
                {
                    addToResultTable(vote, resultOver21);
                }
            }

            getMiddle(resultsUnder21);
            getMiddle(results21);
            getMiddle(resultOver21);

            return true;
        }

        private void addToResultTable(ResultVoteModel vote, ResultTableModel result)
        {
            result.Info += vote.Info;
            result.Place += vote.Place;
            result.Map += vote.Map;
            result.Sealed += vote.Sealed;
            result.Distance += vote.Distance;
            result.Print += vote.Print;
            result.Start += vote.Start;
            result.Finish += vote.Finish;
            result.Results += vote.Results;
            result.Center += vote.Center;

            result.CountOfVoters++;
        }

        private void getMiddle(ResultTableModel table)
        {
            if(table.CountOfVoters != 0)
            {
                table.Middle = (table.Info / table.CountOfVoters) +
                               (table.Place / table.CountOfVoters) +
                               (table.Map / table.CountOfVoters) +
                               (table.Sealed / table.CountOfVoters) +
                               (table.Distance / table.CountOfVoters) +
                               (table.Print / table.CountOfVoters) +
                               (table.Start / table.CountOfVoters) +
                               (table.Finish / table.CountOfVoters) +
                               (table.Results / table.CountOfVoters) +
                               (table.Center / table.CountOfVoters);
            }
        }



        private bool CommentsByAge(IQueryable<Comment> comments, List<UserGroup> usersGroups, ResultCommentModel commentsUnder21, ResultCommentModel comments21, ResultCommentModel commentsOver21)
        {
            foreach (var comment in comments)
            {
                string group = usersGroups.First(x => x.UserID == comment.UserId).GroupName;

                // get age. for example: W21E -> 21
                int age = Convert.ToInt32(group.Substring(1, 2));

                if (age < 21)
                {
                    IncrementCommentsTableField(comment, commentsUnder21);
                }
                if (age == 21)
                {
                    IncrementCommentsTableField(comment, comments21);
                }
                if (age > 21)
                {
                    IncrementCommentsTableField(comment, commentsOver21);
                }
            }

            return true;
        }
        private bool CommentsByAge(IQueryable<Comment> comments, List<UserGroup> usersGroups, ResultCommentModel menComments, ResultCommentModel womanComments)
        {
            foreach (var comment in comments)
            {
                string group = usersGroups.First(x => x.UserID == comment.UserId).GroupName;

                if (group[0] == 'М')
                 {
                    IncrementCommentsTableField(comment, menComments);
                 }

                 if (group[0] == 'Ж')
                 {
                    IncrementCommentsTableField(comment, womanComments);
                 }
            }

            return true;
        }

        private void IncrementCommentsTableField(Comment comment, ResultCommentModel commentTable)
        {
            switch (comment.FieldName)
            {
                case "Info":
                    commentTable.Info++;
                    break;
                case "Place":
                    commentTable.Place++;
                    break;
                case "Map":
                    commentTable.Map++;
                    break;
                case "Print":
                    commentTable.Print++;
                    break;
                case "Distance":
                    commentTable.Distance++;
                    break;
                case "Sealed":
                    commentTable.Sealed++;
                    break;
                case "Start":
                    commentTable.Start++;
                    break;
                case "Finish":
                    commentTable.Finish++;
                    break;
                case "Center":
                    commentTable.Center++;
                    break;
                case "Results":
                    commentTable.Results++;
                    break;
                default:
                    break;
            }
        }

        private double GetTotalForCompetition(ResultTableModel resultsUnder21, ResultTableModel results21, ResultTableModel resultOver21)
        {
            double total = 0;

            int count = 0;

            count += resultsUnder21.Middle == 0 ? 0 : 1;
            count += results21.Middle == 0 ? 0 : 1;
            count += resultOver21.Middle == 0 ? 0 : 1;

            double ResultsUnder21 = (double)resultsUnder21.Middle;
            double Results21 = (double)results21.Middle;
            double ResultsOver21 = (double)resultOver21.Middle;

            if (count != 0)
            {
                total = (ResultsUnder21 + Results21 + ResultsOver21) / count;
            }
           
            return total;
        }

        private double GetTotalForCompetition(ResultTableModel men, ResultTableModel woman)
        {
            double total = 0;

            int count = 0;

            count += men.Middle == 0 ? 0 : 1;
            count += woman.Middle == 0 ? 0 : 1;

            double ResultsMen = (double)men.Middle;
            double ResultsWoman = (double)woman.Middle;

            if (count != 0)
            {
                total = (ResultsMen + ResultsWoman) / count;
            }

            return total;
        }

    }
}
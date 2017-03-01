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

        private double GetTotalForCompetition(ResultTableModel resultsUnder21, ResultTableModel results21, ResultTableModel resultOver21)
        {
            double total = 0;

            int count = 0;

            count += resultsUnder21.Middle == 0 ? 0 : 1;
            count += results21.Middle == 0 ? 0 : 1;
            count += resultOver21.Middle == 0 ? 0 : 1;

            if(count != 0)
            {
                total = (resultsUnder21.Middle + results21.Middle + resultOver21.Middle) / count;
            }
           
            return total;
        }

        private double GetTotalForCompetition(ResultTableModel men, ResultTableModel woman)
        {
            double total = 0;

            int count = 0;

            count += men.Middle == 0 ? 0 : 1;
            count += woman.Middle == 0 ? 0 : 1;

            if (count != 0)
            {
                total = (men.Middle + woman.Middle) / count;
            }

            return total;
        }


    }
}
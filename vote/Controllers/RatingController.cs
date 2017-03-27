using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using vote.Models;

namespace vote.Controllers
{
    public class RatingController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        // GET: Rating
        public ActionResult Full(int? year, string typeOfRating)
        {
            if(year == null)
            {
                year = DateTime.Now.Year;
            }
            // get current year
            ViewBag.CurrentYear = year; 

            List<string> listOfYears = GetListOfYears();          
            ViewBag.ListOfYears = new SelectList(listOfYears, year.ToString());

            List<RatingViewModels> ratingTable = new List<RatingViewModels>();

            int ratingId = 0;
            try
            {
                switch (typeOfRating)
                {
                    case "Кубок БФО":
                        ratingId = db.Ratings.Single(r => r.Name == "Кубок БФО").Id;
                        ratingTable = GetRating(year.Value, ratingId);
                        break;
                    case "Кубок БФО по спринтам":
                        ratingId = db.Ratings.Single(r => r.Name == "Кубок БФО по спринтам").Id;
                        ratingTable = GetRating(year.Value, ratingId);
                        break;
                    case "Кубок БФО среди ветеранов":
                        ratingId = db.Ratings.Single(r => r.Name == "Кубок БФО среди ветеранов").Id;
                        ratingTable = GetRating(year.Value, ratingId);
                        break;
                    default:
                        ratingId = db.Ratings.Single(r => r.Name == "Кубок БФО").Id;
                        ratingTable = GetRating(year.Value, ratingId);
                        break;
                }
            }
            catch (Exception)
            {
                return View("Error");
            }
            
            ViewBag.RatingTable = ratingTable;

            return View();
        }

        private List<RatingViewModels> GetRating(int year, int ratingId)
        {
            List<RatingViewModels> eliteCompetitions = db.Competitions.Where(c => c.Ratings.Any(r => r.Id == ratingId) && c.Date.Contains(year.ToString())).Select(c => new RatingViewModels()
            {
                CompetitionId = c.Id,
                Name = c.Name,
                Date = c.Date,
                Kind = c.Kind,
                Score = 0.0
            }).ToList();

            //List<RatingViewModels> ratingList = new List<RatingViewModels>();
            foreach (var competition in eliteCompetitions)
            {
                GetScore(competition);
            }

            // sort table by total rating
            List<RatingViewModels> sortedEliteCompetitions = eliteCompetitions.OrderByDescending(x => x.Score).ToList();
                
            return sortedEliteCompetitions;
        }

        private bool GetScore(RatingViewModels competition)
        {
            var results = from v in db.Votes
                          join g in db.Groups on v.GroupID equals g.Id
                          where v.CompetitionID == competition.CompetitionId
                          select new RatingVoteModel()
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

            var groupType = db.Competitions.Where(x => x.Id == competition.CompetitionId).SelectMany(x => x.Groups);

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
                RatingTableModel men = new RatingTableModel();
                RatingTableModel woman = new RatingTableModel();
                if (SplitByAge(results, men, woman) == false)
                {
                    return false;
                }

                competition.Score = GetTotalForCompetition(men, woman);
            }

            // Type: M10, W10, M21E, e.t.c.
            if (groupExample.Length > 2)
            {
                RatingTableModel resultsUnder21 = new RatingTableModel();
                RatingTableModel results21 = new RatingTableModel();
                RatingTableModel resultOver21 = new RatingTableModel();

                if (SplitByAge(results, resultsUnder21, results21, resultOver21) == false)
                {
                    return false;
                }

                competition.Score = GetTotalForCompetition(resultsUnder21, results21, resultOver21);
            }

            return true;
        }

        private bool SplitByAge(IQueryable<RatingVoteModel> results, RatingTableModel men, RatingTableModel woman)
        {
            foreach (var vote in results)
            {
                if (vote.GroupName[0] == 'М')
                {
                    addToResultTable(vote, men);
                }

                if (vote.GroupName[0] == 'Ж')
                {
                    addToResultTable(vote, woman);
                }
            }

            getMiddle(men);
            getMiddle(woman);

            return true;
        }

        private bool SplitByAge(IQueryable<RatingVoteModel> results, RatingTableModel resultsUnder21, RatingTableModel results21, RatingTableModel resultOver21)
        {
            foreach (var vote in results)
            {
                // example: W21E -> 21
                int age = Convert.ToInt32(vote.GroupName.Substring(1, 2));
                if (age < 21)
                {
                    addToResultTable(vote, resultsUnder21);
                }
                if (age == 21)
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

        private void addToResultTable(RatingVoteModel vote, RatingTableModel result)
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

        private void getMiddle(RatingTableModel table)
        {
            if (table.CountOfVoters != 0)
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


        private double GetTotalForCompetition(RatingTableModel resultsUnder21, RatingTableModel results21, RatingTableModel resultOver21)
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

        private double GetTotalForCompetition(RatingTableModel men, RatingTableModel woman)
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

        private List<string> GetListOfYears()
        {
            // get dates from all competitions
            var years = db.Competitions.Select(competition => competition.Date);

            // get all years from dates
            List<string> listOfYears = new List<string>();

            foreach (var item in years)
            {
                string date = item.Substring(item.LastIndexOf('.') + 1);

                if (listOfYears.Contains(date) == false)
                {
                    listOfYears.Add(date);
                }
            }

            return listOfYears;
        }
    }
}
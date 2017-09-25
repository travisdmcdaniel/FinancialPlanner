using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using System.Data.Entity;
using Newtonsoft.Json;

namespace FinancialPlanner.Controllers
{
    public class HomeController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();

        [Authorize]
        public ActionResult Index()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var dashboard = new DashboardViewModel();
            dashboard.Accounts = db.Accounts.AsNoTracking().Where(a => a.HouseholdId == user.HouseholdId).Include(a => a.Transactions).Include(a => a.Deposits).ToList();
            dashboard.Budgets = db.Budgets.AsNoTracking().Where(b => b.HouseholdId == user.HouseholdId && b.Deleted != true).Include(b => b.BudgetItems).ToList();
            dashboard.Budgets = dashboard.Budgets.Where(b => b.Month == DateTime.Now.Month && b.Year == DateTime.Now.Year).ToList();
            if (user.Household != null)
            {
                dashboard.Household = user.Household;
            }
            dashboard.Invitations = db.Invitations.AsNoTracking().Where(i => i.HouseholdId == user.HouseholdId).ToList();
            dashboard.Members = db.Users.Where(u => u.HouseholdId == user.HouseholdId).ToList();
            dashboard.BudgetItems = new List<BudgetItem>();
            dashboard.Deposits = new List<Deposit>();
            dashboard.Transactions = new List<Transaction>();
            foreach (var item in dashboard.Budgets)
            {
                foreach (var bi in db.BudgetItems.Where(bi => bi.BudgetId == item.Id))
                {
                    if (bi.Deleted != true)
                    {
                        dashboard.BudgetItems.Add(bi);
                    }
                }
            }
            foreach (var item in dashboard.Accounts)
            {
                foreach (var dep in db.Deposits.Where(d => d.AccountId == item.Id))
                {
                    dashboard.Deposits.Add(dep);
                }
                foreach (var trans in db.Transactions.Where(t => t.AccountId == item.Id))
                {
                    dashboard.Transactions.Add(trans);
                }
            }
            dashboard.Deposits.OrderByDescending(d => d.Date.Date);
            dashboard.Transactions.OrderByDescending(t => t.Date.Date);
            return View(dashboard);
        }

        [Authorize]
        [NoDirectAccess]
        [HttpPost]
        public ActionResult RepopulateBudgets(int householdId)
        {
            var household = db.Households.Find(householdId);
            var date = DateTime.Now.AddMonths(-1);
            var budgets = household.Budgets.Where(b => b.Monthly == true && b.Deleted != true).ToList();
            foreach (var item in budgets)
            {
                if (DateTime.Now.Month == 1)
                {
                    if (!(item.Month == 12 && item.Year == DateTime.Now.AddYears(-1).Year))
                    {
                        budgets.Remove(item);
                    }
                }
                else
                {
                    if (!(item.Month == DateTime.Now.AddMonths(-1).Month && item.Year == DateTime.Now.Year))
                    {
                        budgets.Remove(item);
                    }
                }
            }
            foreach (var item in budgets)
            {
                Budget budget = new Budget { Name = item.Name, Description = item.Description, AmountBudgeted = item.AmountBudgeted, Date = DateTime.Now, Month = DateTime.Now.Month, Year = DateTime.Now.Year, Monthly = item.Monthly, HouseholdId = item.HouseholdId };
                db.Budgets.Add(budget);
                db.SaveChanges();
                foreach (var budgetItem in budget.BudgetItems.Where(bi => bi.Deleted != true))
                {
                    BudgetItem newBudgetItem = new BudgetItem { Name = budgetItem.Name, BudgetId = budget.Id, Month = DateTime.Now.Month, Year = DateTime.Now.Year, Date = DateTime.Now };
                    db.BudgetItems.Add(newBudgetItem);
                    db.SaveChanges();
                }
            }
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [NoDirectAccess]
        [HttpPost]
        public ActionResult RemoveUserFromHousehold(string userId)
        {
            ApplicationUser user = db.Users.Find(userId);
            user.HouseholdId = null;
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }

        [Authorize]
        [NoDirectAccess]
        public ActionResult AllAccountTransactions(int accountId)
        {
            AllTransactionsVM allTransactions = new AllTransactionsVM();
            Account account = db.Accounts.Include(a => a.Transactions).Include(a => a.Deposits).FirstOrDefault(a => a.Id == accountId);
            allTransactions.AccountName = account.Name;
            allTransactions.Deposits = account.Deposits.OrderBy(d => d.Date).ToList();
            allTransactions.Transactions = account.Transactions.OrderBy(t => t.Date).ToList();
            return View(allTransactions);
        }

        public class BudgetChartData
        {
            public string Name { get; set; }
            public decimal TargetAmt { get; set; }
            public decimal ActualAmt { get; set; }
        }

        public class AccountChartData
        {
            public string Name { get; set; }
            public decimal AmountSpent { get; set; }
            public DateTime TransactionDate { get; set; }
        }

        [NoDirectAccess]
        public ActionResult GetBudgetChartData()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var chartData = new List<BudgetChartData>();
            foreach (var item in db.Budgets.Where(b => b.HouseholdId == user.HouseholdId))
            {
                chartData.Add(new BudgetChartData { Name = item.Name, TargetAmt = item.AmountBudgeted, ActualAmt = item.AmountAgainst });
            }
            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        [NoDirectAccess]
        public ActionResult GetAccountChartData()
        {
            var user = db.Users.Find(User.Identity.GetUserId());
            var chartData = new List<AccountChartData>();
            AccountChartData tempData = new AccountChartData();
            List<AccountHistory> tempHistory = new List<AccountHistory>();
            foreach (var item in db.Accounts.AsNoTracking().Include(a => a.Transactions).Include(a => a.Deposits).Where(b => b.HouseholdId == user.HouseholdId).ToList())
            {
                if (db.AccountHistories.Where(ah => ah.AccountId == item.Id) != null && item.Deleted != true)
                {
                    foreach (var tItem in db.AccountHistories.Where(ah => ah.AccountId == item.Id))
                    {
                        tempHistory.Add(tItem);
                    }
                }
            }
            tempHistory.OrderBy(th => th.TransactionDate);
            foreach (var transaction in tempHistory)
            {
                if (transaction.TransactionId != null && transaction.TransactionDate.Month == DateTime.Now.Month)
                {
                    chartData.Add(new AccountChartData { AmountSpent = transaction.Transaction.Amount, TransactionDate = transaction.TransactionDate, Name = transaction.Transaction.BudgetItem.Name });
                }
            }
            chartData.OrderBy(c => c.TransactionDate);
            return Content(JsonConvert.SerializeObject(chartData), "application/json");
        }

        [NoDirectAccess]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        [NoDirectAccess]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
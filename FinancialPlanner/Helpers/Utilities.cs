using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Helpers
{
    public static class Utilities
    {
        private static UserManager<ApplicationUser> userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static string GetUserName(string userId)
        {
            ApplicationUser user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return "Guest User";
            }
            else
            {
                return user.FullName;
            }
        }

        public static string GetHouseholdName(int? Id)
        {
            if (Id == null)
            {
                return "Household Name";
            }
            else
            {
                return db.Households.Find(Id).Name;
            }
        }

        public static bool IsUserInAHousehold(string userId)
        {
            ApplicationUser user = new ApplicationUser();
            //ApplicationUser user = db.Users.Find(userId);
            //var user1 = db.Entry(user).GetDatabaseValues();
            user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            if (user.HouseholdId == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static Household GetUsersHousehold(string userId)
        {
            if (IsUserInAHousehold(userId))
            {
                ApplicationUser user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
                Household household = db.Households.Find(user.HouseholdId);
                return household;
            }
            else
            {
                return null;
            }
        }

        public static decimal getTrueTransactionAmount(int transactionId)
        {
            Transaction transaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == transactionId);
            if (transaction.ReconciledAmount > 0)
            {
                return transaction.ReconciledAmount;
            }
            else
            {
                return transaction.Amount;
            }
        }

        public static decimal getTrueDepositAmount(int depositId)
        {
            Deposit deposit = db.Deposits.AsNoTracking().FirstOrDefault(d => d.Id == depositId);
            if (deposit.ReconciledAmount > 0)
            {
                return deposit.ReconciledAmount;
            }
            else
            {
                return deposit.Amount;
            }
        }

        public static string GenHouseholdKey()
        {
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            var stringChars = new char[9];
            var random = new Random();

            for (int i = 0; i < stringChars.Length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            var key = new string(stringChars);
            return key;
        }

        public static void AddUserToRole(string userId, string role)
        {
            var roles = userManager.GetRoles(userId);
            userManager.RemoveFromRoles(userId, roles.ToArray());

            var result = userManager.AddToRole(userId, role);

            if (result.Succeeded)
            {
                var authenticationManager = HttpContext.Current.GetOwinContext().Authentication;
                authenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                var user = userManager.FindById(userId);
                var identity = userManager.CreateIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);

                authenticationManager.SignIn(new AuthenticationProperties { IsPersistent = false }, identity);
            }
        }

        public static List<ApplicationUser> GetMembersOfUsersHousehold(string userId)
        {
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            var householdId = user.HouseholdId;
            var household = db.Households.AsNoTracking().FirstOrDefault(h => h.Id == householdId);
            var list = new List<ApplicationUser>();
            list = household.Users.ToList();
            return list;
        }

        public static int GetNumberOfUsersTransactions(string userId)
        {
            if (db.Transactions.Where(t => t.EnteredById == userId) != null)
            {
                return db.Transactions.Where(t => t.EnteredById == userId).Count();
            }
            else
            {
                return 0;
            }
        }

        public static ICollection<string> ListUserRoles(string userId)
        {
            return userManager.GetRoles(userId);

        }

        public static bool IsUserInRole(string userId, string role)
        {
            return userManager.IsInRole(userId, role);
        }

        public static int GetNumberOfUsersInHousehold(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Households.Find(householdId).Users == null)
            { return 0; }
            else
            { return db.Households.Find(householdId).Users.Count(); }
        }

        public static int GetNumberOfHouseholdAccounts(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Households.Find(householdId).Accounts == null)
            { return 0; }
            else
            {
                List<Account> aList = new List<Account>();
                aList = db.Households.Find(householdId).Accounts.ToList();
                int count = 0;
                foreach (var account in aList)
                {
                    if (!account.Deleted)
                    {
                        count++;
                    }
                }
                return count;
            }
        }

        public static bool DoesHouseholdHaveBudgetsForCurrentMonth(int householdId)
        {
            var household = db.Households.Find(householdId);
            //var budgets = household.Budgets.Where(b => b.Month == DateTime.Now.Month && b.Year == DateTime.Now.Year);
            if (household.Budgets.Where(b => b.Month == DateTime.Now.Month && b.Year == DateTime.Now.Year && b.Deleted != true).Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DoesHouseholdHaveBudgets(int householdId)
        {
            var household = db.Households.Find(householdId);
            if (household.Budgets.Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static bool DoesHouseholdHaveRepeatableBudgets(int householdId)
        {
            var household = db.Households.Find(householdId);
            var budgets = household.Budgets;
            if (budgets.Where(b => b.Monthly == true).Count() == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static decimal GetHouseholdBalance(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            else
            {
                Household household = db.Households.AsNoTracking().FirstOrDefault(h => h.Id == householdId);
                decimal tempData = new decimal();
                foreach (var account in household.Accounts)
                {
                    if (account.Deleted != true)
                    {
                        tempData += account.Balance;
                    }
                }
                household = db.Households.Find(householdId);
                household.Balance = tempData;
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return tempData;
            }
        }

        //public static bool IsUserInRole(string userId, string roleName)
        //{
        //    return userManager.IsInRole(userId, roleName);
        //}

        public static int GetNumberOfHouseholdInvitations(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Invitations.Where(i => i.HouseholdId == householdId) == null)
            { return 0; }
            else
            { return db.Invitations.Where(i => i.HouseholdId == householdId).Count(); }
        }

        public static int GetNumberOfHouseholdInvitationsAccepted(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Invitations.Where(i => i.HouseholdId == householdId) == null)
            { return 0; }
            else
            {
                var list = db.Invitations.Where(i => i.HouseholdId == householdId);
                list = list.Where(i => i.Accepted == true);
                return list.Count();
            }
        }

        public static List<Budget> GetCurrentBudgetsForUsersHousehold(string userId)
        {
            var list = new List<Budget>();
            var user = db.Users.Find(userId);
            list = db.Budgets.Where(b => b.HouseholdId == user.HouseholdId && b.Month == DateTime.Now.Month && b.Year == DateTime.Now.Year).ToList();
            return list;
        }

        public static List<Transaction> GetRecentTransactions(int accountId)
        {
            Account account = db.Accounts.AsNoTracking().Include(a => a.Transactions).FirstOrDefault(a => a.Id == accountId);
            List<Transaction> tList = new List<Transaction>();
            if (account.Transactions.Count() == 0)
            {
                return null;
            }
            if (account.Transactions.Count() <= 5)
            {
                return account.Transactions.Reverse().ToList();
            }
            else
            {
                List<Transaction> fullList = new List<Transaction>();
                fullList = account.Transactions.ToList();
                fullList.Reverse();
                for (int n = 0; n < 5; n++)
                {
                    tList.Add(fullList[n]);
                }
                //tList.Reverse();
                return tList;
            }
        }

        public static List<Deposit> GetRecentDeposits(int accountId)
        {
            Account account = db.Accounts.AsNoTracking().Include(a => a.Deposits).FirstOrDefault(a => a.Id == accountId);
            List<Deposit> tList = new List<Deposit>();
            if (account.Deposits.Count() == 0)
            {
                return null;
            }
            if (account.Transactions.Count() <= 5)
            {
                return account.Deposits.Reverse().ToList();
            }
            else
            {
                List<Deposit> fullList = new List<Deposit>();
                fullList = account.Deposits.ToList();
                fullList.Reverse();
                for (int n = 0; n < 5; n++)
                {
                    tList.Add(fullList[n]);
                }
                //tList.Reverse();
                return tList;
            }
        }

        public static string GetUsersDisplaySettings(string userId)
        {
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            if (user.DisplaySetting == null)
            {
                return "panel-default";
            }
            else
            {
                return user.DisplaySetting;
            }
        }

        public static decimal GetUsersBalanceWarning(string userId)
        {
            var user = db.Users.AsNoTracking().FirstOrDefault(u => u.Id == userId);
            if (user.BalanceWarning > 0)
            {
                return user.BalanceWarning;
            }
            else
            {
                return 25.00m;
            }
        }

        public static string GetTransactionDate(int id)
        {
            Transaction transaction = db.Transactions.AsNoTracking().FirstOrDefault(t => t.Id == id);
            string date = string.Empty;
            switch (transaction.Date.Month)
            {
                case 1:
                    date = "January" + " " + transaction.Date.Year.ToString();
                    break;
                case 2:
                    date = "February" + " " + transaction.Date.Year.ToString();
                    break;
                case 3:
                    date = "March" + " " + transaction.Date.Year.ToString();
                    break;
                case 4:
                    date = "April" + " " + transaction.Date.Year.ToString();
                    break;
                case 5:
                    date = "May" + " " + transaction.Date.Year.ToString();
                    break;
                case 6:
                    date = "June" + " " + transaction.Date.Year.ToString();
                    break;
                case 7:
                    date = "July" + " " + transaction.Date.Year.ToString();
                    break;
                case 8:
                    date = "August" + " " + transaction.Date.Year.ToString();
                    break;
                case 9:
                    date = "September" + " " + transaction.Date.Year.ToString();
                    break;
                case 10:
                    date = "October" + " " + transaction.Date.Year.ToString();
                    break;
                case 11:
                    date = "November" + " " + transaction.Date.Year.ToString();
                    break;
                case 12:
                    date = "December" + " " + transaction.Date.Year.ToString();
                    break;
            }
            return date;
        }

        public static string GetBudgetDate(int id)
        {
            Budget budget = db.Budgets.AsNoTracking().FirstOrDefault(t => t.Id == id);
            string date = string.Empty;
            switch (budget.Date.Month)
            {
                case 1:
                    date = "January" + " " + budget.Date.Year.ToString();
                    break;
                case 2:
                    date = "February" + " " + budget.Date.Year.ToString();
                    break;
                case 3:
                    date = "March" + " " + budget.Date.Year.ToString();
                    break;
                case 4:
                    date = "April" + " " + budget.Date.Year.ToString();
                    break;
                case 5:
                    date = "May" + " " + budget.Date.Year.ToString();
                    break;
                case 6:
                    date = "June" + " " + budget.Date.Year.ToString();
                    break;
                case 7:
                    date = "July" + " " + budget.Date.Year.ToString();
                    break;
                case 8:
                    date = "August" + " " + budget.Date.Year.ToString();
                    break;
                case 9:
                    date = "September" + " " + budget.Date.Year.ToString();
                    break;
                case 10:
                    date = "October" + " " + budget.Date.Year.ToString();
                    break;
                case 11:
                    date = "November" + " " + budget.Date.Year.ToString();
                    break;
                case 12:
                    date = "December" + " " + budget.Date.Year.ToString();
                    break;
            }
            return date;
        }

        public static int GetNumberOfHouseholdBudgets(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Households.Find(householdId).Budgets == null)
            { return 0; }
            else
            //{ return db.Households.Find(householdId).Budgets.Count(); }
            {
                var budgets = db.Households.Find(householdId).Budgets.ToList();
                List<Budget> removeList = new List<Budget>();
                foreach (var item in budgets)
                {
                    if (item.Deleted == true)
                    {
                        removeList.Add(item);
                    }
                }
                foreach (var item in budgets)
                {
                    if (item.Year != DateTime.Now.Year && item.Month != DateTime.Now.Month)
                    {
                        removeList.Add(item);
                    }
                }
                budgets.RemoveAll(x => !removeList.Any(y => y.Id == x.Id));
                return budgets.Count();
            }
        }

        public static int GetNumberOfHouseholdBudgetItems(int householdId)
        {
            if (db.Households.Find(householdId) == null)
            { return 0; }
            if (db.Households.Find(householdId).Budgets == null)
            { return 0; }
            //List<Budget> list = db.Budgets.Where(b => b.HouseholdId == householdId).ToList();
            //int count = 0;
            //foreach (var item in list)
            //{
            //    foreach (var bi in item.BudgetItems)
            //    {
            //        count++;
            //    }
            //}
            //return count;
            else
            {
                var budgets = db.Households.Find(householdId).Budgets.ToList();
                List<Budget> removeList = new List<Budget>();
                foreach (var item in budgets)
                {
                    if (item.Deleted == true)
                    {
                        removeList.Add(item);
                    }
                    else if (item.Year != DateTime.Now.Year && item.Month != DateTime.Now.Month)
                    {
                        removeList.Add(item);
                    }
                }
                budgets.RemoveAll(x => !removeList.Any(y => y.Id == x.Id));
                int count = 0;
                foreach (var item in budgets)
                {
                    foreach (var bi in item.BudgetItems)
                    {
                        count++;
                    }
                }
                return count;
            }
        }
    }
}
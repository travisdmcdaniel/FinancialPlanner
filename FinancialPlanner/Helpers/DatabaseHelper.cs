using FinancialPlanner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace FinancialPlanner.Helpers
{
    public static class DatabaseHelper
    {
        private static ApplicationDbContext db = new ApplicationDbContext();

        public static void GenerateTransactions(int accountId)
        {
            int MAXTDOLLAR = 101; //Set to 101 so that it can be used as the exclusive maxvalue in rnd.Next(minvalue, maxvalue).  The Actual Max Dollar Amount will be 100.
            int MAXDDOLLAR = 1001;
            int MINTDOLLAR = 0;
            int MINDDOLLAR = 500;
            Account account = db.Accounts.Find(accountId);
            Household household = db.Households.AsNoTracking().FirstOrDefault(h => h.Id == account.HouseholdId);
            List<Budget> budgets = db.Budgets.Where(b => b.HouseholdId == account.HouseholdId && b.Deleted != true).ToList();
            int BudgetCount = budgets.Count();
            Dictionary<Budget, List<int>> BudgetItemsDictionary = new Dictionary<Budget, List<int>>();
            Dictionary<int, List<decimal>> BudgetsDictionary = new Dictionary<int, List<decimal>>();
            foreach (var item in budgets)
            {
                List<decimal> tempList = new List<decimal>();
                tempList.Add(item.AmountAgainst);
                tempList.Add(item.AmountBudgeted);
                BudgetsDictionary.Add(item.Id, tempList);
                List<int> biTempList = new List<int>();
                foreach (var bi in item.BudgetItems)
                {
                    biTempList.Add(bi.Id);
                }
                BudgetItemsDictionary.Add(item, biTempList);
            }
            Random rnd = new Random();
            for (int n = 30; n >= 0; n--)
            {
                if (DateTime.Now.AddDays(-n).Month == DateTime.Now.Month)
                {
                    decimal cents = (decimal)rnd.Next(0, 100) / 100m;
                    decimal tDollars = rnd.Next(MINTDOLLAR, MAXTDOLLAR);
                    decimal dDollars = rnd.Next(MINDDOLLAR, MAXDDOLLAR);
                    AccountHistory accountHistory = new AccountHistory();
                    if (account.Balance < 100m)
                    {
                        Deposit deposit = new Deposit();
                        deposit.Amount = dDollars + cents;
                        deposit.AccountId = accountId;
                        deposit.EnteredById = HttpContext.Current.User.Identity.GetUserId();
                        deposit.Date = DateTime.Now.AddDays(-n);
                        deposit.Month = deposit.Date.Month;
                        deposit.Year = deposit.Date.Year;
                        deposit.Source = "Paycheck";
                        db.Deposits.Add(deposit);
                        db.SaveChanges();
                        accountHistory.AccountId = accountId;
                        accountHistory.AccountBeginBalance = account.Balance;
                        account.Balance = account.Balance + deposit.Amount;
                        db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        accountHistory.AccountEndBalance = account.Balance;
                        accountHistory.TransactionDate = deposit.Date;
                        accountHistory.DepositId = deposit.Id;
                        db.AccountHistories.Add(accountHistory);
                        db.SaveChanges();
                    }
                    else
                    {
                        Transaction transaction = new Transaction();
                        transaction.Amount = tDollars + cents;
                        int budgetChoice = rnd.Next(0, BudgetItemsDictionary.Count());
                        List<int> budgetItemList = new List<int>();
                        BudgetItemsDictionary.TryGetValue(budgets[budgetChoice], out budgetItemList);
                        var budgetItemChoice = budgetItemList[rnd.Next(0, budgetItemList.Count())];
                        BudgetItem budgetItem = db.BudgetItems.Find(budgetItemChoice);
                        Budget budget = db.Budgets.Find(budgetItem.BudgetId);
                        budget.AmountAgainst += transaction.Amount;
                        db.Entry(budget).State = System.Data.Entity.EntityState.Modified;
                        transaction.BudgetItemId = budgetItemChoice;
                        transaction.AccountId = accountId;
                        transaction.EnteredById = HttpContext.Current.User.Identity.GetUserId();
                        transaction.Date = DateTime.Now.AddDays(-n);
                        transaction.Year = transaction.Date.Year;
                        transaction.Month = transaction.Date.Month;
                        db.Transactions.Add(transaction);
                        db.SaveChanges();
                        accountHistory.TransactionId = transaction.Id;
                        accountHistory.AccountId = accountId;
                        accountHistory.AccountBeginBalance = account.Balance;
                        account.Balance = account.Balance - transaction.Amount;
                        db.Entry(account).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                        accountHistory.AccountEndBalance = account.Balance;
                        accountHistory.TransactionDate = transaction.Date;
                        db.AccountHistories.Add(accountHistory);
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}
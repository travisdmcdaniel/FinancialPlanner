using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Configuration;
using System.Net.Mail;
using FinancialPlanner.Helpers;
using System.Web.Security;

namespace FinancialPlanner.Controllers
{
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        [NoDirectAccess]
        public ActionResult Index()
        {
            var transactions = db.Transactions.Include(t => t.Account).Include(t => t.BudgetItem).Include(t => t.EnteredBy);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create([Bind(Include = "Id,Amount")] Transaction TransactionToCreate, int id, int ddList)
        {
            TransactionToCreate.AccountId = id;
            TransactionToCreate.EnteredById = User.Identity.GetUserId();
            TransactionToCreate.BudgetItemId = ddList;
            BudgetItem budgetItem = db.BudgetItems.Find(ddList);
            if (ModelState.IsValid)
            {
                TransactionToCreate.Date = DateTime.Now;
                TransactionToCreate.Month = DateTime.Now.Month;
                TransactionToCreate.Year = DateTime.Now.Year;
                db.Transactions.Add(TransactionToCreate);
                db.SaveChanges();
                Account account = db.Accounts.Find(id);
                account.Balance = account.Balance - TransactionToCreate.Amount;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                if (account.Balance < 0.00m && account.OverDraftFee != null)
                {
                    Transaction odTransaction = new Transaction();
                    odTransaction.BudgetItemId = db.BudgetItems.FirstOrDefault(bi => bi.Name == "Abacus - Overdraft Fee").Id;
                    odTransaction.Amount = (decimal)account.OverDraftFee;
                    odTransaction.EnteredById = User.Identity.GetUserId();
                    odTransaction.Date = DateTime.Now;
                    odTransaction.Month = DateTime.Now.Month;
                    odTransaction.Year = DateTime.Now.Year;
                    odTransaction.AccountId = account.Id;
                    db.Transactions.Add(odTransaction);
                    db.SaveChanges();
                    AccountHistory odHistory = new AccountHistory();
                    odHistory.TransactionId = odTransaction.Id;
                    odHistory.AccountId = account.Id;
                    odHistory.TransactionAmount = odTransaction.Amount;
                    odHistory.TransactionDate = odTransaction.Date;
                    odHistory.AccountBeginBalance = account.Balance;
                    account.Balance = account.Balance - odTransaction.Amount;
                    account.Updated = DateTime.Now;
                    db.Entry(account).State = EntityState.Modified;
                    db.SaveChanges();
                    odHistory.AccountEndBalance = account.Balance;
                }
                Budget budget = db.Budgets.Find(budgetItem.BudgetId);
                budget.AmountAgainst += TransactionToCreate.Amount;
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                AccountHistory accountHistory = new AccountHistory();
                accountHistory.AccountId = id;
                accountHistory.TransactionId = TransactionToCreate.Id;
                accountHistory.TransactionAmount = TransactionToCreate.Amount;
                accountHistory.TransactionDate = DateTime.Now;
                accountHistory.AccountBeginBalance = account.Balance + TransactionToCreate.Amount;
                accountHistory.AccountEndBalance = account.Balance - TransactionToCreate.Amount;
                db.AccountHistories.Add(accountHistory);
                db.SaveChanges();
                ApplicationUser sendNotificationTo = new ApplicationUser();
                bool sendOverdraftNotification = false;
                bool sendWarningNotification = false;
                var users = db.Users.Where(u => u.HouseholdId == account.HouseholdId);
                foreach (var user in users)
                {
                    if (Utilities.IsUserInRole(user.Id, "Head Of Household"))
                    {
                        if (account.Balance < 0.00m)
                        {
                            sendOverdraftNotification = true;
                            sendNotificationTo = user;
                        }
                        else if (account.Balance < Utilities.GetUsersBalanceWarning(user.Id))
                        {
                            sendWarningNotification = true;
                            sendNotificationTo = user;
                        }
                    }
                }
                if (sendOverdraftNotification)
                {
                    Overdraft(sendNotificationTo.Id);
                }
                if (sendWarningNotification)
                {
                    Warning(sendNotificationTo.Id);
                }
                return RedirectToAction("Index", "Home");
            }

            //ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return RedirectToAction("Index", "Home");
        }

        [NoDirectAccess]
        public ActionResult GenerateTransactions(int accountId)
        {
            DatabaseHelper.GenerateTransactions(accountId);
            return RedirectToAction("Index", "Home");
        }

        // GET: Transactions/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Edit([Bind(Include = "Id,AccountId,CategoryId,EnteredById,Type,Amount,ReconciledAmount")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            //ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", transaction.AccountId);
            //ViewBag.CategoryId = new SelectList(db.Categories, "Id", "Name", transaction.CategoryId);
            //ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", transaction.EnteredById);
            return View(transaction);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Reconcile(int accountId, int transactionId, decimal amount, decimal reconciled)
        {
            Transaction transaction = db.Transactions.Find(transactionId);
            if (transaction.ReconciledAmount > 0)
            {
                Account account = db.Accounts.Find(accountId);
                account.Balance += transaction.ReconciledAmount;
                account.Balance -= reconciled;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                transaction.ReconciledAmount = reconciled;
                db.Entry(transaction).State = EntityState.Modified;
                db.SaveChanges();
                ApplicationUser sendNotificationTo = new ApplicationUser();
                bool sendOverdraftNotification = false;
                bool sendWarningNotification = false;
                var users = db.Users.Where(u => u.HouseholdId == account.HouseholdId);
                foreach (var user in users)
                {
                    if (Utilities.IsUserInRole(user.Id, "Head Of Household"))
                    {
                        if (account.Balance < 0.00m)
                        {
                            sendOverdraftNotification = true;
                            sendNotificationTo = user;
                        }
                        else if (account.Balance < Utilities.GetUsersBalanceWarning(user.Id))
                        {
                            sendWarningNotification = true;
                            sendNotificationTo = user;
                        }
                    }
                }
                if (sendOverdraftNotification)
                {
                    Overdraft(sendNotificationTo.Id);
                }
                if (sendWarningNotification)
                {
                    Warning(sendNotificationTo.Id);
                }
                //if (User.IsInRole("Head Of Household"))
                //{
                //    if (account.Balance < Utilities.GetUsersBalanceWarning(User.Identity.GetUserId()))
                //    {
                //        await Warning(User.Identity.GetUserId());
                //    }
                //}
            }
            else
            {
                transaction.ReconciledAmount = reconciled;
                db.Entry(transaction).State = EntityState.Modified;
                Account account = db.Accounts.Find(accountId);
                account.Balance += amount;
                account.Balance -= reconciled;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                ApplicationUser sendNotificationTo = new ApplicationUser();
                bool sendOverdraftNotification = false;
                bool sendWarningNotification = false;
                var users = db.Users.Where(u => u.HouseholdId == account.HouseholdId);
                foreach (var user in users)
                {
                    if (Utilities.IsUserInRole(user.Id, "Head Of Household"))
                    {
                        if (account.Balance < 0.00m)
                        {
                            sendOverdraftNotification = true;
                            sendNotificationTo = user;
                        }
                        else if (account.Balance < Utilities.GetUsersBalanceWarning(user.Id))
                        {
                            sendWarningNotification = true;
                            sendNotificationTo = user;
                        }
                    }
                }
                if (sendOverdraftNotification)
                {
                    Overdraft(sendNotificationTo.Id);
                }
                if (sendWarningNotification)
                {
                    Warning(sendNotificationTo.Id);
                }
                //var user = users.FirstOrDefault(u => u.Roles.)
                //if (User.IsInRole("Head Of Household"))
                //{

                //}
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Transactions/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpPost]
        [Authorize]
        [NoDirectAccess]
        public async Task oldWarning(string userId)
        {
            var user = db.Users.Find(userId);
            IdentityMessage message = new IdentityMessage();
            //var callbackUrl = Url.Action("InvitationRegister", "Account", new { household = inviteViewModel.HouseholdId, code = code }, protocol: Request.Url.Scheme);
            message.Destination = user.Email;
            string LowAccounts = string.Empty;
            foreach (var item in db.Accounts)
            {
                if (item.Deleted == false && item.HouseholdId == user.HouseholdId)
                {
                    if (item.Balance <= Utilities.GetUsersBalanceWarning(User.Identity.GetUserId()))
                    {
                        LowAccounts = LowAccounts + "Account \"" + item.Name + "\" has a balance of " + string.Format("{0:C2}", @item.Balance) + "\n";
                    }
                }
            }
            message.Subject = "Account Balance Warning";
            message.Body = "This is an automated message from the Abacus Financial Planner.  One or more of your accounts has dropped below your warning threshold.  " +
                "The accounts with balances below the warning threshold are listed below:\n" + LowAccounts;
            await SendWarningAsync(message);
        }

        public void Warning(string userId)
        {
            var user = db.Users.Find(userId);
            string LowAccounts = string.Empty;
            foreach (var item in db.Accounts)
            {
                if (item.Deleted == false && item.HouseholdId == user.HouseholdId)
                {
                    if (item.Balance <= Utilities.GetUsersBalanceWarning(User.Identity.GetUserId()))
                    {
                        LowAccounts = LowAccounts + "Account \"" + item.Name + "\" has a balance of " + string.Format("{0:C2}", @item.Balance) + "\n";
                    }
                }
            }
            MailMessage message = new MailMessage();
            message.From = new MailAddress("Abacus Financial Planner <abacus@travismcdaniel.me>");
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "Account Balance Warning";
            message.Body = "This is an automated message from the Abacus Financial Planner.  One or more of your accounts has dropped below your warning threshold.  " +
                "The accounts with balances below the warning threshold are listed below:\n" + LowAccounts;
            SmtpClient client = new SmtpClient();
            client.Send(message);
        }

        [HttpPost]
        [Authorize]
        [NoDirectAccess]
        public async Task oldOverdraft(string userId)
        {
            var user = db.Users.Find(userId);
            IdentityMessage message = new IdentityMessage();
            //var callbackUrl = Url.Action("InvitationRegister", "Account", new { household = inviteViewModel.HouseholdId, code = code }, protocol: Request.Url.Scheme);
            message.Destination = user.Email;
            string LowAccounts = string.Empty;
            foreach (var item in db.Accounts)
            {
                if (item.Deleted == false && item.HouseholdId == user.HouseholdId)
                {
                    if (item.Balance <= Utilities.GetUsersBalanceWarning(User.Identity.GetUserId()))
                    {
                        LowAccounts = LowAccounts + "Account \"" + item.Name + "\" has a balance of " + string.Format("{0:C2}", @item.Balance) + "\n";
                    }
                }
            }
            message.Subject = "Account Overdraft Warning";
            message.Body = "This is an automated message from the Abacus Financial Planner.  One or more of your accounts has been overdrafted.  " +
                "The accounts that are overdrafted are listed below:\n" + LowAccounts;
            await SendWarningAsync(message);
        }

        public void Overdraft(string userId)
        {
            var user = db.Users.Find(userId);
            string LowAccounts = string.Empty;
            foreach (var item in db.Accounts)
            {
                if (item.Deleted == false && item.HouseholdId == user.HouseholdId)
                {
                    if (item.Balance <= 0)
                    {
                        LowAccounts = LowAccounts + "Account \"" + item.Name + "\" has a balance of " + string.Format("{0:C2}", @item.Balance) + "\n";
                    }
                }
            }
            MailMessage message = new MailMessage();
            message.From = new MailAddress("Abacus Financial Planner <abacus@travismcdaniel.me>");
            message.To.Add(new MailAddress(user.Email));
            message.Subject = "Account Overdraft Warning";
            message.Body = "This is an automated message from the Abacus Financial Planner.  One or more of your accounts has been overdrafted.  " +
                "The overdrafted accounts are listed below:\n" + LowAccounts;
            SmtpClient client = new SmtpClient();
            client.Send(message);

        }

        [NoDirectAccess]
        public async Task SendWarningAsync(IdentityMessage message)
        {
            var GmailUsername = WebConfigurationManager.AppSettings["username"];
            var GmailPassword = WebConfigurationManager.AppSettings["password"];
            var host = WebConfigurationManager.AppSettings["host"];
            int port = Convert.ToInt32(WebConfigurationManager.AppSettings["port"]);

            using (var smtp = new SmtpClient()
            {
                Host = host,
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(GmailUsername, GmailPassword)
            })

                try
                {
                    var emailFrom = WebConfigurationManager.AppSettings["emailfrom"];
                    await smtp.SendMailAsync(emailFrom, message.Destination, message.Subject, message.Body);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    await Task.FromResult(0);
                }
        }
    }
}

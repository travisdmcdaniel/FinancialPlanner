using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using FinancialPlanner.Models;

namespace FinancialPlanner.Controllers
{
    public class AccountHistoriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AccountHistories
        public ActionResult Index()
        {
            var accountHistories = db.AccountHistories.Include(a => a.Account).Include(a => a.Transaction);
            return View(accountHistories.ToList());
        }

        // GET: AccountHistories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHistory accountHistory = db.AccountHistories.Find(id);
            if (accountHistory == null)
            {
                return HttpNotFound();
            }
            return View(accountHistory);
        }

        // GET: AccountHistories/Create
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.TransactionId = new SelectList(db.Transactions, "Id", "EnteredById");
            return View();
        }

        // POST: AccountHistories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TransactionId,AccountId,TransactionAmount,AccountBeginBalance,AccountEndBalance,TransactionDate")] AccountHistory accountHistory)
        {
            if (ModelState.IsValid)
            {
                db.AccountHistories.Add(accountHistory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", accountHistory.AccountId);
            ViewBag.TransactionId = new SelectList(db.Transactions, "Id", "EnteredById", accountHistory.TransactionId);
            return View(accountHistory);
        }

        // GET: AccountHistories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHistory accountHistory = db.AccountHistories.Find(id);
            if (accountHistory == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", accountHistory.AccountId);
            ViewBag.TransactionId = new SelectList(db.Transactions, "Id", "EnteredById", accountHistory.TransactionId);
            return View(accountHistory);
        }

        // POST: AccountHistories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TransactionId,AccountId,TransactionAmount,AccountBeginBalance,AccountEndBalance,TransactionDate")] AccountHistory accountHistory)
        {
            if (ModelState.IsValid)
            {
                db.Entry(accountHistory).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", accountHistory.AccountId);
            ViewBag.TransactionId = new SelectList(db.Transactions, "Id", "EnteredById", accountHistory.TransactionId);
            return View(accountHistory);
        }

        // GET: AccountHistories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccountHistory accountHistory = db.AccountHistories.Find(id);
            if (accountHistory == null)
            {
                return HttpNotFound();
            }
            return View(accountHistory);
        }

        // POST: AccountHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccountHistory accountHistory = db.AccountHistories.Find(id);
            db.AccountHistories.Remove(accountHistory);
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
    }
}

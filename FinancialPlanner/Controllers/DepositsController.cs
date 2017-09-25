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

namespace FinancialPlanner.Controllers
{
    public class DepositsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Deposits
        [NoDirectAccess]
        public ActionResult Index()
        {
            var deposits = db.Deposits.Include(d => d.Account).Include(d => d.EnteredBy);
            return View(deposits.ToList());
        }

        // GET: Deposits/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // GET: Deposits/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name");
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName");
            return View();
        }

        // POST: Deposits/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create([Bind(Include = "Id,Amount,Source")] Deposit DepositToCreate, int id)
        {
            if (string.IsNullOrWhiteSpace(DepositToCreate.Source))
            {
                DepositToCreate.Source = "Uspecified";
            }
            DepositToCreate.AccountId = id;
            DepositToCreate.EnteredById = User.Identity.GetUserId();
            if (ModelState.IsValid)
            {
                DepositToCreate.Date = DateTime.Now;
                DepositToCreate.Year = DateTime.Now.Year;
                DepositToCreate.Month = DateTime.Now.Month;
                db.Deposits.Add(DepositToCreate);
                db.SaveChanges();
                Account account = db.Accounts.Find(id);
                account.Balance = account.Balance + DepositToCreate.Amount;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Reconcile(int accountId, int depositId, decimal amount, decimal reconciled)
        {
            Deposit deposit = db.Deposits.Find(depositId);
            if (deposit.ReconciledAmount > 0)
            {
                Account account = db.Accounts.Find(accountId);
                account.Balance += deposit.ReconciledAmount;
                account.Balance -= reconciled;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                deposit.ReconciledAmount = reconciled;
                db.Entry(deposit).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                deposit.ReconciledAmount = reconciled;
                db.Entry(deposit).State = EntityState.Modified;
                Account account = db.Accounts.Find(accountId);
                account.Balance += amount;
                account.Balance -= reconciled;
                account.Updated = DateTime.Now;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
            }

            return RedirectToAction("Index", "Home");
        }

        // GET: Deposits/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", deposit.AccountId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", deposit.EnteredById);
            return View(deposit);
        }

        // POST: Deposits/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Edit([Bind(Include = "Id,AccountId,EnteredById,Source,Amount,ReconciledAmount")] Deposit deposit)
        {
            if (ModelState.IsValid)
            {
                db.Entry(deposit).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Name", deposit.AccountId);
            ViewBag.EnteredById = new SelectList(db.Users, "Id", "FirstName", deposit.EnteredById);
            return View(deposit);
        }

        // GET: Deposits/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Deposit deposit = db.Deposits.Find(id);
            if (deposit == null)
            {
                return HttpNotFound();
            }
            return View(deposit);
        }

        // POST: Deposits/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            Deposit deposit = db.Deposits.Find(id);
            db.Deposits.Remove(deposit);
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

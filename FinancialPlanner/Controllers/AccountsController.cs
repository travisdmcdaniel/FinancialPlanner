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
using Microsoft.Ajax.Utilities;

namespace FinancialPlanner.Controllers
{
    public class AccountsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Accounts
        [NoDirectAccess]
        public ActionResult Index()
        {
            var accounts = db.Accounts.Include(a => a.Household);
            return View(accounts.ToList());
        }

        // GET: Accounts/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name");
            return View();
        }

        // POST: Accounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Balance,OverDraftFee,Shared")] Account AccountToCreate)
        {
            if (AccountToCreate.OverDraftFee == null)
            {
                AccountToCreate.OverDraftFee = 0;
            }
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                AccountToCreate.OwnerId = user.Id;
                AccountToCreate.HouseholdId = (int)user.HouseholdId;
                AccountToCreate.Created = DateTime.Now;
                
                db.Accounts.Add(AccountToCreate);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }

            //ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return RedirectToAction("Index", "Home");
        }

        // GET: Accounts/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", account.HouseholdId);
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Edit(int accountId, string NewName, decimal? NewOverDraftFee, bool NewShared)
        {
            if (ModelState.IsValid)
            {
                Account account = db.Accounts.Find(accountId);
                if (!NewName.IsNullOrWhiteSpace() && account.Name != NewName)
                {
                    account.Name = NewName;
                }
                if (NewOverDraftFee != null)
                {
                    account.OverDraftFee = NewOverDraftFee;
                }
                account.Shared = NewShared;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Accounts/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.Find(id);
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.Find(id);
            //db.Accounts.Remove(account);
            account.Deleted = true;
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
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

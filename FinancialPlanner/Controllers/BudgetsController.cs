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
    public class BudgetsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Budgets
        [NoDirectAccess]
        public ActionResult Index()
        {
            var budgets = db.Budgets.Include(b => b.Household);
            return View(budgets.ToList());
        }

        // GET: Budgets/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // GET: Budgets/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Budgets/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Monthly,AmountBudgeted")] Budget BudgetToCreate)
        {
            if (ModelState.IsValid)
            {
                var user = db.Users.Find(User.Identity.GetUserId());
                BudgetToCreate.HouseholdId = (int)user.HouseholdId;
                BudgetToCreate.Date = DateTime.Now;
                BudgetToCreate.Year = DateTime.Now.Year;
                BudgetToCreate.Month = DateTime.Now.Month;
                BudgetToCreate.AmountAgainst = 0.00m;
                db.Budgets.Add(BudgetToCreate);
                db.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", "Home");
        }

        // GET: Budgets/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // POST: Budgets/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Edit([Bind(Include = "Id,HouseholdId,Name")] Budget budget)
        {
            if (ModelState.IsValid)
            {
                db.Entry(budget).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.HouseholdId = new SelectList(db.Households, "Id", "Name", budget.HouseholdId);
            return View(budget);
        }

        // GET: Budgets/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Budget budget = db.Budgets.Find(id);
            if (budget == null)
            {
                return HttpNotFound();
            }
            return View(budget);
        }

        // POST: Budgets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            Budget budget = db.Budgets.Find(id);
            List<BudgetItem> budgetItems = db.BudgetItems.Where(bi => bi.BudgetId == id).ToList();
            budget.Deleted = true;
            foreach (var item in budgetItems)
            {
                item.Deleted = true;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            db.Entry(budget).State = EntityState.Modified;
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

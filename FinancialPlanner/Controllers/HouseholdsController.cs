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
using System.Net.Mail;
using System.Web.Configuration;
using FinancialPlanner.Helpers;

namespace FinancialPlanner.Controllers
{
    public class HouseholdsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        //private ApplicationUserManager UserManager;

        // GET: Households
        [NoDirectAccess]
        public ActionResult Index()
        {
            return View(db.Households.ToList());
        }

        [NoDirectAccess]
        public ActionResult Invite(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }

            InviteViewModel inviteViewModel = new InviteViewModel();
            inviteViewModel.HouseholdId = (int)id;
            ViewBag.HouseholdId = id;
            return View(inviteViewModel);
        }

        [HttpPost]
        [Authorize]
        [NoDirectAccess]
        public ActionResult Invite(string FirstName, string LastName, string InviteEmail)
        {
            MailMessage message = new MailMessage();
            var code = Guid.NewGuid().ToString();
            Invitation invitation = new Invitation();
            invitation.Code = code;
            invitation.Email = InviteEmail;
            invitation.FirstName = FirstName;
            invitation.LastName = LastName;
            invitation.HouseholdId = (int)db.Users.Find(User.Identity.GetUserId()).HouseholdId;
            invitation.ExpireDate = DateTime.Now.AddDays(7);
            invitation.Accepted = false;
            db.Invitations.Add(invitation);
            db.SaveChanges();
            var callbackUrl = Url.Action("InvitationRegister", "Account", new { household = (int)db.Users.Find(User.Identity.GetUserId()).HouseholdId, code = code }, protocol: Request.Url.Scheme);
            message.To.Add(new MailAddress(InviteEmail));
            message.Subject = "Invitation to Abacus household.";
            message.Body = "You have been invited to join a household in the Abacus Financial Planner!  " + 
                "Please click <a href=\"" + callbackUrl + "\">here</a> to accept the invitation and register!";
            SmtpClient client = new SmtpClient();
            client.Send(message);
            return RedirectToAction("Index", "Home");
        }

        // GET: Households/Details/5
        [NoDirectAccess]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // GET: Households/Create
        [NoDirectAccess]
        public ActionResult Create()
        {
            ApplicationUser user = db.Users.Find(User.Identity.GetUserId());

            if (user.HouseholdId != null)
            {
                RedirectToAction("Index", "Home");
            }
            return View();
        }

        // POST: Households/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Create([Bind(Include = "Id,Name,Description,Password")] Household household)
        {
            if (ModelState.IsValid)
            {
                bool repeat;
                string key;
                do
                {
                    key = Utilities.GenHouseholdKey();
                    repeat = false;
                    foreach (var house in db.Households)
                    {
                        if (house.Key == key)
                        {
                            repeat = true;
                        }
                    }
                } while (repeat);

                Utilities.AddUserToRole(User.Identity.GetUserId(), "Head Of Household");
                household.Key = key;
                household.Users.Add(db.Users.Find(User.Identity.GetUserId()));
                db.Households.Add(household);
                db.SaveChanges();
                return RedirectToAction("HouseholdCreated");
            }

            return View(household);
        }

        //[Authorize]
        [NoDirectAccess]
        public ActionResult Join()
        {
            return View();
        }

        [HttpPost]
        //[Authorize]
        [NoDirectAccess]
        public ActionResult Join(JoinHouseholdViewModel joinHouseholdVM)
        {
            if (!ModelState.IsValid)
            {
                return View();
            }
            Household household = db.Households.FirstOrDefault(h => h.Key == joinHouseholdVM.Key);
            if (household.Password == joinHouseholdVM.Password)
            {
                ApplicationUser user = db.Users.Find(User.Identity.GetUserId());
                user.HouseholdId = household.Id;
                Utilities.AddUserToRole(user.Id, "Member Of Household");
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("HouseholdJoined");
            }
            else
            {
                return View();
            }
        }

        //[Authorize]
        [NoDirectAccess]
        public ActionResult HouseholdJoined()
        {
            return View();
        }

        //[Authorize]
        [NoDirectAccess]
        public ActionResult HouseholdCreated()
        {
            return View();
        }

        [NoDirectAccess]
        public ActionResult Search(string searchStr)
        {
            ViewBag.Search = searchStr;
            var result = HouseholdSearch(searchStr);
            return View("SearchResults", result);
        }

        [NoDirectAccess]
        public ActionResult SearchResults(IQueryable result)
        {
            return View();
        }

        [NoDirectAccess]
        public IQueryable<Household> HouseholdSearch(string searchStr)
        {
            IQueryable<Household> result = null;
            if (searchStr != null)
            {
                result = db.Households.AsQueryable();
                result = result.Where(h => h.Name.Contains(searchStr) || h.Users.Any(u => u.FullName.Contains(searchStr) || u.LastName.Contains(searchStr) || u.DisplayName.Contains(searchStr) || u.Email.Contains(searchStr)));
            }
            else
            {
                result = db.Households.AsQueryable();
            }
            return result;
        }

        // GET: Households/Edit/5
        [NoDirectAccess]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] Household household)
        {
            if (ModelState.IsValid)
            {
                db.Entry(household).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(household);
        }

        // GET: Households/Delete/5
        [NoDirectAccess]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Household household = db.Households.Find(id);
            if (household == null)
            {
                return HttpNotFound();
            }
            return View(household);
        }

        // POST: Households/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [NoDirectAccess]
        public ActionResult DeleteConfirmed(int id)
        {
            Household household = db.Households.Find(id);
            db.Households.Remove(household);
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

        [NoDirectAccess]
        public async Task SendInviteAsync(IdentityMessage message)
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

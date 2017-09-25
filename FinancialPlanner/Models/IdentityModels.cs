using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations.Schema;
using System;
using System.Collections.Generic;

namespace FinancialPlanner.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DisplayName { get; set; }
        public string DisplaySetting { get; set; }
        public decimal BalanceWarning { get; set; }

        public int? HouseholdId { get; set; }
        
        public bool Protected { get; set; }

        public DateTime? DoB { get; set; }
        public int Age { get; set; }

        public virtual Household Household { get; set; }
        
        public virtual ICollection<Transaction> Transactions { get; set; } 

        [NotMapped]
        public string FullName
        {
            get { return FirstName + " " + LastName;  }
        }

        public ApplicationUser()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DefaultConnection", throwIfV1Schema: false)
        {
        }

        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Account> Accounts { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Household> Households { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Budget> Budgets { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.BudgetItem> BudgetItems { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Deposit> Deposits { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Invitation> Invitations { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.Transaction> Transactions { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.BudgetStatus> BudgetStatuses { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.InviteViewModel> InviteViewModels { get; set; }

        public System.Data.Entity.DbSet<FinancialPlanner.Models.AccountHistory> AccountHistories { get; set; }
    }
}
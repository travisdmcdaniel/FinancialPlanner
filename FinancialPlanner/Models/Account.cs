using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Account
    {
        // Primary Key
        public int Id { get; set; }

        // Foreign Keys
        public int HouseholdId { get; set; }

        public DateTime Created { get; set; }
        public DateTime? Updated { get; set; }

        public bool Deleted { get; set; }
        // Properties
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }

        public bool Shared { get; set; }
        public string OwnerId { get; set; }
        [Required]
        public decimal Balance { get; set; }
        public decimal ReconciledBalance { get; set; }
        public decimal? OverDraftFee { get; set; }

        public virtual Household Household { get; set; }
        public virtual ApplicationUser Owner { get; set; }

        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Deposit> Deposits { get; set; }
        //public ICollection<AccountHistory> AccountHistories { get; set; }

        public Account()
        {
            //this.AccountHistories = new HashSet<AccountHistory>();
            this.Transactions = new HashSet<Transaction>();
            this.Deposits = new HashSet<Deposit>();
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancialPlanner.Models
{
    public class Transaction
    {
        // Primary Key
        public int Id { get; set; }

        public bool Voided { get; set; }
        public bool Deleted { get; set; }

        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        //Foreign Keys
        //public int AccountHistoryId { get; set; }
        public int AccountId { get; set; }
        public int? BudgetItemId { get; set; }
        public string EnteredById { get; set; }

        public string Type { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public decimal ReconciledAmount { get; set; }

        public virtual Account Account { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
        public virtual BudgetItem BudgetItem { get; set; }
        //public virtual AccountHistory AccountHistory { get; set; }

        public Transaction()
        {

        }

        public Transaction(Transaction transaction)
        {
            this.AccountId = transaction.AccountId;
            this.BudgetItemId = transaction.BudgetItemId;
            this.Amount = transaction.Amount;
            this.Date = transaction.Date;
            this.Deleted = transaction.Deleted;
            this.EnteredById = transaction.EnteredById;
            this.Month = transaction.Month;
            this.ReconciledAmount = transaction.ReconciledAmount;
            this.Type = transaction.Type;
            this.Voided = transaction.Voided;
            this.Year = transaction.Year;
        }
    }
}
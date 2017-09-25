using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancialPlanner.Models
{
    public class BudgetItem
    {
        public int Id { get; set; }
        
        public int BudgetId { get; set; }

        public bool Deleted { get; set; }
        public DateTime Date { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }

        public string Name { get; set; }
        public decimal Amount { get; set; }

        public virtual Budget Budget { get; set; }

        public virtual  ICollection<Transaction> Transactions { get; set; }

        public BudgetItem()
        {
            this.Transactions = new HashSet<Transaction>();
        }

        public BudgetItem(BudgetItem budgetItem)
        {
            this.Transactions = new HashSet<Transaction>();
            this.Amount = budgetItem.Amount;
            this.BudgetId = budgetItem.BudgetId;
            this.Date = DateTime.Now;
            this.Month = Date.Month;
            this.Year = Date.Year;
            this.Name = budgetItem.Name;
        }
    }
}
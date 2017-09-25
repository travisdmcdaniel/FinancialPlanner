using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancialPlanner.Models
{
    public class Budget
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        public decimal AmountBudgeted { get; set; }
        public decimal AmountAgainst { get; set; }

        public bool Deleted { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        public DateTime Date { get; set; }

        public bool Monthly { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public virtual Household Household { get; set; }
        public virtual ICollection<BudgetItem> BudgetItems { get; set; }

        public Budget()
        {
            this.BudgetItems = new HashSet<BudgetItem>();
        }
    }
}
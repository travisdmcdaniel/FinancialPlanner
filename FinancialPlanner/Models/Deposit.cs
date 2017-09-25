using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Deposit
    {
        // Primary Key
        public int Id { get; set; }

        //Foreign Keys
        public int AccountId { get; set; }
        public string EnteredById { get; set; }
        public string Source { get; set; }

        public DateTime Date { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }

        [Required]
        public decimal Amount { get; set; }
        public decimal ReconciledAmount { get; set; }

        public virtual Account Account { get; set; }
        public virtual ApplicationUser EnteredBy { get; set; }
    }
}
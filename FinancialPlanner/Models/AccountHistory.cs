using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class AccountHistory
    {
        [Key]
        public int Id { get; set; }

        public int? TransactionId { get; set; }
        public int? DepositId { get; set; }
        public int AccountId { get; set; }

        public decimal TransactionAmount { get; set; }
        public decimal AccountBeginBalance { get; set; }
        public decimal AccountEndBalance { get; set; }
        public DateTime TransactionDate { get; set; }

        public virtual Transaction Transaction { get; set; }
        public virtual Account Account { get; set; }
        public virtual Deposit Deposit { get; set; }
    }
}
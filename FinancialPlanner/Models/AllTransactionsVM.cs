using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class AllTransactionsVM
    {
        public string AccountName { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Deposit> Deposits { get; set; }
    }
}
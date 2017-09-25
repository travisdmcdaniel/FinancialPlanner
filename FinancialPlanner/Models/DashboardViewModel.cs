using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class DashboardViewModel
    {
        public Account AccountToCreate { get; set; }
        public Transaction TransactionToCreate { get; set; }
        public Budget BudgetToCreate { get; set; }
        public BudgetItem BudgetItemToCreate { get; set; }
        public Deposit DepositToCreate { get; set; }
        public Invitation InvitationToCreate { get; set; }
        public Household Household { get; set; }
        public ICollection<Account> Accounts { get; set; }
        public ICollection<Budget> Budgets { get; set; }
        public ICollection<ApplicationUser> Members { get; set; }
        public ICollection<BudgetItem> BudgetItems { get; set; }
        public ICollection<Deposit> Deposits { get; set; }
        public ICollection<Transaction> Transactions { get; set; }
        public ICollection<Invitation> Invitations { get; set; }
    }
}
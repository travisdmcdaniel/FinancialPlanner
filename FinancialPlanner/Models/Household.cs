using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class Household
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Description { get; set; }

        public string Key { get; set; }
        public string Password { get; set; }

        [NotMapped]
        public decimal? Balance { get; set; }

        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<Account> Accounts { get; set; }
        public virtual ICollection<Budget> Budgets { get; set; }

        public Household()
        {
            this.Users = new HashSet<ApplicationUser>();
            this.Accounts = new HashSet<Account>();
            this.Budgets = new HashSet<Budget>();
        }
    }
}
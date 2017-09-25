using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class InviteViewModel
    {
        public int Id { get; set; }
        public int HouseholdId { get; set; }
        [Required]
        [EmailAddress]
        public string InviteEmail { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }
        public virtual Household Household { get; set; }
    }
}
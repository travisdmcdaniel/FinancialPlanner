using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace FinancialPlanner.Models
{
    public class Invitation
    {
        public int Id { get; set; }

        public int HouseholdId { get; set; }

        public DateTime ExpireDate { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Code { get; set; }
        public bool Accepted { get; set; }
        public bool Expired { get; set; }

        public virtual Household Household { get; set; }

        public Invitation()
        {
            this.Code = Guid.NewGuid().ToString();
        }
    }
}
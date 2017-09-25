using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class JoinHouseholdViewModel
    {
        [Required]
        public string Key { get; set; }
        [Required]
        public string Password { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class HouseholdSearchViewModel
    {
        [Required]
        public string SearchType { get; set; }
        [Required]
        public string SearchString { get; set; }
    }
}
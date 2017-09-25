using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinancialPlanner.Models
{
    public class GraphData
    {
        public string label { get; set; }
        public decimal budget { get; set; }
        public decimal spent { get; set; }

        public GraphData(string label, decimal budget, decimal spent)
        {
            this.label = label;
            this.budget = budget;
            this.spent = spent;
        }

        public GraphData()
        {
        }
    }

    public class SimpleGraphData
    {
        public string label { get; set; }
        public decimal value { get; set; }

        public SimpleGraphData(string label, decimal value)
        {
            this.label = label;
            this.value = value;
        }

        public SimpleGraphData()
        {
        }
    }
}
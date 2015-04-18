using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class DashboardViewModel
    {
        public DashboardViewModel()
        {
            StartDate = DateTime.Now.AddDays(-30).ToString("dd/MM/yyyy");
            EndDate = DateTime.Now.ToString("dd/MM/yyyy");
        }

        public DashboardProjectsViewModel Projects { get; set; }
        public Dictionary<int, string> Clients { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
    }
}
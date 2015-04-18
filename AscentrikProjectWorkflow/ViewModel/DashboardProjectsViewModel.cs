using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class DashboardProjectsViewModel
    {
        public List<ProjectViewModel> NewProjects { get; set; }
        public List<ProjectViewModel> InProgressProjects { get; set; }
        public List<ProjectViewModel> CompleteProjects { get; set; }
        public bool ShowNewProjects { get; set; }
        public bool ShowInProgressProjects { get; set; }
        public bool ShowCompleteProjects { get; set; }
    }
}
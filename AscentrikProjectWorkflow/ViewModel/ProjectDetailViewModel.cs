using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.ViewModel
{
    public class ProjectDetailViewModel
    {
        public ProjectViewModel ProjectInfo { get; set; }
        public ProjectCostingViewModel ProjectCosting { get; set; }
        public bool ShowCosting { get; set; }
    }
}
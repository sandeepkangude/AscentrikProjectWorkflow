using AscentrikProjectWorkflow.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AscentrikProjectWorkflow.Controllers
{
    [Authorize]
    public class DashboardController : BaseController
    {
        //
        // GET: /Dashboard/

        public ActionResult Index()
        {
            var model = new DashboardViewModel();
            var result = new List<ProjectViewModel>();
            var userId = Request.Cookies["auth.id"].Value;
            if (User.IsInRole("Admin"))
            {
                result = dashboardModel.GetProjectListForAdmin(DateTime.Now.AddDays(-30), DateTime.Now, 0);
            }
            else
            {
                dashboardModel.GetProjectListForManager(DateTime.Now.AddDays(-30), DateTime.Now, 0, Convert.ToInt32(userId));
            }
            model.Projects = BindDashboardProjectsModel(result);
            model.Clients = clientModel.GetActiveClientList().ToDictionary(x => x.Id, x => x.Code);
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="result"></param>
        /// <returns></returns>
        private DashboardProjectsViewModel BindDashboardProjectsModel(List<ProjectViewModel> result)
        {
            var model = new DashboardProjectsViewModel();

            model.NewProjects = new List<ProjectViewModel>();
            model.InProgressProjects = new List<ProjectViewModel>();
            model.CompleteProjects = new List<ProjectViewModel>();

            if (result != null)
            {
                foreach (var item in result)
                {
                    if (item.ProjectStatus == (int)AscentrikProjectWorkflow.Enum.ProjectStatus.New)
                        model.NewProjects.Add(item);
                    else if (item.ProjectStatus == (int)AscentrikProjectWorkflow.Enum.ProjectStatus.InvoiceRaised || item.ProjectStatus == (int)AscentrikProjectWorkflow.Enum.ProjectStatus.PartlyDeliveredAndInvoiceRaised && item.ProjectStatus == (int)AscentrikProjectWorkflow.Enum.ProjectStatus.PartlyDeliveredAndResearchHalted)
                        model.CompleteProjects.Add(item);
                    else
                        model.InProgressProjects.Add(item);
                }
            }

            if (model.NewProjects.Any())
                model.ShowNewProjects = true;
            if (model.InProgressProjects.Any())
                model.ShowInProgressProjects = true;
            if (model.CompleteProjects.Any())
                model.ShowCompleteProjects = true;


            return model;
        }

        public ActionResult GetProjectById(int id)
        {
            var model = new ProjectDetailViewModel();
            if (User.IsInRole("Admin"))
            {
                model = projectModel.GetProjectById_Admin(id);
            }
            else
            {
                model = projectModel.GetProjectById_Manager(id);
            }

            model.ProjectInfo = BindProjectModel(model.ProjectInfo);

            if (User.IsInRole("Admin"))
            {
                model.ShowCosting = true;
                model.ProjectCosting = BindProjectCostingModel(model.ProjectCosting);

            }
            else
            {
                model.ShowCosting = false;
            }

            return PartialView("_Partial_ProjectAdmin", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        public ActionResult AdminDashboard()
        {
            var model = new DashboardViewModel();
            var result = dashboardModel.GetProjectListForAdmin(DateTime.Now.AddDays(-30), DateTime.Now, 0);
            model.Projects = BindDashboardProjectsModel(result);
            model.Clients = clientModel.GetActiveClientList().ToDictionary(x => x.Id, x => x.Code);

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="startMonth"></param>
        /// <param name="startYear"></param>
        /// <param name="endMonth"></param>
        /// <param name="endYear"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        public ActionResult FilterProjects(int startMonth, int startYear, int endMonth, int endYear, int client)
        {
            var startDate = new DateTime(startYear, startMonth, 1);
            var endDate = new DateTime(endYear, endMonth, 1).AddMonths(1).AddDays(-1);
            var result = dashboardModel.GetProjectListForAdmin(startDate, endDate, client);
            var model = BindDashboardProjectsModel(result);
            return PartialView("_Partial_Projects", model);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Manager")]
        public ActionResult ManagerDashboard()
        {
            return View();
        }

    }
}

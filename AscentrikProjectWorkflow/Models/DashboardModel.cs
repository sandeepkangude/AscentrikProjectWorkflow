using AscentrikProjectWorkflow.DataModel;
using AscentrikProjectWorkflow.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.Models
{
    public class DashboardModel
    {
        private AscentrikProjectWorkflowEntities db = new AscentrikProjectWorkflowEntities();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        internal List<ProjectViewModel> GetProjectListForAdmin(DateTime start, DateTime end, int client)
        {
            var tblProjects = new List<Project>();

            if (client <= 0)
            {
                tblProjects = db.Projects
                    .Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu")
                    .Where(x => x.EditedOn >= start && x.EditedOn <= end)
                    .OrderByDescending(x => x.EditedOn).ThenByDescending(x => x.CreatedOn).ToList();
            }
            else
            {
                var lstLists = db.Lists.Where(x => x.ClientId == client).Select(x => x.Id).ToList();
                tblProjects = db.Projects
                    .Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu")
                    .Where(x => (x.EditedOn >= start && x.EditedOn <= end) && lstLists.Contains(x.ListId))
                    .OrderByDescending(x => x.EditedOn).ThenByDescending(x => x.CreatedOn).ToList();
            }
            var model = Mapper.Map<List<Project>, List<ProjectViewModel>>(tblProjects);

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="client"></param>
        /// <returns></returns>
        internal List<ProjectViewModel> GetProjectListForManager(DateTime start, DateTime end, int client, int userId)
        {
            var tblProjects = new List<Project>();

            if (client <= 0)
            {
                tblProjects = db.Projects
                    .Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu")
                    .Where(x => (x.EditedOn >= start && x.EditedOn <= end) && x.CreatedBy == userId)
                    .OrderByDescending(x => x.EditedOn).ThenByDescending(x => x.CreatedOn).ToList();
            }
            else
            {
                var lstLists = db.Lists.Where(x => x.ClientId == client).Select(x => x.Id).ToList();
                tblProjects = db.Projects
                    .Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu")
                    .Where(x => (x.EditedOn >= start && x.EditedOn <= end) && x.CreatedBy == userId && lstLists.Contains(x.ListId))
                    .OrderByDescending(x => x.EditedOn).ThenByDescending(x => x.CreatedOn).ToList();
            }
            var model = Mapper.Map<List<Project>, List<ProjectViewModel>>(tblProjects);

            return model;
        }
    }
}
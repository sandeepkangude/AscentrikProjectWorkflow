using AscentrikProjectWorkflow.DataModel;
using AscentrikProjectWorkflow.ViewModel;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace AscentrikProjectWorkflow.Models
{
    public class ProjectModel
    {
        private AscentrikProjectWorkflowEntities db = new AscentrikProjectWorkflowEntities();

        #region CommonMethods

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> ProjectPriorityList()
        {
            var lst = db.Priorities.ToList();
            var model = lst.ToDictionary(x => x.Id, x => x.Name);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> ProjectStatusList()
        {
            var lst = db.ProjectStatus.ToList();
            var model = lst.ToDictionary(x => x.Id, x => x.Name);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> ClientList()
        {
            var lst = db.Clients.Where(x => x.IsActive).ToList();
            var model = lst.ToDictionary(x => x.Id, x => x.Code);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> ListReferenceList(int clientId)
        {
            var lst = db.Lists.Where(x => x.ClientId == clientId && x.IsActive).ToList();
            var model = lst.ToDictionary(x => x.Id, x => x.Code);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> CurrencyList()
        {
            var lst = db.Currencies.ToList();
            var model = lst.ToDictionary(x => x.Id, x => x.Name);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal Dictionary<int, string> PaymentStatusList()
        {
            var model = new Dictionary<int, string> { { 1, "Pending" }, { 2, "Received" } };
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int AddProject(ProjectViewModel model)
        {
            if (model != null)
            {
                var tblLists = db.Projects.Count(x => x.Name == model.Name);
                if (tblLists > 0)
                    return 2; // Project is already present in the database.
                var project = new Project
                {
                    Name = model.Name,
                    ListId = model.ListId,
                    Priority = model.Priority,
                    ProjectCode = model.ProjectCode,
                    ProjectBrief = model.ProjectBrief,
                    LinkdinLink = model.LinkdinLink,
                    Requester = model.Requester,
                    ApprovalDate = model.ApprovalDate,
                    DatesInformedToClient = model.DatesInformedToClient,
                    NumberOfCompaniesApproved = model.NumberOfCompaniesApproved,
                    JobTitlesPerCompany = model.JobTitlesPerCompany,
                    RecordsToBeResearched = model.RecordsToBeResearched,
                    ExpectedDeliveryDate = model.ExpectedDeliveryDate,
                    ProjectStatus = 1,
                    IsActive = model.IsActive,
                    AllotedTo = model.AllotedTo,
                    Names = model.Names,
                    CreatedBy = model.CreatedBy,
                    CreatedOn = model.CreatedOn,
                    EditedOn = model.CreatedOn
                };

                db.Projects.Add(project);
                db.SaveChanges();
                return 0;
            }
            return 1; //Null object
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditProjectBasicInfo(ProjectViewModel model)
        {
            if (model != null)
            {
                var tblLists = db.Projects.Count(x => x.ProjectCode == model.ProjectCode);
                if (tblLists > 1)
                    return 2; // Project is already present in the database.

                var project = db.Projects.Where(x => x.Id == model.Id).FirstOrDefault();
                if (project == null)
                    return 3; //Data is not available in the database.

                project.Name = model.Name;
                project.ProjectCode = model.ProjectCode;
                project.ListId = model.ListId;
                project.ProjectBrief = model.ProjectBrief;
                project.Priority = model.Priority;
                project.LinkdinLink = model.LinkdinLink;
                project.Requester = model.Requester;
                project.ApprovalDate = model.ApprovalDate;
                project.DatesInformedToClient = model.DatesInformedToClient;
                project.NumberOfCompaniesApproved = model.NumberOfCompaniesApproved;
                project.JobTitlesPerCompany = model.JobTitlesPerCompany;
                project.RecordsToBeResearched = model.RecordsToBeResearched;
                project.ExpectedDeliveryDate = model.ExpectedDeliveryDate;
                project.AllotedTo = model.AllotedTo;
                project.Names = model.Names;
                project.ProjectStatus = model.ProjectStatus;
                project.IsActive = model.IsActive;
                project.EditedBy = model.EditedBy;
                project.EditedOn = model.EditedOn;

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return 0;
            }
            return 1; //Null object
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal int EditProjectTrackingInfo(ProjectViewModel model)
        {
            if (model != null)
            {
                var project = db.Projects.Where(x => x.Id == model.Id).FirstOrDefault();
                if (project == null)
                    return 3; //Data is not available in the database.

                if (model.ProjectStatus == (int)AscentrikProjectWorkflow.Enum.ProjectStatus.New && model.NoOfRecordsDeliverd > 0)
                    model.ProjectStatus = (int)AscentrikProjectWorkflow.Enum.ProjectStatus.ResearchOngoing;

                project.NoOfRecordsDeliverd = model.NoOfRecordsDeliverd;
                project.DuplicateFounds = model.DuplicateFounds;
                project.ActualDeliveryDate = model.ActualDeliveryDate;
                project.FTPUploadDate = model.FTPUploadDate;
                project.FTPUploadRemarks = model.FTPUploadRemarks;
                project.ResearchRemark = model.ResearchRemark;
                project.EditedBy = model.EditedBy;
                project.EditedOn = model.EditedOn;

                db.Entry(project).State = EntityState.Modified;
                db.SaveChanges();
                return 0;
            }
            return 1; //Null object
        }
        #endregion

        #region ADMIN

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ProjectViewModel> GetProjectListForAdmin()
        {
            var tblProjects = db.Projects.Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu").OrderByDescending(x => x.CreatedOn).ThenByDescending(x => x.EditedOn).ToList();
            var model = Mapper.Map<List<Project>, List<ProjectViewModel>>(tblProjects);

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal ProjectDetailViewModel GetProjectById_Admin(int id)
        {
            var project = db.Projects.Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu").Include("ProjectCostings").FirstOrDefault(x => x.Id == id);
            var projectInfo = Mapper.Map<Project, ProjectViewModel>(project);
            projectInfo.ClientId = project.List.ClientId;
            var tblProjCosting = projectInfo.ProjectCostings.FirstOrDefault();
            var projectCosting = Mapper.Map<ProjectCosting, ProjectCostingViewModel>(tblProjCosting);

            var model = new ProjectDetailViewModel();
            model.ProjectInfo = projectInfo;
            if (projectCosting == null)
            {
                projectCosting = new ProjectCostingViewModel();
                projectCosting.ProjectId = id;
                var currency = projectCosting.Currency.GetValueOrDefault();
                projectCosting.Currency1 = db.Currencies.FirstOrDefault(x => x.Id == currency);
            }
            model.ProjectCosting = projectCosting;
            
            return model;
        }

        internal int EditProjectCostingInfo(ProjectCostingViewModel model)
        {
            if (model != null)
            {
                var project = db.Projects.Where(x => x.Id == model.ProjectId).FirstOrDefault();
                if (project == null)
                    return 1; //Data is not available in the database.

                var projectCosting = db.ProjectCostings.Where(x => x.Id == model.ProjectCostingId).FirstOrDefault();

                if (projectCosting != null)
                {
                    projectCosting.CostPerRecord = model.CostPerRecord;
                    projectCosting.Currency = model.Currency;
                    projectCosting.InvoiceCode = model.InvoiceCode;
                    projectCosting.InvoiceDate = model.InvoiceDate;
                    projectCosting.InvoiceRemark = model.InvoiceRemark;
                    projectCosting.PaymentStatus = model.PaymentStatus;
                    projectCosting.ProjectInvoice = project.NoOfRecordsDeliverd.GetValueOrDefault() * model.CostPerRecord.GetValueOrDefault();
                    projectCosting.EditedBy = model.EditedBy;
                    projectCosting.EditedOn = model.EditedOn;

                    db.Entry(projectCosting).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    projectCosting = new ProjectCosting();
                    projectCosting.ProjectId = model.ProjectId;
                    projectCosting.CostPerRecord = model.CostPerRecord;
                    projectCosting.Currency = model.Currency;
                    projectCosting.InvoiceCode = model.InvoiceCode;
                    projectCosting.InvoiceDate = model.InvoiceDate;
                    projectCosting.InvoiceRemark = model.InvoiceRemark;
                    projectCosting.PaymentStatus = model.PaymentStatus;
                    projectCosting.ProjectInvoice = project.NoOfRecordsDeliverd.GetValueOrDefault() * model.CostPerRecord.GetValueOrDefault();
                    projectCosting.EditedBy = model.EditedBy;
                    projectCosting.EditedOn = model.EditedOn;

                    db.ProjectCostings.Add(projectCosting);
                    db.SaveChanges();
                    
                }
                
                return 0;
            }
            return 1; //Null object
        }
        #endregion

        #region MANAGER

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        internal List<ProjectViewModel> GetProjectListForManager(int userId)
        {
            var tblProjects = db.Projects.Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu").Where(x => x.CreatedBy == userId).OrderByDescending(x => x.EditedOn).ThenByDescending(x => x.CreatedOn).ToList();
            var model = Mapper.Map<List<Project>, List<ProjectViewModel>>(tblProjects);
            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        internal ProjectDetailViewModel GetProjectById_Manager(int id)
        {
            var project = db.Projects.Include("List.Client").Include("List").Include("Priority1").Include("ProjectStatu").FirstOrDefault(x => x.Id == id);
            var projectInfo = Mapper.Map<Project, ProjectViewModel>(project);
            projectInfo.ClientId = project.List.ClientId;
            var tblProjCosting = projectInfo.ProjectCostings.FirstOrDefault();
            var model = new ProjectDetailViewModel();
            model.ProjectInfo = projectInfo;

            return model;
        }
        #endregion
    }
}
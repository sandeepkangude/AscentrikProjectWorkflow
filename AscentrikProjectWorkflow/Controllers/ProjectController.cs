using AscentrikProjectWorkflow.ViewModel;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AscentrikProjectWorkflow.Controllers
{
    public class ProjectController : BaseController
    {
        //
        // GET: /Project/

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize]
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var model = projectModel.GetProjectListForAdmin();
                return View("AdminIndex", model);
            }
            else
            {
                var userId = Request.Cookies["auth.id"].Value;
                var model = projectModel.GetProjectListForManager(Convert.ToInt32(userId));
                return View("ManagerIndex", model);
            }
        }

        #region Create Project

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Manager")]
        public ActionResult Create()
        {
            var model = new ProjectViewModel();
            model = BindProjectModel(model);
            model.ProjectStatus = (int)AscentrikProjectWorkflow.Enum.ProjectStatus.New;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Manager")]
        [HttpPost]
        public ActionResult Create(ProjectViewModel model)
        {
            model = BindProjectModel(model);
            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.CreatedOn = DateTime.Now;
                var result = projectModel.AddProject(model);
                if (result == 0)
                {
                    return RedirectToAction("Index");
                }
                else if (result == 1)
                {
                    ModelState.AddModelError("CustomError", "Invalid project.");
                }
                else if (result == 2)
                {
                    ModelState.AddModelError("CustomError", "This list type is already present in the database.");
                }
                else
                {
                    ModelState.AddModelError("CustomError", "Sorry! Record is not added successfully.");
                }
            }
            else
            {
                StringBuilder result = new StringBuilder();

                foreach (var item in ModelState)
                {
                    string key = item.Key;
                    var errors = item.Value.Errors;

                    foreach (var error in errors)
                    {
                        result.Append(key + " " + error.ErrorMessage);
                    }
                }
                ModelState.AddModelError("CustomError", result.ToString());
            }
            return View(model);
        }

        #endregion

        #region Edit Project

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        public ActionResult Edit(int id)
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
            return View("Edit", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost]
        public ActionResult Edit(ProjectViewModel projectInfo)
        {
            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                projectInfo.EditedBy = Convert.ToInt32(userId);
                projectInfo.EditedOn = DateTime.Now;

                try
                {
                    if (!string.IsNullOrWhiteSpace(projectInfo.ApprovalDateText))
                        projectInfo.ApprovalDate = Convert.ToDateTime(projectInfo.ApprovalDateText);
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Actual Delivery Date format is incorrect." });
                }

                try
                {
                    if (!string.IsNullOrWhiteSpace(projectInfo.ExpectedDeliveryDateText))
                        projectInfo.ExpectedDeliveryDate = Convert.ToDateTime(projectInfo.ExpectedDeliveryDateText);
                }
                catch (Exception ex)
                {
                    return Json(new { status = false, message = "Expected Delivery Date format is incorrect." });
                }

                var result = projectModel.EditProjectBasicInfo(projectInfo);

                if (result == 0)
                {
                    return Json(new { status = true, message = "Project is updated successfully." });
                }
                else if (result == 1)
                {
                    return Json(new { status = false, message = "Please enter all required values." });
                }
                else if (result == 2)
                {
                    return Json(new { status = false, message = "This Project Code is already present in the database." });
                }
                else
                {
                    return Json(new { status = false, message = "Project is not updated successfully." });
                }
            }
            else
            {
                StringBuilder result = new StringBuilder();

                foreach (var item in ModelState)
                {
                    string key = item.Key;
                    var errors = item.Value.Errors;

                    foreach (var error in errors)
                    {
                        result.Append(key + " " + error.ErrorMessage);
                    }
                }
                return Json(new { status = false, message = result });
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        /// 
        [Authorize]
        [HttpPost]
        public ActionResult TrackProject(ProjectViewModel projectInfo)
        {

            var userId = Request.Cookies["auth.id"].Value;
            projectInfo.EditedBy = Convert.ToInt32(userId);
            projectInfo.EditedOn = DateTime.Now;
            try
            {
                if (!string.IsNullOrWhiteSpace(projectInfo.FTPUploadDateText))
                    projectInfo.FTPUploadDate = Convert.ToDateTime(projectInfo.FTPUploadDateText);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "FTP Upload Date format is incorrect." });
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(projectInfo.ActualDeliveryDateText))
                    projectInfo.ActualDeliveryDate = Convert.ToDateTime(projectInfo.ActualDeliveryDateText);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "Actual Delivery Date format is incorrect." });
            }
            var result = projectModel.EditProjectTrackingInfo(projectInfo);

            if (result == 0)
            {
                return Json(new { status = true, message = "Project is updated successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Please enter all required values." });
            }
            else
            {
                return Json(new { status = false, message = "Project is not updated successfully." });
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="projectInfo"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult EditProjectCosting(ProjectCostingViewModel projectInfo)
        {

            var userId = Request.Cookies["auth.id"].Value;
            projectInfo.EditedBy = Convert.ToInt32(userId);
            projectInfo.EditedOn = DateTime.Now;
            try
            {
                if (!string.IsNullOrWhiteSpace(projectInfo.InvoiceDateText))
                    projectInfo.InvoiceDate = Convert.ToDateTime(projectInfo.InvoiceDateText);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = "FTP Upload Date format is incorrect." });
            }


            var result = projectModel.EditProjectCostingInfo(projectInfo);

            if (result == 0)
            {
                return Json(new { status = true, message = "Project costing is updated successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Please enter all required values." });
            }
            else
            {
                return Json(new { status = false, message = "Project costing is not updated successfully." });
            }

        }

        #endregion

        #region Download Report

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        public void GenerateInvoice(int id)
        {
            var sb = User.IsInRole("Admin") ? GenerateInvoiceAdmin(id) : GenerateInvoiceManager(id);
            var fileName = id.ToString() + "_report.csv";
            Response.Clear();
            Response.ContentType = "text/csv";
            Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
            Response.Flush();
            Response.Write(sb.ToString());
            Response.End();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private StringBuilder GenerateInvoiceAdmin(int id)
        {
            var project = projectModel.GetProjectById_Admin(id);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (project != null && project.ProjectInfo != null)
            {
                sb.Append(MakeValueCsvFriendly("Client")).Append(",");
                sb.Append(MakeValueCsvFriendly("Client Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Ref")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Name")).Append(",");
                sb.Append(MakeValueCsvFriendly("Priority")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Type")).Append(",");

                sb.Append(MakeValueCsvFriendly("LinkedIn Link")).Append(",");
                sb.Append(MakeValueCsvFriendly("Requester")).Append(",");
                sb.Append(MakeValueCsvFriendly("Approval Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Dates Informed to Client")).Append(",");
                sb.Append(MakeValueCsvFriendly("No. Companies Approved")).Append(",");
                sb.Append(MakeValueCsvFriendly("Job Titles per Company")).Append(",");
                sb.Append(MakeValueCsvFriendly("Records to be Rsearched")).Append(",");
                sb.Append(MakeValueCsvFriendly("Duplicates Found")).Append(",");
                sb.Append(MakeValueCsvFriendly("No. Records Delivered")).Append(",");
                sb.Append(MakeValueCsvFriendly("Expected Delivery Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Actual Delivery Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Upload Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Upload Remark")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Status")).Append(",");
                sb.Append(MakeValueCsvFriendly("Alloted To")).Append(",");
                sb.Append(MakeValueCsvFriendly("Names")).Append(",");
                sb.Append(MakeValueCsvFriendly("Cost Per Record")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Invoice")).Append(",");
                sb.Append(MakeValueCsvFriendly("Currency")).Append(",");
                sb.Append(MakeValueCsvFriendly("Invoice Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("Invoice Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Payment Status")).Append(",");
                sb.Append(MakeValueCsvFriendly("Invoice Remarks")).Append(",");
                sb.Append(MakeValueCsvFriendly("Research Remarks")).Append(",");


                sb.AppendLine();

                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Client.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Client.Code)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Code)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Reference)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Priority1.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ProjectCode)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.ListType.Type)).Append(",");

                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.LinkdinLink)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Requester)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ApprovalDate.HasValue ? project.ProjectInfo.ApprovalDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.DatesInformedToClient.GetValueOrDefault() ? "Yes" : "No")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.NumberOfCompaniesApproved)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.JobTitlesPerCompany)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.RecordsToBeResearched)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.DuplicateFounds)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.NoOfRecordsDeliverd)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ExpectedDeliveryDate.HasValue ? project.ProjectInfo.ExpectedDeliveryDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ActualDeliveryDate.HasValue ? project.ProjectInfo.ActualDeliveryDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.FTPUploadDate.HasValue ? project.ProjectInfo.FTPUploadDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.FTPUploadRemarks)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ProjectStatu.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.AllotedTo)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Names)).Append(",");

                if (project.ProjectCosting != null)
                {
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.CostPerRecord)).Append(",");
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.ProjectInvoice)).Append(",");
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.Currency1 != null ? project.ProjectCosting.Currency1.Code : "")).Append(",");
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.InvoiceCode)).Append(",");
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.InvoiceDate.HasValue ? project.ProjectCosting.InvoiceDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                    
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.PaymentStatus.GetValueOrDefault())).Append(",");
                    sb.Append(MakeValueCsvFriendly(project.ProjectCosting.InvoiceRemark)).Append(",");
                }
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ResearchRemark)).Append(",");

            }

            return sb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        private StringBuilder GenerateInvoiceManager(int id)
        {
            var project = projectModel.GetProjectById_Admin(id);
            StringBuilder sb = new StringBuilder();
            sb.AppendLine();
            if (project != null && project.ProjectInfo != null)
            {
                //sb.Append(MakeValueCsvFriendly("Client")).Append(",");
                sb.Append(MakeValueCsvFriendly("Client Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Ref")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Name")).Append(",");
                sb.Append(MakeValueCsvFriendly("Priority")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Code")).Append(",");
                sb.Append(MakeValueCsvFriendly("List Type")).Append(",");

                sb.Append(MakeValueCsvFriendly("LinkedIn Link")).Append(",");
                sb.Append(MakeValueCsvFriendly("Requester")).Append(",");
                sb.Append(MakeValueCsvFriendly("Approval Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Dates Informed to Client")).Append(",");
                sb.Append(MakeValueCsvFriendly("No. Companies Approved")).Append(",");
                sb.Append(MakeValueCsvFriendly("Job Titles per Company")).Append(",");
                sb.Append(MakeValueCsvFriendly("Records to be Rsearched")).Append(",");
                sb.Append(MakeValueCsvFriendly("Duplicates Found")).Append(",");
                sb.Append(MakeValueCsvFriendly("No. Records Delivered")).Append(",");
                sb.Append(MakeValueCsvFriendly("Expected Delivery Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Actual Delivery Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Upload Date")).Append(",");
                sb.Append(MakeValueCsvFriendly("Upload Remark")).Append(",");
                sb.Append(MakeValueCsvFriendly("Project Status")).Append(",");
                sb.Append(MakeValueCsvFriendly("Alloted To")).Append(",");
                sb.Append(MakeValueCsvFriendly("Names")).Append(",");
                sb.Append(MakeValueCsvFriendly("Research Remarks")).Append(",");
                

                sb.AppendLine();

                //sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Client.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Client.Code)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Code)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.Reference)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Priority1.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ProjectCode)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.List.ListType.Type)).Append(",");

                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.LinkdinLink)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Requester)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ApprovalDate.HasValue ? project.ProjectInfo.ApprovalDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.DatesInformedToClient.GetValueOrDefault() ? "Yes" : "No")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.NumberOfCompaniesApproved)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.JobTitlesPerCompany)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.RecordsToBeResearched)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.DuplicateFounds)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.NoOfRecordsDeliverd)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ExpectedDeliveryDate.HasValue ? project.ProjectInfo.ExpectedDeliveryDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ActualDeliveryDate.HasValue ? project.ProjectInfo.ActualDeliveryDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.FTPUploadDate.HasValue ? project.ProjectInfo.FTPUploadDate.GetValueOrDefault().ToShortDateString() : "")).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.FTPUploadRemarks)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ProjectStatu.Name)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.AllotedTo)).Append(",");
                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.Names)).Append(",");

                sb.Append(MakeValueCsvFriendly(project.ProjectInfo.ResearchRemark)).Append(",");

            }

            return sb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sb"></param>
        private void MakeBlankRow(StringBuilder sb)
        {
            sb.Append("\r\n");
        }

        //get the csv value for field.
        private string MakeValueCsvFriendly(object value)
        {
            if (value == null) return "";
            if (value is Nullable && ((INullable)value).IsNull) return "";

            if (value is DateTime)
            {
                if (((DateTime)value).TimeOfDay.TotalSeconds == 0)
                    return ((DateTime)value).ToString("yyyy-MM-dd");
                return ((DateTime)value).ToString("yyyy-MM-dd HH:mm:ss");
            }
            string output = value.ToString();

            if (output.Contains(",") || output.Contains("\""))
                output = '"' + output.Replace("\"", "\"\"") + '"';
            output = output.Replace("\n", " ");

            return output;

        }

        #endregion
    }
}

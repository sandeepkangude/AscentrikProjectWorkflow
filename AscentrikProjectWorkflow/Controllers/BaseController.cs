using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AscentrikProjectWorkflow.Filters;
using log4net;
using AscentrikProjectWorkflow.Models;
using AscentrikProjectWorkflow.ViewModel;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace AscentrikProjectWorkflow.Controllers
{
    [CustomHandleError]
    public class BaseController : Controller
    {
        protected readonly ILog _log;
        internal readonly ListModel listModel;
        internal readonly ClientModel clientModel;
        internal readonly UserModel userModel;
        internal readonly ProjectModel projectModel;
        internal readonly DashboardModel dashboardModel;
        public BaseController()
        {
            listModel = new ListModel();
            clientModel = new ClientModel();
            userModel = new UserModel();
            projectModel = new ProjectModel();
            dashboardModel = new DashboardModel();
            _log = LogManager.GetLogger(typeof(BaseController).FullName);
        }

        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal ProjectViewModel BindProjectModel(ProjectViewModel model)
        {

            model.PriorityList = projectModel.ProjectPriorityList();
            model.StatusList = projectModel.ProjectStatusList();
            model.ClientList = projectModel.ClientList();
            model.ListReferenceList = projectModel.ListReferenceList(model.ClientId); //new Dictionary<int, string>();
            model.Choice = new Dictionary<bool, string> { { false, "No" }, { true, "Yes" } };
            model.AllotedToList = new Dictionary<string, string> { { "Vendor", "Vendor" }, { "Inhouse", "Inhouse" } };
            if (model.ExpectedDeliveryDate != null)
                model.ExpectedDeliveryDateText = model.ExpectedDeliveryDate.Value.ToString("dd/MM/yyyy");
            if (model.ActualDeliveryDate != null)
                model.ActualDeliveryDateText = model.ActualDeliveryDate.Value.ToString("dd/MM/yyyy");
            if (model.ApprovalDate != null)
                model.ApprovalDateText = model.ApprovalDate.Value.ToString("dd/MM/yyyy");
            if (model.FTPUploadDate != null)
                model.FTPUploadDateText = model.FTPUploadDate.Value.ToString("dd/MM/yyyy");

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        internal ProjectCostingViewModel BindProjectCostingModel(ProjectCostingViewModel model)
        {
            if (model == null)
                model = new ProjectCostingViewModel();
            model.CurrencyList = projectModel.CurrencyList();
            model.PaymentStatusList = projectModel.PaymentStatusList();
            if (model.InvoiceDate != null)
                model.InvoiceDateText = model.InvoiceDate.Value.ToString("dd/MM/yyyy");
            model.ProjectCostingId = model.Id;

            return model;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="clientId"></param>
        /// <returns></returns>
        public JsonResult GetListReferenceByClient(int clientId)
        {
            var lists = projectModel.ListReferenceList(clientId);
            var returnData = lists.Select(m => new SelectListItem()
            {
                Text = m.Value,
                Value = m.Key.ToString(),
            });
            return Json(returnData, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="maxSize"></param>
        /// <returns></returns>
        internal static string GetUniqueKey(int maxSize)
        {
            char[] chars = new char[62];
            chars =
            "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();
            byte[] data = new byte[1];
            RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider();
            crypto.GetNonZeroBytes(data);
            data = new byte[maxSize];
            crypto.GetNonZeroBytes(data);
            StringBuilder result = new StringBuilder(maxSize);
            foreach (byte b in data)
            {
                result.Append(chars[b % (chars.Length)]);
            }
            return result.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        internal static bool ValidateEmailAddress(string email)
        {
            Regex mailIDPattern = new Regex(@"[\w-]+@([\w-]+\.)+[\w-]+");
            if (!string.IsNullOrEmpty(email) && mailIDPattern.IsMatch(email))
            {
                return true;
            }
            return false;
        }
    }
}

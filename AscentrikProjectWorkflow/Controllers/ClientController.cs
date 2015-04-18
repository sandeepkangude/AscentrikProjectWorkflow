using AscentrikProjectWorkflow.Models;
using AscentrikProjectWorkflow.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AscentrikProjectWorkflow.Controllers
{
    [Authorize]
    public class ClientController : BaseController
    {
        //
        // GET: /Client/

        public ActionResult Index()
        {
            var model = new List<ClientViewModel>();
            model = clientModel.GetClientList();

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(ClientViewModel model)
        {
            
            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.CreatedOn = DateTime.Now;
                var result = clientModel.AddClient(model);
                if (result == 0)
                {
                    return Json(new { status = true, message = "Record is added successfully." });
                }
                else if (result == 1)
                {
                    return Json(new { status = false, message = "Please enter all required values." });
                }
                else if (result == 2)
                {
                    return Json(new { status = false, message = "This client code is already present in the database." });
                }
                else
                {
                    return Json(new { status = false, message = "Record is not added successfully." });
                }
            }
            return Json(new { status = false, message = "Please enter all required values." });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult Edit(int id)
        {
            var model = clientModel.GetClientById(id);
            
            return PartialView("_Partial_ClientEditor", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(ClientViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.EditedBy = Convert.ToInt32(userId);
                model.EditedOn = DateTime.Now;
                var result = clientModel.EditClient(model);
                if (result == 0)
                {
                    return Json(new { status = true, message = "Record is edited successfully." });
                }
                else if (result == 1)
                {
                    return Json(new { status = false, message = "Please enter all required values." });
                }
                else if (result == 2)
                {
                    return Json(new { status = false, message = "This client code is already present in the database." });
                }
                else
                {
                    return Json(new { status = false, message = "Record is not edited successfully." });
                }
            }
            return Json(new { status = false, message = "Please enter all required values." });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Delete(int id)
        {
            var result = clientModel.DeleteClient(id);
            if (result == 0)
            {
                return Json(new { status = true, message = "Record is deleted successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Invalid client." });
            }
            else if (result == 2)
            {
                return Json(new { status = false, message = "This client is already associated with some lists. You cannot removed this client" });
            }
            else
            {
                return Json(new { status = false, message = "Sorry some error occured." });
            }
        }

    }
}

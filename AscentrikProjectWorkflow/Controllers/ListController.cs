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
    public class ListController : BaseController
    {
        public ListController()
        {}

        
        #region LIST TYPE

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ListTypes()
        {
            var lstLists = listModel.GetListTypes();
            return View(lstLists);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateListType()
        {
            var model = new ListTypeViewModel();
            return PartialView("CreateListType", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateListType(ListTypeViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.CreatedOn = DateTime.Now;
                var result = listModel.AddListType(model);
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
                    return Json(new { status = false, message = "This list type is already present in the database." });
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
        /// <returns></returns>
        public ActionResult EditListType(int id)
        {
            var model = listModel.GetListTypeById(id);
            return PartialView("EditListType", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditListType(ListTypeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.EditedBy = Convert.ToInt32(userId);
                model.EditedOn = DateTime.Now;
                var result = listModel.EditListType(model);
                if (result == 0)
                {
                    return Json(new { status = true, message = "Record is updated successfully." });
                }
                else if (result == 1)
                {
                    return Json(new { status = false, message = "Please enter all required values." });
                }
                else if (result == 2)
                {
                    return Json(new { status = false, message = "This list type is already present in the database." });
                }
                else if (result == 3)
                {
                    return Json(new { status = false, message = "This list type you are trying to modify is not present in the database." });
                }
                else if (result == 4)
                {
                    return Json(new { status = false, message = "Cannot inactive this list type. Because it is assigened to few Lists." });
                }
                else
                {
                    return Json(new { status = false, message = "Sorry, Record is not updated successfully." });
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
        public ActionResult DeleteListType(int id)
        {
            var result = listModel.DeleteListType(id);
            if (result == 0)
            {
                return Json(new { status = true, message = "Record is deleted successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Invalid list type." });
            }
            else if (result == 2)
            {
                return Json(new { status = false, message = "This list type is already associated with some lists. You cannot removed this list type." });
            }
            else
            {
                return Json(new { status = false, message = "Sorry some error occured." });
            }
        }

        #endregion


        #region LIST

        //
        // GET: /List/
        public ActionResult Index()
        {
            var lstLists = listModel.GetLists();
            return View(lstLists);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult CreateList()
        {
            var model = new ListViewModel();
            model.Clients = clientModel.GetActiveClientList().ToDictionary(x => x.Id, x => x.Code); ///new Dictionary<int, string>();
            model.ListTypes = listModel.GetActiveListTypes().ToDictionary(x => x.Id, x => x.Type); ;
            return PartialView("CreateList", model); 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult CreateList(ListViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.CreatedOn = DateTime.Now;
                var result = listModel.AddList(model);
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
                    return Json(new { status = false, message = "This list code is already present in the database." });
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
        /// <returns></returns>
        public ActionResult EditList(int id)
        {
            var model = listModel.GetListById(id);
            model.Clients = clientModel.GetActiveClientList().ToDictionary(x => x.Id, x => x.Code); ///new Dictionary<int, string>();
            model.ListTypes = listModel.GetActiveListTypes().ToDictionary(x => x.Id, x => x.Type); ;
            return PartialView("EditList", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult EditList(ListViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.EditedBy = Convert.ToInt32(userId);
                model.EditedOn = DateTime.Now;
                var result = listModel.EditList(model);
                if (result == 0)
                {
                    return Json(new { status = true, message = "Record is updated successfully." });
                }
                else if (result == 1)
                {
                    return Json(new { status = false, message = "Please enter all required values." });
                }
                else if (result == 2)
                {
                    return Json(new { status = false, message = "This list code is already present in the database." });
                }
                else if (result == 3)
                {
                    return Json(new { status = false, message = "This list you are trying to modify is not present in the database." });
                }
                else if (result == 4)
                {
                    return Json(new { status = false, message = "Cannot change the client of this list. Because it is assigened to few projects." });
                }
                else if (result == 5)
                {
                    return Json(new { status = false, message = "Cannot inactive this list. Because it is assigened to few projects." });
                }
                else
                {
                    return Json(new { status = false, message = "Sorry, Record is not updated successfully." });
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
        public ActionResult DeleteList(int id)
        {
            var result = listModel.DeleteList(id);
            if (result == 0)
            {
                return Json(new { status = true, message = "Record is deleted successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Invalid list." });
            }
            else if (result == 2)
            {
                return Json(new { status = false, message = "This list is already associated with some projects. You cannot removed this list." });
            }
            else
            {
                return Json(new { status = false, message = "Sorry some error occured." });
            }
        }

        #endregion
    }
}

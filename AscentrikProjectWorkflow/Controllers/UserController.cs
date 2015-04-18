using AscentrikProjectWorkflow.Helper;
using AscentrikProjectWorkflow.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Tesseris.Web.SimpleSecurity;

namespace AscentrikProjectWorkflow.Controllers
{
    [Authorize]
    public class UserController : BaseController
    {
        //
        // GET: /User/

        public ActionResult Index()
        {
            var model = userModel.GetUserList();
            return View(model);
        }

        public ActionResult Create()
        {
            var model = new UserViewModel();
            model.Roles = userModel.GetRoleList();
            return PartialView("Create", model);
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Create(UserViewModel model)
        {

            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.CreatedBy = Convert.ToInt32(userId);
                model.CreatedDate = DateTime.Now;

                var password = GetUniqueKey(8);

                var result = SimpleSecurityProvider.Current.Register(model.EmailAddress, password, model.Role.ToString(), userId, model.IsActive);
                if (result)
                {
                    var name = model.EmailAddress.Split('@').Length > 0 ? model.EmailAddress.Split('@')[0] : "User";
                    EmailHelper.SendRegisterUserEmail(name, password, model.EmailAddress);
                    return Json(new { status = true, message = "User is added successfully." });
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
            var model = userModel.GetUserById(id);
            model.Roles = userModel.GetRoleList();
            return PartialView("Edit", model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Edit(UserViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userId = Request.Cookies["auth.id"].Value;
                model.UpdatedBy = Convert.ToInt32(userId);
                model.LastUpdatedDate = DateTime.Now;
                var result = userModel.EditUser(model);
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
                    return Json(new { status = false, message = "This Email Address is already present in the database." });
                }
                else if (result == 3)
                {
                    return Json(new { status = false, message = "The user you are trying to modify is not present in the database." });
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
        public ActionResult Delete(int id)
        {
            var result = userModel.DeleteUser(id);
            if (result == 0)
            {
                return Json(new { status = true, message = "Record is deleted successfully." });
            }
            else if (result == 1)
            {
                return Json(new { status = false, message = "Invalid User." });
            }
            else if (result == 2)
            {
                return Json(new { status = false, message = "This user is already associated with some projects. You cannot removed this list." });
            }
            else
            {
                return Json(new { status = false, message = "Sorry some error occured." });
            }
        }


        public ActionResult Manage()
        {
            var userId = Request.Cookies["auth.id"].Value;
            var model = userModel.GetUserById(Convert.ToInt32(userId));
            return View(model);
        }

        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            model.UserId = Convert.ToInt32(Request.Cookies["auth.id"].Value);
            model.EmailAddress = Request.Cookies["auth.user"].Value;
            return PartialView("_Partial_ChangePassword", model);
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
               var result = SimpleSecurityProvider.Current.SetUserPassword(model.EmailAddress, model.OldPassword, model.NewPassword);
                
                if (result)
                {
                    var name = model.EmailAddress.Split('@')[0];
                    EmailHelper.SendChangePasswordEmail(name, model.NewPassword, model.EmailAddress);
                    return Json(new { status = true, message = "Password is changed successfully." });
                }
                else
                {
                    return Json(new { status = false, message = "Sorry, password is not changed successfully." });
                }
            }
            return Json(new { status = false, message = "Please enter all required values." });
        }
    }
}

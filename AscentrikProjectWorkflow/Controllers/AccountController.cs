using AscentrikProjectWorkflow.Filters;
using AscentrikProjectWorkflow.Helper;
using AscentrikProjectWorkflow.Models;
using System.Web.Mvc;
using Tesseris.Web.SimpleSecurity;

namespace AscentrikProjectWorkflow.Controllers
{
    [Authorize]
    [InitializeSimpleMembership]
    public class AccountController : BaseController
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (SimpleSecurityProvider.Current.Login(model.UserName, model.Password, model.RememberMe, 1))
                {
                    return RedirectToAction("Index", "Dashboard");
                }

                ModelState.AddModelError("", "Invalid username or password.");
            }
            return View();
        }

        //
        // POST: /Account/LogOff

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            //WebSecurity.Logout();
            SimpleSecurityProvider.Current.Logout();
            return RedirectToAction("Login", "Account");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult Register(RegisterModel model)
        {
            //if (ModelState.IsValid)
            //{
            //    // Attempt to register the user
            //    try
            //    {
            //        WebSecurity.CreateUserAndAccount(model.UserName, model.Password);
            //        WebSecurity.Login(model.UserName, model.Password);
            //        return RedirectToAction("Index", "Home");
            //    }
            //    catch (MembershipCreateUserException e)
            //    {
            //        ModelState.AddModelError("", ErrorCodeToString(e.StatusCode));
            //    }
            //}

            //// If we got this far, something failed, redisplay form
            //return View(model);
            if (ModelState.IsValid)
            {
                if (SimpleSecurityProvider.Current.Register(model.UserName, model.Password, "1", "1", true))
                {
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Using with such name already registered.");
            }

            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        /// 
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ForgotPassword(string email)
        {
            if(!ValidateEmailAddress(email))
                return Json(new { status = false, message = "Invalid email address." });

            var password = GetUniqueKey(8);
            var result = SimpleSecurityProvider.Current.ChangePassword(email, password);
            if (result == 0)
            {
                var user = email.Split('@')[0];
                EmailHelper.SendChangePasswordEmail(user, password, email);
                return Json(new { status = true, message = "Email with new password has been sent." });
            }
            else if (result == 1)
                return Json(new { status = false, message = "Provide email id is not present in the database. Please try with registered email id only." });

            return Json(new { status = false, message = "Sorry some error occured. Please try again later" });
        }

    }
}

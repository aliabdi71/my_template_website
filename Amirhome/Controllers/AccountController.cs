using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Mvc;
using System.IO;
using System.Data.Entity;
using Amirhome.Models;
using System.Web.Security;

namespace Amirhome.Controllers
{
    public class AccountController : Controller
    {
        UserManager _userManager = new UserManager();
        AgentManager _agentManager = new AgentManager();
        public ActionResult RegisterView()
        {
            if (Session["UserID"] != null)
                return RedirectToAction("Index", "Home");
            return View();
        }

        [HttpPost]
        public ActionResult RegisterView(UserAccouunt model)
        {
            try
            {
                if (Request.Files.Count > 0)
                {
                    HttpPostedFileBase img = Request.Files[0];
                    byte[] data = null;
                    long numBytes = img.ContentLength;
                    BinaryReader bin_reader = new BinaryReader(img.InputStream);
                    data = bin_reader.ReadBytes((int)numBytes);
                    model.ProfileImage = data;
                    if (model.RoleID == null)
                        model.RoleID = 5;
                    model.CreateDate = DateTime.Now;
                    model.LastTimeOnline = DateTime.Now;
                }
                model.Approved = true;
                bool res = _userManager.createNewUser(model);
                if (res)
                    ViewBag.Message = "حساب کاربری با موفقیت ثبت گردید";
                else
                    ModelState.AddModelError("", "کاربر با این آدرس ایمیل قبلاً ثبت شده است");
            }
            catch
            {
                ModelState.AddModelError("", "خطا در ثبت حساب کاربری");
            }
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(string email, string password, bool remember)
        {
            try
            {
                int id = _userManager.authenticateUser(email, password);
                if (id < 0)
                {
                    return Json("ایمیل یا کلمه عبور اشتباه است");
                }
                else
                {
                    Session["UserID"] = id;
                    if (remember)
                    {
                        var userEmailCookie = new HttpCookie("AmirhomeUserEmail", email);
                        var userPassCookie = new HttpCookie("AmirhomeUserPass", password);
                        userEmailCookie.Expires.AddDays(365);
                        userPassCookie.Expires.AddDays(365);
                        HttpContext.Request.Cookies.Add(userEmailCookie);
                        HttpContext.Request.Cookies.Add(userPassCookie);
                    }
                    return Json("success");
                }
            }
            catch
            {
                return Json("خطا در برقراری ارتباط با سرور");
            }
        }

        public JsonResult getUserInfo(int id)
        {
            if (Session["user_role_id"] == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            UserAccouunt user = _userManager.getUserByID(id);
            var res_object = new
            {
                ID = user.ID,
                Role = user.UserAccouuntsRole.Name,
                Name = user.Name,
                LastOnline = (user.LastTimeOnline != null) ? user.LastTimeOnline.ToString().Split(' ')[0] : "2016/01/01",
                imageUrl = string.Format("data:image/jpg;base64,{0}", Convert.ToBase64String(user.ProfileImage)),
                Phone = user.Phone
            };
            return Json(res_object, JsonRequestBehavior.AllowGet);
        }

        public JsonResult deleteUser(int id)
        {
            if (Session["user_role_id"] == null)
                return Json("error", JsonRequestBehavior.AllowGet);
            bool res = _userManager.deleteUser(id);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }
        public JsonResult getAgentInfo(int id)
        {
            if (Session["user_role_id"] == null)
                return Json(null, JsonRequestBehavior.AllowGet);
            User agent = _agentManager.getAgentById(id);
            var res_object = new 
            {
                DisplayName = agent.DisplayName,
                UserID = agent.UserID,
                Email = agent.Email,
            };
            return Json(res_object, JsonRequestBehavior.AllowGet);
        }

        public JsonResult deleteAgent(int id)
        {
            if (Session["user_role_id"] == null)
                return Json("error", JsonRequestBehavior.AllowGet);
            bool res = _agentManager.deleteAgent(id);
            if (res)
                return Json("Success", JsonRequestBehavior.AllowGet);
            else
                return Json("Error", JsonRequestBehavior.AllowGet);
        }
    }
}
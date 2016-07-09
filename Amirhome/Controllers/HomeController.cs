using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;

namespace Amirhome.Controllers
{
    public class HomeController : Controller
    {
        UserManager _userManager = new UserManager();
        EstateManager _estateManager = new EstateManager();
        public ActionResult Index()
        {
            if (Session["UserID"] != null)
            {
                if (Session["user_name"] == null)
                {
                    try
                    {
                        int uid = int.Parse(Session["UserID"].ToString());
                        UserAccouunt user = _userManager.getUserByID(uid);
                        string imageBase64 = Convert.ToBase64String(user.ProfileImage);
                        string imageSrc = string.Format("data:image/jpg;base64,{0}", imageBase64);
                        Session["user_name"] = user.Name;
                        Session["user_image_src"] = imageSrc;
                    }
                    catch
                    {
                        Session["user_name"] = null;
                        Session["user_image_src"] = null;
                    }
                }
                else
                {
                    ViewData["user_name"] = Session["user_name"];
                    ViewData["user_image_src"] = Session["user_image_src"];
                }
            }
            return View();
        }

        public JsonResult GetOccasionEstates()
        {
            List<State> occasions = _estateManager.getOccasions();
            return Json(occasions.OrderByDescending(E => E.Date).Take(5));
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
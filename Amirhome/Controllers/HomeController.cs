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

        [HttpGet]
        public ActionResult GetOccasionEstates()
        {
            List<State> occasions = _estateManager.getOccasions().OrderByDescending(E => E.Date).Take(4).ToList();
            var data = occasions.Select(o => new { 
                ID = o.ID,
                Area = o.Area,
                TotalPrice = o.TotalPrice,
                Date = o.Date.ToString().Replace("PM", "").Replace("AM", ""),
                Prepayment = o.PrepaymentPrice,
                Loan = o.Loan,
                Mortage = o.MortgagePrice,
                Condition = o.Condition,
                Address = o.Address,
                Occasion = o.Occasion,
                ImageSrc = (o.Images.Count(i => i.Primary == true) == 0 ? "no-thumb.png" : o.Images.FirstOrDefault(i => i.Primary == true).url),
                ImageCount = o.Images.Count
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
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
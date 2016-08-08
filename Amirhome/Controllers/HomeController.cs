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

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            base.OnActionExecuting(filterContext);
            if (Session["UserID"] == null)
            {
                var userEmail = Request.Cookies["AmirhomeUserEmail"];
                if (userEmail != null)
                {
                    var userPass = Request.Cookies["AmirhomeUserPass"];
                    int id = _userManager.authenticateUser(userEmail.Value, userPass.Value);
                    if (id > 0)
                    {
                        Session["UserID"] = id;
                    }
                }
            }
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
                        Session["user_role_id"] = user.UserAccouuntsRole.ID;
                        Session["user_role_access"] = user.UserAccouuntsRole.Username;
                    }
                    catch
                    {
                        Session["user_name"] = null;
                        Session["user_image_src"] = null;
                    }
                }
                ViewData["user_name"] = Session["user_name"];
                ViewData["user_image_src"] = Session["user_image_src"];
                ViewData["user_role_id"] = Session["user_role_id"];
            }
        }

        UserManager _userManager = new UserManager();
        EstateManager _estateManager = new EstateManager();
        AdvertiseManager _advertiseManager = new AdvertiseManager();
        public ActionResult Index()
        {
            ViewData["Province"] = _estateManager.getAllProvince();
            var advers = _advertiseManager.getAdvertises();
            List<AdverShowModelView> model = new List<AdverShowModelView>();
            foreach (var adver in advers)
            {
                string final_date = "",
                       difference = ((int)(DateTime.Now - adver.create_date.Value).TotalDays).ToString();
                switch (difference)
                {
                    case "0": final_date = "لحظاتی قبل"; break;
                    case "1": final_date = "یک روز قبل"; break;
                    case "2": final_date = "دو روز قبل"; break;
                    case "3": final_date = "سه روز قبل"; break;
                    case "4": final_date = "چهار روز قبل"; break;
                    default: final_date = gregorianToJalali(adver.create_date.Value.ToString().Split(' ')[0]); break;
                }
                model.Add(new AdverShowModelView()
                {
                    Title = adver.title,
                    Condition = adver.condition,
                    Date = final_date,
                    District = adver.district,
                    FirstPrice = adver.condition == "فروش" ? adver.price_total.ToString() : adver.price_prepayment.ToString(),
                    SecondPrice = adver.condition == "فروش" ? adver.price_per_meter.ToString() : adver.price_mortage.ToString(),
                    ImgUrl = string.IsNullOrEmpty(adver.image) ? "no-thumb.png" : adver.image.Split(';')[0],
                });
            }
            ViewData["adverModel"] = model;
            return View();
        }

        [HttpGet]
        public ActionResult GetOccasionEstates()
        {
            List<State> occasions = _estateManager.getOccasions().Take(4).ToList();
            var data = occasions.Select(o => new { 
                ID = o.ID,
                Area = o.Area,
                TotalPrice = o.TotalPrice,
                Date = o.Date.ToString().Split(' ')[0],
                Prepayment = o.PrepaymentPrice,
                Loan = o.Loan,
                Mortage = o.MortgagePrice,
                Condition = o.Condition,
                Address = o.Address,
                Occasion = o.Occasion,
                ImageSrc = (o.Images.Count() == 0 ? "no-thumb.png" : o.Images.FirstOrDefault().url),
                ImageCount = o.Images.Count
            }).ToList();
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult SignOut()
        {
            int id = int.Parse(Session["UserID"].ToString());
            bool success = _userManager.refreshLastOnline(id);
            Session.Abandon();
            return RedirectToAction("Index", "Home");
        }



        private string gregorianToJalali(string date)
        {
            string[] splited = date.Split('/');
            int m = int.Parse(splited[0]),
                d = int.Parse(splited[1]),
                y = int.Parse(splited[2]);
            int[] g_days_in_month = { 31, 28, 31, 30, 31, 30, 31, 31, 30, 31, 30, 31 };
            int[] j_days_in_month = { 31, 31, 31, 31, 31, 31, 30, 30, 30, 30, 30, 29 };
            int gy = y - 1600;
            int gm = m - 1;
            int gd = d - 1;
            int g_day_number = (365 * gy) + ((gy + 3) / 4) - ((gy + 99) / 100) + ((gy + 399) / 400);
            for (int i = 0; i < gm; i++)
                g_day_number += g_days_in_month[i];
            if (gm > 1 && (((gy % 4 == 0) && (gy % 100 != 0)) || (gy % 400 != 0)))
                ++g_day_number;
            g_day_number += gd;

            int j_day_number = g_day_number - 79;
            int j_np = j_day_number / 12053;
            j_day_number %= 12053;
            int jy = 979 + (33 * j_np) + 4 * (j_day_number / 1461);
            j_day_number %= 1461;
            if (j_day_number >= 366)
            {
                jy += (j_day_number - 1) / 365;
                j_day_number = (j_day_number - 1) % 365;
            }
            int index;
            for (index = 0; index < 11 && j_day_number >= j_days_in_month[index]; index++)
            {
                j_day_number -= j_days_in_month[index];
            }
            int jm = index + 1;
            int jd = j_day_number + 1;
            string result = jy.ToString() + "/" + jm.ToString() + "/" + jd.ToString();

            return result;
        }
    }
}
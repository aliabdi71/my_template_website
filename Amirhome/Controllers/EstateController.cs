using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Amirhome.Models;

namespace Amirhome.Controllers
{
    public class EstateController : Controller
    {
        UserManager _userManager = new UserManager();
        EstateManager _estateManager = new EstateManager();

        public ActionResult EstateDetails()
        {
            int ID = int.Parse(Request.QueryString["EstateID"].ToString());
            State estate_model = _estateManager.getStateByID(ID);
            /*if (estate_model.AgentID != null)
            {
                Agent estate_agent = _userManager.getAgentByID((int)estate_model.AgentID);
                ViewData["Agent"] = estate_agent;
            }*/
            return View(estate_model);
        }
        public ActionResult SearchPage()
        {
            return View();
        }

        [HttpPost]
        public JsonResult DoSearch(SearchParams _params, int page, string order = "date")
        {
            List<State> states = _estateManager.doSearch(_params, page * 10, order);
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                TotalPrice = o.TotalPrice,
                PricePerMeter = o.PricePerMeter,
                Date = o.Date.ToString().Split(' ')[0],
                Prepayment = o.PrepaymentPrice,
                Loan = o.Loan,
                Mortage = o.MortgagePrice,
                Address = o.Address,
                Floor = o.Floor,
                Age = o.Age,
                Condition = o.Condition,
                Dist = o.District1.name,
                Bedrooms = o.Bedrooms,
                Serial = o.Serial,
                ImageSrc = (o.Images.Count(i => i.Primary == true) == 0 ? "no-thumb.png" : o.Images.FirstOrDefault(i => i.Primary == true).url),
                ImageCount = o.Images.Count
            }).ToList();

            if (data.Count() % 10 == 0)
            {
                data = data.Skip(Math.Max(0, data.Count() - 10)).ToList();
            }
            else
            {
                data = data.Skip(Math.Max(0, data.Count() - (data.Count() % 10))).ToList();
            }

            return Json(data);
        }

        public ActionResult SubmitEstate()
        {
            ViewData["Features"] = _estateManager.getAllFeatures();
            return View();
        }
    }
}
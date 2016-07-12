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
        // GET: Estate
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
        public JsonResult DoSearch(SearchParams _params)
        {
            List<State> states = _estateManager.doSearch(_params);
            var data = states.Select(o => new
            {
                ID = o.ID,
                Area = o.Area,
                TotalPrice = o.TotalPrice,
                Date = o.Date.ToString().Replace("PM", "").Replace("AM", ""),
                Prepayment = o.PrepaymentPrice,
                Loan = o.Loan,
                Mortage = o.MortgagePrice,
                Address = o.Address,
                Floor = o.Floor,
                Age = o.Age,
                Bedrooms = o.Bedrooms,
                Serial = o.Serial,
                ImageSrc = (o.Images.Count(i => i.Primary == true) == 0 ? "no-thumb.png" : o.Images.FirstOrDefault(i => i.Primary == true).url),
                ImageCount = o.Images.Count
            }).ToList();
            return Json(data);
        }
    }
}
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
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Amirhome
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
        protected void Session_End(object sender, EventArgs e)
        {
            Amirhome.Models.UserManager um = new Models.UserManager();
            if (Session["UserID"] != null)
            {
                int id = int.Parse(Session["UserID"].ToString());
                bool success = um.refreshLastOnline(id);
            }
            Session.Abandon();
        }
    }
}

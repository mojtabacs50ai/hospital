using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Threading;
using System.Globalization;

namespace hospital
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_BeginRequest()
        {
            if(Request.Cookies["lang"] != null)
            {
                if (Request.Cookies["lang"].Value != null)
                {
                    string langg = Request.Cookies["lang"].Value.ToString();
                    Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(langg);
                    Thread.CurrentThread.CurrentUICulture = new CultureInfo(langg);
                }
            }
        }

    }
}

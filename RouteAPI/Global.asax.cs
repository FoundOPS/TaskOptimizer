using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using ServiceStack.ServiceHost;
using ServiceStack.WebHost.Endpoints;
using ServiceStack.Common;
using ServiceStack.WebHost;
using ServiceStack.ServiceInterface.ServiceModel;
using ServiceStack;
using ServiceStack.WebHost.Endpoints.Support;
using System.Net;


namespace RouteAPI
{
    public class Global : System.Web.HttpApplication
    {
        public class RouteAPIHost : AppHostBase
        {
            //Tell Service Stack the name of your application and where to find your web services
            public RouteAPIHost() : base("Routing API", typeof(RouteAPIService).Assembly) { }

            public override void Configure(Funq.Container container)
            {
                //register user-defined REST-ful urls
                Routes
                  .Add<RouteAPI>("/route")
                  .Add<RouteAPI>("/route/{Loc}");
            }
        }

        protected void Application_Start(object sender, EventArgs e)
        {
            new RouteAPIHost().Init();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {

        }
    }
}
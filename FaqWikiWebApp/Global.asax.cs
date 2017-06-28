using SitefinityWebApp.Mvc.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Telerik.Sitefinity.Modules.Forms.Events;
using Telerik.Sitefinity.Services;

namespace SitefinityWebApp
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            Telerik.Sitefinity.Abstractions.Bootstrapper.Initialized += Bootstrapper_Initialized;
        }
        protected void Bootstrapper_Initialized(object sender, Telerik.Sitefinity.Data.ExecutedEventArgs args)
        {
            if (args.CommandName == "Bootstrapped")
            { EventHub.Subscribe<FormSavedEvent>(evt => FormEventHandler.Handler(evt)); }
        }
    }
}
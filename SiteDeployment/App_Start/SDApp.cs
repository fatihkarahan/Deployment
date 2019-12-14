using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Deployment;

namespace SiteDeployment.App_Start
{
    public class SDApp
    {
        public static Deployment.DeployManager Dmanager = null;

        public static void Init()
        {
            Dmanager = new DeployManager(HttpContext.Current.Server.MapPath("DeploymentSettings.xml"));
        }
    }
}
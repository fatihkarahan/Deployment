using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using Deployment;
using SiteDeployment.App_Start;

namespace SiteDeployment.Controllers
{
    public class DefaultController : Controller
    {
        public ActionResult Index()
        {
            ViewData["projects"] = SDApp.Dmanager.GetProjects();

            return View();
        }

        public JsonResult GetServers(string key)
        {
            return Json(SDApp.Dmanager.GetServers(key), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequests(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.GetRequestsPerSec(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Backup(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Backup(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Restore(string server, string filename)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Restore(dd[1], dd[0], filename), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetBackupFiles(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.GetBackupFiles(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Create(string project)
        {
            return Json(SDApp.Dmanager.Create(project), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Check(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Check(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Deploy(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Deploy(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Open(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Open(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Reset(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Reset(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        public JsonResult Close(string server)
        {
            string[] dd = server.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
            return Json(SDApp.Dmanager.Close(dd[1], dd[0]), JsonRequestBehavior.AllowGet);
        }

        //public JsonResult dser()
        //{
        //    DeploymentSettings settings = null;
        //    XmlSerializer ser = new XmlSerializer(typeof(List<Server>));

        //    FileStream fs = new FileStream(Server.MapPath("deploymentsettings.xml"), FileMode.Open);

        //    settings = (DeploymentSettings)ser.Deserialize(fs);

        //    fs.Close();

        //    return Json("dfd", JsonRequestBehavior.AllowGet);
        //}

        //public JsonResult ser()
        //{

        //    XmlSerializer ser = new XmlSerializer(typeof(DeploymentSettings));

        //    DeploymentSettings ds = new DeploymentSettings();

        //    ds.Servers = new List<Deployment.Server>();


        //    ds.Servers.Add(new Server { Name = "aa", Site = "bb" });
        //    ds.Servers.Add(new Server { Name = "aa1", Site = "bb1" });
        //    ds.Servers.Add(new Server { Name = "aa3", Site = "bb3\\." });

        //    ds.BackupParams = " -source:contentpath='yeni.dr.com.tr',computerName='{0}',userName='WDeployUser',password='1qaz-2wsx',includeAcls='False' -dest:package={1}, -verb:sync -disableLink:AppPoolExtension -disableLink:FrameworkConfigExtension -allowUntrusted -disableLink:CertificateExtension -skip:objectName=dirPath,absolutePath=\"content\\\\uploads|facebook\" -skip:objectName=filePath,absolutePath=\".*\\.xml\"";

        //    ds.WMIPath = @"\\{0}\root\CIMV2";

        //    FileStream fs = new FileStream(@"c:\server.xml", FileMode.Create);

        //    ser.Serialize(fs, ds);

        //    fs.Close();

        //    return Json("dfd", JsonRequestBehavior.AllowGet);
        //}

    }
}

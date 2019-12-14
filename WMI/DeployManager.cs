using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Management;
using System.Security;
using System.Web;
using System.Xml.Serialization;

namespace Deployment
{
    public class DeployManager
    {
        private Dictionary<string, ManagementScope> scopes = new Dictionary<string, ManagementScope>();

        private Dictionary<string, ManagementObjectSearcher> searchers = new Dictionary<string, ManagementObjectSearcher>();

        private DeploymentSettings settings;

        private Dictionary<string, List<decimal>> requests = new Dictionary<string, List<decimal>>();
        public DeployManager(string settingsFile)
        {
            XmlSerializer ser = new XmlSerializer(typeof(DeploymentSettings));

            FileStream fs = new FileStream(settingsFile, FileMode.Open);

            settings = (DeploymentSettings)ser.Deserialize(fs);

            fs.Close();
        }

        private ManagementScope getScope(string server)
        {
            ManagementScope ret;

            bool exists = scopes.TryGetValue(server, out ret);

            if (!exists)
            {
                ConnectionOptions co = new ConnectionOptions { Username = settings.WMIUser, Password = settings.WMIPassword, Authority = settings.WMIAuthority };
                ret = new ManagementScope(string.Format(settings.WMIPath, server), co);
                ret.Connect();

                scopes.Add(server, ret);
            }

            return ret;
        }

        public List<Project> GetProjects()
        {
            return settings.Projects;
        }

        public Project GetProject(string projectname)
        {
            return settings.Projects.Where(w => w.Name == projectname).FirstOrDefault();
        }

        public List<Server> GetServers(string projectName)
        {
            return settings.Projects.Where(w => w.Name == projectName).FirstOrDefault().Servers;
        }

        public decimal GetRequestsPerSec(string server, string projectName)
        {
            //Server ss = GetProject(projectName).Servers.Where(w => w.Name == server).FirstOrDefault();

            Project p = GetProject(projectName);

            ObjectQuery q = new ObjectQuery(string.Format(settings.WMIQuery, p.Site));
            ManagementObjectSearcher mos;
            bool exists = searchers.TryGetValue(projectName + server, out mos);

            if (!exists)
            {
                mos = new ManagementObjectSearcher(getScope(server), q);
                searchers.Add(projectName + server, mos);
            }

            List<decimal> reqs;

            exists = requests.TryGetValue(projectName + server, out reqs);

            if (!exists)
            {
                reqs = new List<decimal>();
                requests.Add(projectName + server, reqs);
            }


            decimal ret = 0;

            foreach (var item in mos.Get())
            {
                ret = decimal.Parse(item["CurrentConnections"].ToString());
            }

            reqs.Add(ret);

            if (reqs.Count >= 10)
                reqs.RemoveAt(0);

            return Math.Round(reqs.Average(), 2);
        }

        public string Open(string server, string key)
        {
            string cmd = string.Format("{0}\\{2}\\open.bat {1}", settings.DeploymentRootFolder, server, key);

            return RunDosCommand(cmd);
        }

        public string Reset(string server, string key)
        {
            string cmd = string.Format("{0}\\{2}\\reset.bat {1}", settings.DeploymentRootFolder, server, key);

            return RunDosCommand(cmd);
        }

        public string Close(string server, string key)
        {
            string cmd = string.Format("{0}\\{2}\\close.bat {1}", settings.DeploymentRootFolder, server, key);

            return RunDosCommand(cmd);
        }


        public string Backup(string server, string projectName)
        {
            Project p = GetProject(projectName);

            string backupfile = string.Format("{0}\\{1}\\{2}_backup\\{1}_{3:yyyyMMddHHmmss}.zip", settings.DeploymentRootFolder, projectName, server, DateTime.Now);
            string cmd = string.Format("\"" + settings.MsDeployPath + "\"" + settings.BackupParams, p.Site, server, backupfile, p.BackupSkip);

            return RunDosCommand(cmd);
        }

        public List<string> GetBackupFiles(string server, string projectName)
        {
            DirectoryInfo di = new DirectoryInfo(string.Format("{0}\\{1}\\{2}_backup", settings.DeploymentRootFolder, projectName, server));
            FileInfo[] files = di.GetFiles();
            return files.OrderByDescending(f => f.CreationTime).Take(3).Select(ff => new string(ff.Name.ToCharArray())).ToList();
        }

        public object Restore(string server, string projectName, string filename)
        {
            Project p = GetProject(projectName);

            string restorefile = string.Format("{0}\\{1}\\{2}_backup\\{3}", settings.DeploymentRootFolder, projectName, server, filename);

            string cmd = string.Format("\"" + settings.MsDeployPath + "\"" + settings.RestoreParams, restorefile, p.Site, server);

            return RunDosCommand(cmd);
        }

        public string Create(string projectName)
        {
            Project p = GetProject(projectName);

            string packagefile = string.Format("{0}\\{1}\\{1}.zip", settings.DeploymentRootFolder, projectName);
            string cmd = string.Format("\"" + settings.MsDeployPath + "\"" + settings.CreateParams, p.SourceSite, packagefile, p.DeploySkip);

            return RunDosCommand(cmd);
        }

        public string Check(string server, string projectName)
        {
            return Deploy(server, projectName, true);
        }

        public string Deploy(string server, string projectName, bool check = false)
        {
            Project p = GetProject(projectName);

            string packagefile = string.Format("{0}\\{1}\\{1}.zip", settings.DeploymentRootFolder, projectName);
            string cmd = string.Format("\"" + settings.MsDeployPath + "\"" + settings.DeployParams + (check ? " -whatif" : ""), packagefile, p.Site, server, p.DeploySkip);

            return RunDosCommand(cmd);
        }

        private string RunDosCommand(string cmd)
        {

            // Create the ProcessInfo object
            System.Diagnostics.ProcessStartInfo psi =
                    new System.Diagnostics.ProcessStartInfo("cmd.exe");

            psi.UseShellExecute = false;
            psi.RedirectStandardOutput = true;
            psi.RedirectStandardInput = true;
            psi.RedirectStandardError = true;
            //psi.Domain = "drwebdomain";
            //psi.UserName = "wdeployuser";

            //char[] pass = { '1', 'q', 'a', 'z', '-', '2', 'w', 's', 'x' };

            //SecureString ss = new SecureString();
            //foreach (var item in pass)
            //{
            //    ss.AppendChar(item);
            //}
            //psi.Password = ss;


            // Start the process
            System.Diagnostics.Process proc =
                       System.Diagnostics.Process.Start(psi);

            // Open the batch file for reading
            //System.IO.StreamReader strm = 
            //           System.IO.File.OpenText(strFilePath);
            System.IO.StreamReader strm = proc.StandardError;
            // Attach the output for reading
            System.IO.StreamReader sOut = proc.StandardOutput;

            // Attach the in for writing
            System.IO.StreamWriter sIn = proc.StandardInput;

            // Write each line of the batch file to standard input
            //while(strm.Peek() != -1)
            //{
            //    sIn.WriteLine(strm.ReadLine());
            //}
            sIn.WriteLine(cmd);

            // Exit CMD.EXE
            string stEchoFmt = "echo {0} run successfully. Exiting";

            sIn.WriteLine(String.Format(stEchoFmt, cmd));
            sIn.WriteLine("EXIT");

            // Close the process
            proc.Close();

            // Read the sOut to a string.
            string results = sOut.ReadToEnd().Trim();
            results += "<br>";
            results += strm.ReadToEnd().Trim();

            // Close the io Streams;
            sIn.Close();
            sOut.Close();
            strm.Close();

            // Write out the results.
            string fmtStdOut = "<font face=courier size=0>{0}</font><br><br><br>";
            return String.Format(fmtStdOut,
               results.Replace(System.Environment.NewLine, "<br>"));
        }

    }
}
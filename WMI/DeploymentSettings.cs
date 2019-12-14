using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Deployment
{
    public class DeploymentSettings
    {
        [XmlArray(ElementName="Projects")]
        public List<Project> Projects { get; set; }

        public string MsDeployPath { get; set; }

        public string WMIUser { get; set; }

        public string WMIPassword { get; set; }

        public string WMIAuthority { get; set; }

        public string WMIPath { get; set; }

        public string WMIQuery { get; set; }

        public string BackupParams { get; set; }

        public string RestoreParams { get; set; }

        public string CreateParams { get; set; }

        public string DeployParams { get; set; }

        public string DeploymentRootFolder { get; set; }
    }

    public class Project
    {
        [XmlAttribute()]
        public string Name { get; set; }

        [XmlAttribute()]
        public string Site { get; set; }

        [XmlAttribute()]
        public string SourceSite { get; set; }

        [XmlAttribute()]
        public string Desc { get; set; }

        [XmlElement()]
        public string BackupSkip { get; set; }

        [XmlElement()]
        public string DeploySkip { get; set; }

        [XmlArray(ElementName = "Servers")]
        public List<Server> Servers { get; set; }
    }

    public class Server
    {
        [XmlAttribute]
        public string Name { get; set; }

        [XmlAttribute]
        public string WMIRoot { get; set; }

        [XmlAttribute]
        public string Ip { get; set; }

    }
}

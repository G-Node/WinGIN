using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace GinClientLibrary
{
    [DataContract]
    public class GinServerInfo
    {
        [DataMember]
        public string Alias { get; set; }
        [DataMember]
        public WebCfg Web { get; set; }
        [DataMember]
        public GitCfg Git { get; set; }

    }

    public class WebCfg
    {
        [DataMember]
        public string Protocol { get; set; }
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public string Port { get; set; }
    }
    public class GitCfg
    {
        [DataMember]
        public string Host { get; set; }
        [DataMember]
        public string Port { get; set; }
        [DataMember]
        public string User { get; set; }
        [DataMember]
        string HostKey { get; set; }
    }
}

using System;
using System.Runtime.Serialization;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    public class WebSite : IWebSite
    {
        public WebSite()
        {
        }

        public WebSite(IWebSite site)
        {
            FtpPassword = site.FtpPassword;
            FtpUri = site.FtpUri;
            FtpUsername = site.FtpUsername;
            WebSiteUri = site.WebSiteUri;
        }

        [DataMember]
        public Uri WebSiteUri { get; set; }

        [DataMember]
        public Uri FtpUri { get; set; }

        [DataMember]
        public string FtpUsername { get; set; }

        [DataMember]
        public string FtpPassword { get; set; }
    }
}

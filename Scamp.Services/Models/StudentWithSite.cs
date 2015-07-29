using System;
using System.Runtime.Serialization;
using SCAMP.Contracts;

namespace SCAMP.Models
{
    [DataContract]
    public class StudentWithSite : Student, IWebSite
    {
        public StudentWithSite(IStudent student, IWebSite site)
            : base(student)
        {
            if (site != null)
            {
                WebSiteUri = site.WebSiteUri;
                FtpUri = site.FtpUri;
                FtpUsername = site.FtpUsername;
                FtpPassword = site.FtpPassword;
            }
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
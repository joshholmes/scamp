using System;
using Microsoft.WindowsAzure.Storage.Table;
using SCAMP.Contracts;

namespace SCAMP.Azure
{
    public class WebSiteEntity : TableEntity
    {
        internal const string PartitionKeyName = "Website";

        public WebSiteEntity()
        {
            RowKey = Guid.NewGuid().ToString();
            PartitionKey = PartitionKeyName;
        }

        public WebSiteEntity(IWebSite site)
            : this()
        {
            WebSite = site.WebSiteUri.AbsoluteUri;
            FtpSite = site.FtpUri.AbsoluteUri;
            FtpUsername = site.FtpUsername;
            FtpPassword = site.FtpPassword;
        }

        public string WebSite { get; set; }
        public string FtpSite { get; set; }
        public string FtpUsername { get; set; }
        public string FtpPassword { get; set; }
    }
}

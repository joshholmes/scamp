using System;

namespace SCAMP.Contracts
{
    public interface IWebSite : IResource
    {
        Uri WebSiteUri { get; set; }
        Uri FtpUri { get; set; }
        string FtpUsername { get; set; }
        string FtpPassword { get; set; }
    }
}

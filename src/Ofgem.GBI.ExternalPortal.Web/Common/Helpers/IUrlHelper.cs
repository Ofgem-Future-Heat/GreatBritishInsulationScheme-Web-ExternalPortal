using Ofgem.GBI.ExternalPortal.Web.Common.Configuration;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Helpers
{
    public interface IUrlHelper
    {
        string GetPath(string? baseUrl, string path = "");
        string GetPath(IUserContext userContext, string baseUrl, string path = "", string prefix = "accounts");
    }
}

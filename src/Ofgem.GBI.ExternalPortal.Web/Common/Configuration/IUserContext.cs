using System.Security.Principal;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Configuration
{
    public interface IUserContext
    {
        string HashedAccountId { get; set; }
        IPrincipal User { get; set; }
    }
}

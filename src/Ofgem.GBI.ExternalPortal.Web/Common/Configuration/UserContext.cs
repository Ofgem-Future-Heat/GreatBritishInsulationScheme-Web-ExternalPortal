using System.Security.Principal;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Configuration
{
    public class UserContext : IUserContext
    {
        public required string HashedAccountId { get; set; }
        public required IPrincipal User { get; set; }
    }
}

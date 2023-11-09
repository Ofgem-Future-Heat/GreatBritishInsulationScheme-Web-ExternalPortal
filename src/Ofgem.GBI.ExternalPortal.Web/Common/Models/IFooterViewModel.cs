using Ofgem.GBI.ExternalPortal.Web.Common.Helpers;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models
{
    public interface IFooterViewModel : ILinkCollection, ILinkHelper
    {
        bool UseLegacyStyles { get; }
    }
}

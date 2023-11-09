using Ofgem.GBI.ExternalPortal.Web.Common.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Helpers
{
    public interface ILinkHelper
    {
        string RenderListItemLink<T>(bool isSelected = false, string @class = "", string @role = "") where T : Link;
        string RenderLink<T>(Func<string>? before = null, Func<string>? after = null, bool isSelected = false) where T : Link;
    }
}

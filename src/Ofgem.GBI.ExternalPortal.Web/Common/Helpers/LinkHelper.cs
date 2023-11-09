using Ofgem.GBI.ExternalPortal.Web.Common.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Helpers
{
    public class LinkHelper : ILinkHelper
    {
        private readonly ILinkCollection _linkCollection;

        public LinkHelper(ILinkCollection linkCollection)
        {
            _linkCollection = linkCollection;
        }

        public string RenderListItemLink<T>(bool isSelected = false, string @class = "", string @role = "") where T : Link
        {
            return RenderLink<T>(() => $"<li class=\"{@class}\" role=\"{@role}\">", () => "</li>", isSelected);
        }

        public string RenderLink<T>(Func<string>? before = null, Func<string>? after = null, bool isSelected = false) where T : Link
        {
            if (_linkCollection.Links.OfType<T>().FirstOrDefault() != null)
            {
                var link = _linkCollection.Links.OfType<T>().First();
                link.IsSelected = isSelected;

                return $"{before?.Invoke()}{link.Render()}{after?.Invoke()}";
            }

            return string.Empty;
        }
    }
}

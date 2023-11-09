using Ofgem.GBI.ExternalPortal.Web.Common.Helpers;
using Ofgem.GBI.ExternalPortal.Web.Common.Configuration;
using Ofgem.GBI.ExternalPortal.Web.Common.Models.Links;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models
{
    public class FooterViewModel : IFooterViewModel
    {
        public IReadOnlyList<Link> Links => _linkCollection.Links;

        public bool UseLegacyStyles { get; private set; }

        private readonly ILinkCollection _linkCollection;
        private readonly ILinkHelper _linkHelper;

        public FooterViewModel(
            IFooterConfiguration configuration,
            IUserContext userContext,
            ILinkCollection? linkCollection = null,
            ILinkHelper? linkHelper = null,
            bool useLegacyStyles = false)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            if (userContext == null) throw new ArgumentNullException(nameof(userContext));
            if (configuration.ApplicationBaseUrl == null) configuration.ApplicationBaseUrl = "#";

            _linkCollection = new LinkCollection();

            _linkCollection = linkCollection ?? new LinkCollection();
            _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
            var urlHelper = new UrlHelper();
            UseLegacyStyles = useLegacyStyles;

            AddOrUpdateLink(new Accessibility(urlHelper.GetApplicationBasePath("Footer/Accessibility"), GetLinkClass()));
            AddOrUpdateLink(new Cookies(urlHelper.GetPath(configuration.ApplicationBaseUrl, "cookieConsent"), GetLinkClass()));
            AddOrUpdateLink(new Privacy(urlHelper.GetPath(userContext, configuration.ApplicationBaseUrl, "privacy", "service"), GetLinkClass()));
        }

        private string GetLinkClass()
        {
            return UseLegacyStyles ? "" : "govuk-footer__link";
        }

        public void AddOrUpdateLink<T>(T link) where T : Link
        {
            _linkCollection.AddOrUpdateLink(link);
        }

        public void RemoveLink<T>() where T : Link
        {
            _linkCollection.RemoveLink<T>();
        }

        public string RenderListItemLink<T>(bool isSelected = false, string @class = "", string @role = "") where T : Link
        {
            return _linkHelper.RenderListItemLink<T>(isSelected, @class, @role);
        }

        public string RenderLink<T>(Func<string>? before = null, Func<string>? after = null, bool isSelected = false) where T : Link
        {
            return _linkHelper.RenderLink<T>(before, after, isSelected);
        }
    }
}

using Ofgem.GBI.ExternalPortal.Application.UserManagement.Constants;
using Ofgem.GBI.ExternalPortal.Web.Common.Models.Links;
using Ofgem.GBI.ExternalPortal.Web.Common.Configuration;
using Ofgem.GBI.ExternalPortal.Web.Common.Helpers;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models
{
    public class HeaderViewModel : IHeaderViewModel
    {
        const string OfgemHref = "https://www.ofgem.gov.uk/";
        public IUserContext UserContext { get; private set; }
        public bool MenuIsHidden { get; private set; }
        public string SelectedMenu { get; private set; }

        public IReadOnlyList<Link> Links => _linkCollection.Links;

        public bool UseLegacyStyles { get; private set; }

        private readonly ILinkCollection _linkCollection;
        private readonly ILinkHelper _linkHelper;
        private readonly string phaseBannerTag;

        public HeaderViewModel(
            IHeaderConfiguration configuration,
            IUserContext userContext,
            ILinkCollection? linkCollection = null,
            ILinkHelper? linkHelper = null,
            bool useLegacyStyles = false)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));
            UserContext = userContext ?? throw new ArgumentNullException(nameof(userContext));
            if (configuration.ApplicationBaseUrl == null) configuration.ApplicationBaseUrl = "#";

            _linkCollection = linkCollection ?? new LinkCollection();
            _linkHelper = linkHelper ?? new LinkHelper(_linkCollection);
            UseLegacyStyles = useLegacyStyles;

            MenuIsHidden = false;
            SelectedMenu = "home";

            // Header links
            AddOrUpdateLink(new OfgemLogo(OfgemHref, isLegacy: UseLegacyStyles));
            AddOrUpdateLink(new GbisHome("/", UseLegacyStyles ? "" : "govuk-header__link govuk-header__service-name"));
            AddOrUpdateLink(new SignIn("/home"));
            AddOrUpdateLink(new SignOut($"/{OneLoginConstants.SignOutUrl}"));

            AddOrUpdateLink(new Feedback("#", "govuk-link"));

            phaseBannerTag = configuration.PhaseBannerTag;
        }

        public void HideMenu()
        {
            MenuIsHidden = true;
        }

        public void SelectMenu(string menu)
        {
            SelectedMenu = menu;
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

        public string PhaseBannerTag { get { return phaseBannerTag; } }
    }
}

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class GbisHome : Link
    {
        public GbisHome(string href, string @class = "content, ") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"{Href}\" class=\"govuk-header__link govuk-header__link--service-name govuk-heading-m\" title=\"Go to the Great British Insulation Scheme homepage\">" +
                $"<h2 class=\"govuk-heading-m\">Great British Insulation Scheme</h2>" +
                $"</a>";
        }
    }
}

using System.Text;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class OfgemLogo : Link
    {
        public OfgemLogo(string href, string @class = "content", bool isLegacy = false) : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            var builder = new StringBuilder();
            builder.AppendLine($"<a href = \"{Href}\" class=\"govuk-header__logo\" id=\"logo\">");
            builder.AppendLine("<img src=\"/assets/images/ofgem-logo-with-strapline.png\" alt=\"Ofgem logo\" width=\"278\" height=\"59\"></img>");
            builder.AppendLine("</a>");

            return builder.ToString();
        }
    }
}

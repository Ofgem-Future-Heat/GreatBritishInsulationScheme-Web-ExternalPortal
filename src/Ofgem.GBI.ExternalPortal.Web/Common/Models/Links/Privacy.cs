using static System.Net.WebRequestMethods;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class Privacy : Link
    {
        public Privacy(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href=\"https://www.ofgem.gov.uk/publications/great-british-insulation-scheme-privacy-notice\" target=\"_blank\" class=\"{Class}\">Privacy</a>";
        }
    }
}

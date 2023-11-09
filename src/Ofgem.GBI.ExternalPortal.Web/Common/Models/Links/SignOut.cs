namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class SignOut : Link
    {
        public SignOut(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\" >Sign out</a>";
        }
    }
}

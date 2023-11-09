namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class SignIn : Link
    {
        public SignIn(string href, string @class = "") : base(href, @class: @class)
        {
        }

        public override string Render()
        {
            return $"<a href = \"{Href}\" class=\"{Class}\">Sign in</a>";
        }
    }
}

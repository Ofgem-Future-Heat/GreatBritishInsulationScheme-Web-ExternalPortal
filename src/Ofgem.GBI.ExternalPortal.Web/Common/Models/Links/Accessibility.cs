﻿namespace Ofgem.GBI.ExternalPortal.Web.Common.Models.Links
{
    public class Accessibility : Link
    {
        public Accessibility(string href, string @class = "") : base(href, @class: @class) { }

        public override string Render()
        {
            return $"<a href=\"{Href}\" class=\"{Class}\">Accessibility</a>";
        }

    }
}
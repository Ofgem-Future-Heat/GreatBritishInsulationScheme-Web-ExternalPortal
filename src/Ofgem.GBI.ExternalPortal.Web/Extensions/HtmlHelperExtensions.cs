using Microsoft.AspNetCore.Mvc.Rendering;
using Ofgem.GBI.ExternalPortal.Web.Common.Configuration;
using Ofgem.GBI.ExternalPortal.Web.Common.Models;

namespace Ofgem.GBI.ExternalPortal.Web.Extensions;

public static class HtmlHelperExtensions
    {
        public static IHeaderViewModel GetHeaderViewModel(this IHtmlHelper html, bool hideMenu = false)
        {
             
            var headerModel = new HeaderViewModel(new HeaderConfiguration
            {
                ApplicationBaseUrl = "#",
                AuthenticationAuthorityUrl = "#",
                ClientId = "#",
                SignOutUrl = null,
                ChangeEmailReturnUrl = null,
                ChangePasswordReturnUrl = null,
                PhaseBannerTag = "Beta",
                PhaseBannerFeedbackUrl = "#",
            },
            new UserContext
            {
                User = html.ViewContext.HttpContext.User,
                HashedAccountId = html.ViewContext.RouteData.Values["employerAccountId"]?.ToString() ?? string.Empty
            });

            headerModel.SelectMenu("recruitment");

            if (html.ViewBag.HideNav is bool && html.ViewBag.HideNav)
            {
                headerModel.HideMenu();
            }

            return headerModel;
        }

        public static IFooterViewModel GetFooterViewModel(this IHtmlHelper html)
        {
            return new FooterViewModel(new FooterConfiguration
                {
                    ApplicationBaseUrl = "#"
                },
                new UserContext
                {
                    User = html.ViewContext.HttpContext.User,
                    HashedAccountId = html.ViewContext.RouteData.Values["employerAccountId"]?.ToString() ?? string.Empty
                });
        }
    }
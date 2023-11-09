﻿using Ofgem.GBI.ExternalPortal.Web.Common.Configuration;
using Ofgem.GBI.ExternalPortal.Web.Common.Helpers;

namespace Ofgem.GBI.ExternalPortal.Web.Common.Models
{
    public interface IHeaderViewModel : ILinkCollection, ILinkHelper
    {
        bool MenuIsHidden { get; }
        string SelectedMenu { get; }
        IUserContext UserContext { get; }
        void HideMenu();
        void SelectMenu(string menu);
        bool UseLegacyStyles { get; }
        string PhaseBannerTag { get; }
    }
}

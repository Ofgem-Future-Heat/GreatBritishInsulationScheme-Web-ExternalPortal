namespace Ofgem.GBI.ExternalPortal.Web.Common.Configuration
{
    public class HeaderConfiguration : IHeaderConfiguration
    {
        public string? ApplicationBaseUrl { get; set; }
        public string? AuthenticationAuthorityUrl { get; set; }
        public string? ClientId { get; set; }
        public Uri? SignOutUrl { get; set; }
        public Uri? ChangeEmailReturnUrl { get; set; }
        public Uri? ChangePasswordReturnUrl { get; set; }
        public string PhaseBannerTag { get; set; } = string.Empty;
        public string? PhaseBannerFeedbackUrl { get; set; }
    }
}

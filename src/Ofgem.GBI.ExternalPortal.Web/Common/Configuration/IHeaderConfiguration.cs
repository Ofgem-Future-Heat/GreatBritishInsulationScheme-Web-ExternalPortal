namespace Ofgem.GBI.ExternalPortal.Web.Common.Configuration
{
    public interface IHeaderConfiguration
    {
        string? ApplicationBaseUrl { get; set; }
        string? AuthenticationAuthorityUrl { get; set; }
        Uri? ChangeEmailReturnUrl { get; set; }
        Uri? ChangePasswordReturnUrl { get; set; }
        Uri? SignOutUrl { get; set; }
        string? ClientId { get; set; }
        string PhaseBannerTag { get; set; }
        string? PhaseBannerFeedbackUrl { get; set; }
    }
}

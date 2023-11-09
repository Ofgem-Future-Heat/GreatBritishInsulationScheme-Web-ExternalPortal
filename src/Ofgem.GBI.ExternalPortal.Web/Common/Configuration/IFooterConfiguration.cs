namespace Ofgem.GBI.ExternalPortal.Web.Common.Configuration
{
    public interface IFooterConfiguration
    {
        string? ApplicationBaseUrl { get; set; }
        string? AuthenticationAuthorityUrl { get; set; }
    }
}

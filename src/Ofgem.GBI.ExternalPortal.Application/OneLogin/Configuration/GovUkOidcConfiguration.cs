namespace Ofgem.GBI.ExternalPortal.Application.OneLogin.Configuration
{
    public class GovUkOidcConfiguration
    {
        public required string BaseUrl { get; set; }
        public required string ClientId { get; set; }
        public required string KeyVaultIdentifier { get; set; }
        public required string EnableMfa { get; set; }
    }

    public static class GovUkConstants
    {
        public const string StubAuthCookieName = "GBIS.StubAuthCookie";
        public const string AuthCookieName = "GBIS.AuthCookie";
    }
}
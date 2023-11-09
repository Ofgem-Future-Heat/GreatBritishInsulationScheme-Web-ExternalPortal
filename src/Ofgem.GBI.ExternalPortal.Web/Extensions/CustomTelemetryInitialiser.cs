using Microsoft.ApplicationInsights.Channel;
using Microsoft.ApplicationInsights.Extensibility;

namespace Ofgem.GBI.ExternalPortal.Web.Extensions
{
    public class CustomTelemetryInitialiser : ITelemetryInitializer
    {
        public void Initialize(ITelemetry telemetry)
        {
            if (telemetry == null) return;
            telemetry.Context.Cloud.RoleName = "GBI-ExternalPortal-Web";
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using Serilog;
using Ofgem.GBI.ExternalPortal.Web;
using Ofgem.GBI.ExternalPortal.Infrastructure;
using Ofgem.GBI.ExternalPortal.Application;
using Microsoft.AspNetCore.HttpOverrides;
using Ofgem.GBI.ExternalPortal.Web.ApplicationBuilderExtensions;
using Ofgem.GBI.ExternalPortal.Application.OneLogin.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages()
    .AddRazorPagesOptions(options =>
    {
        options.Conventions.ConfigureFilter(new AutoValidateAntiforgeryTokenAttribute());
		options.Conventions.AuthorizeFolder("/Measures");
        options.Conventions.AuthorizeFolder("/ErrorReports");
        options.Conventions.AuthorizePage("/Home");
        options.Conventions.AddPageRoute("/Home", "");
    });

builder.Services.Configure<GovUkOidcConfiguration>(builder.Configuration.GetSection(nameof(GovUkOidcConfiguration)));
builder.Services.AddApplicationInsightsTelemetry(builder.Configuration.GetSection("APPINSIGHTS_CONNECTIONSTRING"));
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
});

builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddWebUIServices(builder.Configuration);
builder.Services.AddInfrastructureServices();

builder.Host.UseSerilog((context, services, configuration) => configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseStatusCodePagesWithRedirects("/Error?statusCode={0}");
app.UseAuthentication();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapHealthChecks("/health");

app.MapSignout();

app.Run();
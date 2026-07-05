using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using System.Globalization;
using VRT.Resume.Pwa;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Resources;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

builder.Services.AddMudServices();
builder.Services.AddPwaDependencies();
builder.Services.AddPwaDbContext(builder.HostEnvironment.BaseAddress);

var host = builder.Build();
var startup = host.Services.GetRequiredService<PwaStartupState>();

try
{
    await PwaSqliteStartup.InitializeAsync(host.Services);
    await host.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();
    var cultureService = host.Services.GetRequiredService<PwaCultureService>();
    await cultureService.InitializeAsync();
    ResourceHelper.ResolveCulture = () => CultureInfo.GetCultureInfo(cultureService.GetCurrentCulture());
    await host.Services.GetRequiredService<DummyCurrentUserService>().InitializeAsync();
    startup.MarkReady();
}
catch (Exception ex)
{
    startup.SetFailure(ex);

    var cultureService = host.Services.GetRequiredService<PwaCultureService>();
    await cultureService.InitializeAsync();
    ResourceHelper.ResolveCulture = () => CultureInfo.GetCultureInfo(cultureService.GetCurrentCulture());
}

await host.RunAsync();
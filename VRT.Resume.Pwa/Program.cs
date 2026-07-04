using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using SqliteWasmBlazor;
using VRT.Resume.Pwa;
using VRT.Resume.Pwa.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Logging.AddFilter("Microsoft.EntityFrameworkCore", LogLevel.Warning);

builder.Services.AddMudServices();
builder.Services.AddPwaDependencies();
builder.Services.AddPwaDbContext(builder.HostEnvironment.BaseAddress);

var host = builder.Build();

await host.Services.InitializeSqliteWasmAsync();
await host.Services.GetRequiredService<DatabaseInitializer>().InitializeAsync();
await host.Services.GetRequiredService<PwaCultureService>().InitializeAsync();
await host.Services.GetRequiredService<DummyCurrentUserService>().InitializeAsync();

await host.RunAsync();
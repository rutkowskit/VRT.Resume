using System.Globalization;
using Bunit;
using Bunit.TestDoubles;
using MediatR;
using Microsoft.AspNetCore.Components;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MudBlazor.Services;
using SqliteWasmBlazor;
using VRT.Resume.Application;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Application.Common.Services;
using VRT.Resume.Application.Persons.Commands.CreatePersonAccount;
using VRT.Resume.Persistence;
using VRT.Resume.Persistence.Data;
using VRT.Resume.Pwa.Features.Mediator;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Pwa.Tests.Components;
using VRT.Resume.Resources;

namespace VRT.Resume.Pwa.Tests.Fixtures;

public class PwaTestContext : TestContext
{
    private readonly SqliteConnection _connection;

    public PwaTestContext()
    {
        _connection = new SqliteConnection("Data Source=:memory:");
        _connection.Open();

        JSInterop.Mode = JSRuntimeMode.Loose;

        Services.AddLogging();
        Services.AddMudServices();
        Services.AddSingleton<NavigationManager, FakeNavigationManager>();

        Services.AddSingleton<IDateTimeService, DateTimeService>();
        Services.AddSingleton<IProfileImageService, ProfileImageService>();
        Services.AddSingleton<PwaCultureService>();
        Services.AddSingleton<ICultureService>(sp => sp.GetRequiredService<PwaCultureService>());
        Services.AddSingleton<DummyCurrentUserService>();
        Services.AddSingleton<ICurrentUserService>(sp => sp.GetRequiredService<DummyCurrentUserService>());
        Services.AddSingleton<IActiveProfileContext>(sp => sp.GetRequiredService<DummyCurrentUserService>());
        Services.AddScoped<ProfileContextStorage>();
        Services.AddScoped<LocalProfileService>();
        Services.AddSingleton<ISqliteWasmDatabaseService, StubSqliteWasmDatabaseService>();
        Services.AddScoped<PwaDatabaseBackupService>();
        Services.AddScoped<UserNotificationService>();
        Services.AddScoped<MediatorSender>();
        Services.AddSingleton<PwaStartupState>();

        Services.AddDbContextFactory<AppDbContext>(options => options.UseSqlite(_connection));
        Services.AddTransient<AppDbContext>(sp =>
            sp.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext());

        Services.AddApplication();

        ResourceHelper.ResolveCulture = () => CultureInfo.GetCultureInfo("pl");
    }

    public FakeNavigationManager Navigation =>
        Services.GetRequiredService<NavigationManager>() as FakeNavigationManager
        ?? throw new InvalidOperationException("FakeNavigationManager was not registered.");

    public DummyCurrentUserService ProfileContext => Services.GetRequiredService<DummyCurrentUserService>();

    public IRenderedComponent<T> RenderWithMudProviders<T>(Action<ComponentParameterCollectionBuilder<T>>? parameters = null)
        where T : IComponent
    {
        var host = RenderComponent<MudTestShell>(shell => shell.AddChildContent<T>(parameters));
        return host.FindComponent<T>();
    }

    public async Task InitializeDatabaseAsync()
    {
        await using var scope = Services.BuildServiceProvider().CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        await context.InitDatabaseAsync();
    }

    public async Task<string> SeedProfileAsync(
        string firstName,
        string lastName,
        string? userId = null,
        string? email = null)
    {
        userId ??= $"local:{Guid.NewGuid():N}";

        await using var scope = Services.BuildServiceProvider().CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var result = await mediator.Send(new CreatePersonAccountCommand
        {
            UserId = userId,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
        });

        if (result.IsFailure)
            throw new InvalidOperationException(result.Error);

        return email ?? userId;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _connection.Dispose();

        base.Dispose(disposing);
    }
}
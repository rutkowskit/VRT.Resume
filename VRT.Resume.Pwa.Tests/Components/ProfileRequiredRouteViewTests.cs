using FluentAssertions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using VRT.Resume.Pwa.Components;
using VRT.Resume.Pwa.Features.Profiles;
using VRT.Resume.Pwa.Layout;
using VRT.Resume.Pwa.Pages;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Components;

public sealed class ProfileRequiredRouteViewTests
{
    [Fact]
    public void WithoutActiveContext_RedirectsToProfiles()
    {
        using var ctx = new PwaTestContext();

        ctx.RenderWithMudProviders<ProfileRequiredRouteView>(parameters => parameters
            .Add(p => p.RouteData, CreateRouteData(typeof(ProtectedTestPage)))
            .Add(p => p.DefaultLayout, typeof(MainLayout)));

        ctx.Navigation.Uri.Should().EndWith("/profiles");
    }

    [Fact]
    public async Task WithActiveContext_DoesNotRedirectProtectedPage()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();
        var userId = await ctx.SeedProfileAsync("Anna", "Alpha");
        await ctx.ProfileContext.SetContextAsync(userId);

        ctx.RenderWithMudProviders<ProfileRequiredRouteView>(parameters => parameters
            .Add(p => p.RouteData, CreateRouteData(typeof(ProtectedTestPage)))
            .Add(p => p.DefaultLayout, typeof(MainLayout)));

        ctx.Navigation.Uri.Should().Be("http://localhost/");
    }

    [Fact]
    public void ExemptPage_DoesNotRedirectWithoutContext()
    {
        using var ctx = new PwaTestContext();

        ctx.RenderWithMudProviders<ProfileRequiredRouteView>(parameters => parameters
            .Add(p => p.RouteData, CreateRouteData(typeof(ExemptTestPage)))
            .Add(p => p.DefaultLayout, typeof(MainLayout)));

        ctx.Navigation.Uri.Should().Be("http://localhost/");
    }

    private static RouteData CreateRouteData(Type pageType) =>
        new(pageType, new Dictionary<string, object?>());
}
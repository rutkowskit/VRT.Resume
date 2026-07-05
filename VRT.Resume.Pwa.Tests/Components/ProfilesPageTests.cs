using Bunit;
using FluentAssertions;
using VRT.Resume.Pwa.Features.Profiles;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Components;

public sealed class ProfilesPageTests
{
    [Fact]
    public async Task RendersSeededProfiles()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();
        await ctx.SeedProfileAsync("Anna", "Alpha");
        await ctx.SeedProfileAsync("Bart", "Beta");

        var cut = ctx.RenderWithMudProviders<ProfilesPage>();

        cut.WaitForAssertion(() =>
        {
            cut.Markup.Should().Contain("Anna Alpha");
            cut.Markup.Should().Contain("Bart Beta");
        });
    }

    [Fact]
    public async Task SelectProfile_NavigatesToHome()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();
        var userId = await ctx.SeedProfileAsync("Anna", "Alpha");

        var cut = ctx.RenderWithMudProviders<ProfilesPage>();

        cut.WaitForAssertion(() => cut.FindAll("button").Count.Should().BeGreaterThan(0));

        var selectButton = cut.FindAll("button")
            .First(button => button.TextContent.Contains("Wybierz", StringComparison.OrdinalIgnoreCase)
                || button.TextContent.Contains("Select", StringComparison.OrdinalIgnoreCase));

        selectButton.Click();

        cut.WaitForAssertion(() =>
        {
            ctx.Navigation.Uri.Should().EndWith("/");
            ctx.ProfileContext.UserId.Should().Be(userId);
        });
    }
}
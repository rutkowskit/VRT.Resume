using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Services;

public sealed class LocalProfileServiceTests
{
    [Fact]
    public async Task DeleteAsync_RemovesProfileFromList()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();

        var userA = await ctx.SeedProfileAsync("Anna", "Alpha");
        var userB = await ctx.SeedProfileAsync("Bart", "Beta");

        await using var scope = ctx.Services.BuildServiceProvider().CreateAsyncScope();
        var profileService = scope.ServiceProvider.GetRequiredService<LocalProfileService>();

        (await profileService.DeleteAsync(userA)).Should().BeTrue();

        var remaining = await profileService.GetAllAsync();
        remaining.Should().ContainSingle().Which.UserId.Should().Be(userB);
    }

    [Fact]
    public async Task DeleteAsync_ReturnsFalse_WhenProfileMissing()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();

        await using var scope = ctx.Services.BuildServiceProvider().CreateAsyncScope();
        var profileService = scope.ServiceProvider.GetRequiredService<LocalProfileService>();

        (await profileService.DeleteAsync("local:missing")).Should().BeFalse();
    }
}
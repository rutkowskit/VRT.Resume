using FluentAssertions;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using VRT.Resume.Application.Persons.Commands.UpdatePersonData;
using VRT.Resume.Application.Persons.Queries.GetPersonData;
using VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResumeList;
using VRT.Resume.Pwa.Services;
using VRT.Resume.Pwa.Tests.Fixtures;

namespace VRT.Resume.Pwa.Tests.Services;

public sealed class ProfileContextTests
{
    [Fact]
    public async Task SetContextAsync_SwitchesActivePerson()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();

        var userA = await ctx.SeedProfileAsync("Anna", "Alpha");
        var userB = await ctx.SeedProfileAsync("Bart", "Beta");

        await ctx.ProfileContext.SetContextAsync(userA);
        var personIdA = ctx.ProfileContext.PersonId;
        personIdA.Should().NotBeNull();
        ctx.ProfileContext.UserId.Should().Be(userA);

        await ctx.ProfileContext.SetContextAsync(userB);
        ctx.ProfileContext.UserId.Should().Be(userB);
        ctx.ProfileContext.PersonId.Should().NotBe(personIdA);
    }

    [Fact]
    public async Task ActiveContext_IsolatesPersonDataBetweenProfiles()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();

        var userA = await ctx.SeedProfileAsync("Anna", "Alpha");
        var userB = await ctx.SeedProfileAsync("Bart", "Beta");

        await using var scope = ctx.Services.BuildServiceProvider().CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var profileContext = scope.ServiceProvider.GetRequiredService<DummyCurrentUserService>();

        await profileContext.SetContextAsync(userA);
        (await mediator.Send(new UpdatePersonDataCommand
        {
            FirstName = "Alicja",
            LastName = "Alpha",
        })).IsSuccess.Should().BeTrue();

        await profileContext.SetContextAsync(userB);
        var personB = await mediator.Send(new GetPersonDataQuery());
        personB.Value.FirstName.Should().Be("Bart");

        await profileContext.SetContextAsync(userA);
        var personA = await mediator.Send(new GetPersonDataQuery());
        personA.Value.FirstName.Should().Be("Alicja");
    }

    [Fact]
    public async Task ActiveContext_IsolatesResumeLists()
    {
        using var ctx = new PwaTestContext();
        await ctx.InitializeDatabaseAsync();

        var userA = await ctx.SeedProfileAsync("Anna", "Alpha");
        var userB = await ctx.SeedProfileAsync("Bart", "Beta");

        await using var scope = ctx.Services.BuildServiceProvider().CreateAsyncScope();
        var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
        var profileContext = scope.ServiceProvider.GetRequiredService<DummyCurrentUserService>();

        await profileContext.SetContextAsync(userA);
        (await mediator.Send(new UpsertPersonResumeCommand
        {
            ResumeId = 0,
            Description = "CV A",
            Position = "Dev A",
            Summary = "Summary A",
            ShowProfilePhoto = true,
            DataProcessingPermission = string.Empty,
        })).IsSuccess.Should().BeTrue();

        await profileContext.SetContextAsync(userB);
        (await mediator.Send(new UpsertPersonResumeCommand
        {
            ResumeId = 0,
            Description = "CV B",
            Position = "Dev B",
            Summary = "Summary B",
            ShowProfilePhoto = true,
            DataProcessingPermission = string.Empty,
        })).IsSuccess.Should().BeTrue();

        var resumesB = await mediator.Send(new GetResumeListQuery());
        resumesB.Should().ContainSingle().Which.Description.Should().Be("CV B");

        await profileContext.SetContextAsync(userA);
        var resumesA = await mediator.Send(new GetResumeListQuery());
        resumesA.Should().ContainSingle().Which.Description.Should().Be("CV A");
    }
}
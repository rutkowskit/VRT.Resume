using FluentValidation;
using Microsoft.EntityFrameworkCore;
using VRT.Resume.Application.Fixtures;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount;

public class CreatePersonAccountCommandTests : AnonymousQueryTestBase<CreatePersonAccountCommand, int>
{
    public CreatePersonAccountCommandTests(ApplicationFixture fixture): base(fixture)
    {
    }

    [Fact()]
    public async Task Send_CommandWithoutUserId_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.UserId = null!;

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_CommandWithInvalidEmail_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.Email = "invalid_email.com";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_CommandWithoutFirstName_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.FirstName = "";

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }
    [Fact()]
    public async Task Send_FirstNameExceedMaximumLength_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.FirstName = new string('A', 51);

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }

    [Fact()]
    public async Task Send_LastNameExceedMaximumLength_ShouldThrowValidationError()
    {
        var sut = CreateSut();
        sut.FirstName = new string('A', 101);

        await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
    }


    [Fact()]
    public async Task Send_WhenUserEntryNotExists_ShouldAddNewPersonAndAssociatedData()
    {
        var sut = CreateSut();

        var result = await Send(sut);

        var person = Assert.Single(GetDbContext().UserPerson);
        Assert.Equal(sut.Email, person.UserId);


        Assert.True(result.IsSuccess, result.GetErrorSafe());
    }
    [Fact()]
    public async Task Send_WhenOtherUserExists_ShouldAddNewUser()
    {
        var sut = CreateSut();
        sut.UserId = "some.different.user@test.me";
        sut.Email = "some.different.user@test.me";

        var result = await Send(sut);

        result.AssertSuccess();

        var persons = GetDbContext()
            .UserPerson
            .Include(p => p.Person)
            .ToArray();

        Assert.Equal(2, persons.Length);

        var person = persons.FirstOrDefault(p => p.UserId == sut.Email);
        Assert.NotNull(person);
        Assert.Equal(sut.Email, person.UserId);
        Assert.Equal(sut.FirstName, person.Person.FirstName);
        Assert.Equal(sut.LastName, person.Person.LastName);
    }

    protected override CreatePersonAccountCommand CreateSut()
    {
        return new CreatePersonAccountCommand()
        {
            UserId = Defaults.UserId,
            Email = Defaults.UserId,
            FirstName = "Tester",
            LastName = "Testowski"
        };
    }
}
using Xunit;
using System.Threading.Tasks;
using Autofac;
using VRT.Resume.Persistence.Data;
using FluentValidation;
using System.Linq;

namespace VRT.Resume.Application.Persons.Commands.CreatePersonAccount
{
    public class CreatePersonAccountCommandTests
    {
        [Fact()]
        public async Task Send_CommandWithoutEmail_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Email = null;

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }

        [Fact()]
        public async Task Send_CommandWithInvalidEmail_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Email = "invalid_email.com";

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }
        [Fact()]
        public async Task Send_CommandWithoutFirstName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FirstName = "";

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }
        [Fact()]
        public async Task Send_FirstNameExceedMaximumLenght_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FirstName = new string('A',51);

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }

        [Fact()]
        public async Task Send_LastNameExceedMaximumLenght_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FirstName = new string('A', 101);

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }
      

        [Fact()]
        public async Task Send_WhenUserEntryNotExists_ShouldAddNewPersonAndAsociatedData()
        {
            var sut = CreateSut();

            var result = await sut.Send(
                seedDbWithDefaults: false,
                onAfterSend: scope =>
                {
                    var person = Assert.Single(scope.Resolve<AppDbContext>().UserPerson);
                    Assert.Equal(sut.Email, person.UserId);                    
                });
           
            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }
        [Fact()]
        public async Task Send_WhenOtherUserExists_ShouldAddNewUser()
        {
            var sut = CreateSut();
            sut.Email = "some.different.user@test.me";
            var result = await sut.Send(                
                onAfterSend: scope =>
                {
                    var persons = scope.Resolve<AppDbContext>()
                        .UserPerson
                        .ToArray();
                    Assert.Equal(2, persons.Length);

                    var person = persons.FirstOrDefault(p => p.UserId == sut.Email);
                    Assert.NotNull(person);                    
                    Assert.Equal(sut.Email, person.UserId);
                    Assert.Equal(sut.FirstName, person.Person.FirstName);
                    Assert.Equal(sut.LastName, person.Person.LastName);
                });

            Assert.True(result.IsSuccess, result.GetErrorSafe());
        }        

        private CreatePersonAccountCommand CreateSut()
        {
            return new CreatePersonAccountCommand()
            {
                Email = Defaults.UserId,                
                FirstName = "Tester",
                LastName = "Testowski"
            };
        }
    }
}
using Autofac;
using FluentValidation;
using System.Threading.Tasks;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpdatePersonData
{
    public class UpdatePersonDataCommandTests : CommandTestBase<UpdatePersonDataCommand>
    {
        [Fact()]
        public async Task Send_CommandWithoutFirstName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FirstName = null;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooLongFirstName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.FirstName = new string('a',51);

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithoutLastName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.LastName = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithTooLongLastName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.LastName = new string('a', 101);

            await Assert.ThrowsAsync<ValidationException>(() => sut.Send());
        }

        [Fact()]
        public async Task Send_CommandWithWhenPersonExistsIdDb_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();

            var result = await Send(sut, onAfterSend: scope => AssertPersonDataInDb(scope,sut));

            result.AssertSuccess();
        }

        private void AssertPersonDataInDb(ILifetimeScope scope, 
            UpdatePersonDataCommand expectedData)
        {
            var sut = expectedData;
            var person = Assert.Single(scope.Resolve<AppDbContext>().Person);
            Assert.Equal(sut.FirstName, person.FirstName);
            Assert.Equal(sut.LastName, person.LastName);
            Assert.Equal(sut.DateOfBirth, person.DateOfBirth);
            Assert.Equal(Defaults.PersonId, person.PersonId);
            Assert.Equal(Defaults.Today, person.ModifiedDate);
        }

        protected override UpdatePersonDataCommand CreateSut()
        {
            return new UpdatePersonDataCommand()
            {
                FirstName = "Tom",
                LastName = "Testovski",
                DateOfBirth = Defaults.Today.AddYears(-30)
            };
        }
    }
}
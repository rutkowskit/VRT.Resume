using Autofac;
using FluentValidation;
using System.Linq;
using System.Threading.Tasks;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;
using Xunit;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonEducation
{
    public class UpsertPersonEducationCommandTests : CommandTestBase<UpsertPersonEducationCommand>
    {
        [Fact()]
        public async Task Send_CommandWithTooShortDegree_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.Degree = "";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }
        [Fact()]
        public async Task Send_CommandWithTooShortSchoolName_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.SchoolName = "x";

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithNotSetFromDate_ShouldThrowValidationError()
        {
            var sut = CreateSut();            
            sut.FromDate = default;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithNotSetToDate_ShouldThrowValidationError()
        {
            var sut = CreateSut();
            sut.ToDate = default;

            await Assert.ThrowsAsync<ValidationException>(() => Send(sut));
        }

        [Fact()]
        public async Task Send_CommandWithRequiredParametersAndWrongId_ShouldAddNewRecordToDbContext()
        {
            var sut = CreateSut();            
            var result = await Send(sut,
                onBeforeSend: scope =>
                {
                    var edu = scope.Resolve<AppDbContext>().PersonEducation.FirstOrDefault();
                    Assert.Null(edu);
                },
                onAfterSend: scope =>
            {
                var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonEducation);
                AssertPersonEducation(edu, sut);                
            });
            
            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithIdOfExistingEducation_ShouldUpdateExistingRecord()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope => await scope.SeedEducation(),
                onAfterSend: scope =>
                {
                    var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonEducation);
                    AssertPersonEducation(edu, sut);
                });

            result.AssertSuccess();            
        }

        [Fact()]
        public async Task Send_CommandWithSchoolNameThatExistsInDb_ShouldSetExistingSchoolId()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope =>
                {
                    await scope.SeedEducation();
                    await scope.SeedSchool(66,sut.SchoolName);
                },
                onAfterSend: scope =>
                {
                    var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonEducation);
                    AssertPersonEducation(edu, sut);
                    Assert.Equal(66, edu.SchoolId);
                });
            result.AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithDegreeNameThatExistsInDb_ShouldSetExistingDegreeId()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope =>
                {
                    await scope.SeedEducation();
                    await scope.SeedDegree(44, sut.Degree);
                },
                onAfterSend: scope =>
                {
                    var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonEducation);
                    AssertPersonEducation(edu, sut);
                    Assert.Equal(44, edu.DegreeId);
                });
            result.AssertSuccess();
        }

        [Fact()]
        public async Task Send_CommandWithEducationFieldThatExistsInDb_ShouldSetExistingEducationFieldId()
        {
            var sut = CreateSut();

            var result = await Send(sut,
                async scope =>
                {
                    await scope.SeedEducation();
                    await scope.SeedEducationField(88, sut.Field);
                },
                onAfterSend: scope =>
                {
                    var edu = Assert.Single(scope.Resolve<AppDbContext>().PersonEducation);
                    AssertPersonEducation(edu, sut);
                    Assert.Equal(88, edu.EducationFieldId);
                });
            result.AssertSuccess();
        }

        private void AssertPersonEducation(PersonEducation edu, 
               UpsertPersonEducationCommand sut)
        {
            Assert.NotNull(edu);
            Assert.Equal(1, edu.EducationId);
            Assert.Equal(sut.Degree, edu.Degree.Name);
            Assert.Equal(sut.Field, edu.EducationField.Name);
            Assert.Equal(sut.FromDate, edu.FromDate);
            Assert.Equal(sut.ToDate, edu.ToDate);
            Assert.Equal(Defaults.PersonId, edu.PersonId);
        }

        protected override UpsertPersonEducationCommand CreateSut()
        {
            return new UpsertPersonEducationCommand()
            {
                EducationId = 1,
                Degree = "Master of Science",
                Field = "Computer Science",
                SchoolName = "Bajtowards",
                Grade = "Good enough",
                ThesisTitle = "Some thesis title",                
                Specialization = "Assembler",
                FromDate = Defaults.Today.AddYears(-20),
                ToDate = Defaults.Today.AddYears(-16),
            };
        }
    }
}
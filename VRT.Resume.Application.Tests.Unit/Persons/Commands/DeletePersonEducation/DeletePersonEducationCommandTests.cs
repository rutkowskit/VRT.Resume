using VRT.Resume.Application.Fixtures;
using VRT.Resume.Domain.Entities;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonEducation
{
    public class DeletePersonEducationCommandTests
        : DeleteCommandTestBase<DeletePersonEducationCommand, PersonEducation>
    {
        public DeletePersonEducationCommandTests(ApplicationFixture fixture) : base(fixture)
        {

        }

        protected override DeletePersonEducationCommand CreateSut(int educationId)
        {
            return new DeletePersonEducationCommand(educationId);
        }

        protected override DeletePersonEducationCommand CreateSut(PersonEducation entity)
        {
            return CreateSut(entity.EducationId);
        }

        protected override Task<PersonEducation> SeedEntity()
        {
            return GetDbContext().SeedEducation();
        }
    }
}
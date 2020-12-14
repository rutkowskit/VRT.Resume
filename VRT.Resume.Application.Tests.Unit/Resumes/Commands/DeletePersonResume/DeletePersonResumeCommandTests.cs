using Xunit;
using VRT.Resume.Application.Resumes.Commands.DeletePersonResume;
using System;
using System.Collections.Generic;
using System.Text;
using VRT.Resume.Domain.Entities;
using Autofac;
using System.Threading.Tasks;

namespace VRT.Resume.Application.Resumes.Commands.DeletePersonResume.Tests
{
    public class DeletePersonResumeCommandTests
    : DeleteCommandTestBase<DeletePersonResumeCommand, PersonResume>
    {
        protected override DeletePersonResumeCommand CreateSut(int entityId)
        {
            return new DeletePersonResumeCommand(entityId);
        }

        protected override Task SeedEntity(ILifetimeScope scope)
        {
            return scope.SeedPersonResume();
        }
    }
}
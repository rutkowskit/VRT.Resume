using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.DeletePersonsSkill
{
    public sealed class DeletePersonSkillCommand : IRequest<Result>
    {
        public DeletePersonSkillCommand(int skillId)
        {
            SkillId = skillId;
        }
        public int SkillId { get;}
        internal sealed class DeletePersonSkillCommandHandler : DeleteHandlerBase<DeletePersonSkillCommand, PersonSkill>
            
        {
            public DeletePersonSkillCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonSkill> GetExistingData(DeletePersonSkillCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonSkill
                                    where p.PersonId == m
                                    where p.SkillId == request.SkillId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonSkill>(Errors.RecordNotFound);
                    });
            }
        }
    }
}

using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonSkill
{
    public sealed class UpsertPersonSkillCommand : IRequest<Result>
    {
        public int SkillId { get; set; }
        public SkillTypes SkillType { get; set; }    
        public string SkillName { get; set; }
        public string SkillLevel { get; set; }

        internal  sealed class UpsertPersonDataCommandHandler : UpsertHandlerBase<UpsertPersonSkillCommand, PersonSkill>            
        {
            public UpsertPersonDataCommandHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }

            protected override Result<PersonSkill> UpdateData(PersonSkill current, UpsertPersonSkillCommand request)
            {
                current.Level = request.SkillLevel;
                current.Name = request.SkillName;
                current.SkillTypeId = (byte) request.SkillType;
                return current;
            }
                        
            protected override Result<PersonSkill> GetExistingData(UpsertPersonSkillCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonSkill
                                    where p.PersonId == m
                                    where p.SkillId == request.SkillId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonSkill>("Current data not exists");                        
                    });                
            }
        }
    }
}

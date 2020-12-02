using CSharpFunctionalExtensions;
using MediatR;
using System;
using System.Linq;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Commands.UpsertPersonExperience
{
    public sealed class UpsertPersonExperienceCommand : IRequest<Result>
    {
        #region command fields
        public int ExperienceId { get; set; }
        public string Position { get; set; }
        public string CompanyName { get; set; }
        public string Location { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        #endregion

        internal sealed class UpsertPersonExperienceCommandHandler : UpsertHandlerBase<UpsertPersonExperienceCommand, PersonExperience>

        {
            public UpsertPersonExperienceCommandHandler(AppDbContext context,
                ICurrentUserService userService, IDateTimeService dateTiemService)
                : base(context, userService, dateTiemService)
            {
            }

            protected override Result<PersonExperience> UpdateData(PersonExperience current, UpsertPersonExperienceCommand request)
            {
                current.FromDate = request.FromDate;
                current.ToDate = request.ToDate;
                current.Location = request.Location;
                current.Position = request.Position;
                current.CompanyName = request.CompanyName;
                current.ModifyDate = GetCurrentDate();
                return current;
            }
            protected override Result<PersonExperience> GetExistingData(UpsertPersonExperienceCommand request)
            {
                return GetCurrentUserPersonId()
                    .Bind(m =>
                    {
                        var query = from p in Context.PersonExperience
                                    where p.PersonId == m
                                    where p.ExperienceId == request.ExperienceId
                                    select p;
                        var result = query.FirstOrDefault();
                        return result ?? Result.Failure<PersonExperience>(Errors.RecordNotFound);
                    });
            }
        }
    }
}

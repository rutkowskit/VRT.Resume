using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonEducation
{
    public sealed class GetPersonEducationQuery : IRequest<Result<PersonEducationInListVM>>
    {
        public int EducationId { get; }

        public GetPersonEducationQuery(int educationId)
        {
            EducationId = educationId;
        }        

        internal sealed class GetPersonEducationQueryHandler : HandlerBase, 
            IRequestHandler<GetPersonEducationQuery, Result<PersonEducationInListVM>>
        { 
            public GetPersonEducationQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonEducationInListVM>> Handle(GetPersonEducationQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonEducation
                                    where per.PersonId == p
                                    where per.EducationId==request.EducationId
                                    select new PersonEducationInListVM()
                                    {                                        
                                        EducationId = per.EducationId,
                                        Degree = per.Degree.Name,
                                        Field = per.EducationField.Name,                                        
                                        FromDate = per.FromDate,
                                        ToDate = per.ToDate,
                                        Grade = per.Grade,
                                        SchoolName = per.School.Name,
                                        Specialization = per.Specialization,
                                        ThesisTitle = per.ThesisTitle,
                                        ModifiedDate = per.ModifiedDate
                                    };
                        return query.FirstOrDefault();
                    });                
            }
        }
    }
}

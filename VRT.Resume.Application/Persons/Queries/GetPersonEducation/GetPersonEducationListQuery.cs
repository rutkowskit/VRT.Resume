using CSharpFunctionalExtensions;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VRT.Resume.Application.Common.Abstractions;
using VRT.Resume.Persistence.Data;

namespace VRT.Resume.Application.Persons.Queries.GetPersonEducation
{

    public sealed class GetPersonEducationListQuery : IRequest<Result<PersonEducationInListVM[]>>
    {
        public sealed class GetPersonEducationListQueryHandler : HandlerBase, IRequestHandler<GetPersonEducationListQuery, Result<PersonEducationInListVM[]>>
        { 
            public GetPersonEducationListQueryHandler(AppDbContext context, ICurrentUserService userService)
                : base(context, userService)
            {                
            }
            public async Task<Result<PersonEducationInListVM[]>> Handle(GetPersonEducationListQuery request, CancellationToken cancellationToken)
            {
                await Task.Yield();
                return GetCurrentUserPersonId()
                    .Map(p =>
                    {
                        var query = from per in Context.PersonEducation
                                    where per.PersonId == p
                                    select new PersonEducationInListVM()
                                    {
                                        EducationId = per.EducationId,
                                        SchoolName = per.School.Name,
                                        FromDate = per.FromDate,
                                        ToDate = per.ToDate,
                                        Degree = per.Degree.Name,
                                        Field = per.EducationField.Name,
                                        Specialization = per.Specialization,
                                        ThesisTitle = per.ThesisTitle,
                                        Grade = per.Grade,
                                        ModifiedDate = per.ModifiedDate
                                    };
                        return query.ToArray();
                    });                
            }
        }
    }
}

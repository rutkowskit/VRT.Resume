namespace VRT.Resume.Application.Persons.Commands.UpsertPersonEducation;

public sealed class UpsertPersonEducationCommand : IRequest<Result>
{
    #region command fields
    public int EducationId { get; set; }
    public string? SchoolName { get; set; }
    public string? Degree { get; set; }
    public string? Field { get; set; }

    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public string? Grade { get; set; }
    public string? ThesisTitle { get; set; }
    public string? Specialization { get; set; }
    #endregion

    internal sealed class UpsertPersonEducationCommandHandler : UpsertHandlerBase<UpsertPersonEducationCommand, PersonEducation>

    {
        public UpsertPersonEducationCommandHandler(AppDbContext context,
            ICurrentUserService userService, IDateTimeService dateTimeService)
            : base(context, userService, dateTimeService)
        {
        }

        protected override async Task<Result<PersonEducation>> UpdateData(PersonEducation current, UpsertPersonEducationCommand request)
        {
            current.FromDate = request.FromDate;
            current.ToDate = request.ToDate;
            current.Grade = request.Grade;
            current.ThesisTitle = request.ThesisTitle;
            current.Specialization = request.Specialization;

            return await UpdateSchool(current, request)
                .Bind(c => UpdateDegree(c, request))
                .Bind(c => UpdateEducationField(c, request))
                .Bind(UpdateModificationDate);
        }
        protected override async Task<Result<PersonEducation>> GetExistingData(UpsertPersonEducationCommand request)
        {
            return await GetCurrentUserPersonId()
                .Bind(async m =>
                {
                    var query = from p in Context.PersonEducation
                                where p.PersonId == m
                                where p.EducationId == request.EducationId
                                select p;
                    var result = await query.FirstOrDefaultAsync();
                    return result ?? Result.Failure<PersonEducation>(Errors.RecordNotFound);
                });
        }


        private Result<PersonEducation> UpdateSchool(PersonEducation person,
            UpsertPersonEducationCommand request)
        {
            var school = Context.School.FirstOrDefault(s =>
                        (s.SchoolId == person.SchoolId && s.Name == request.SchoolName)
                        || s.Name == request.SchoolName);

            if (school != null)
            {
                person.SchoolId = school.SchoolId;
                return person;
            }

            person.SchoolId = 0;
            person.School = new School()
            {
                Name = request.SchoolName,
                ModifiedDate = GetCurrentDate()
            };
            return person;
        }

        private Result<PersonEducation> UpdateDegree(PersonEducation person,
            UpsertPersonEducationCommand request)
        {
            var degree = Context.Degree.FirstOrDefault(s =>
                        (s.DegreeId == person.DegreeId && s.Name == request.Degree)
                        || s.Name == request.Degree);

            if (degree != null)
            {
                person.DegreeId = degree.DegreeId;
                return person;
            }
            person.DegreeId = 0;
            person.Degree = new Degree()
            {
                Name = request.Degree
            };
            return person;
        }

        private async Task<Result<PersonEducation>> UpdateEducationField(PersonEducation person,
            UpsertPersonEducationCommand request)
        {
            var eduField = await Context.EducationField
                    .FirstOrDefaultAsync(s =>
                        (s.EducationFieldId == person.EducationFieldId && s.Name == request.Field)
                        || s.Name == request.Field);

            if (eduField != null)
            {
                person.EducationFieldId = eduField.EducationFieldId;
                return person;
            }
            person.EducationFieldId = 0;
            person.EducationField = new EducationField()
            {
                Name = request.Field
            };
            return person;
        }

        private Result<PersonEducation> UpdateModificationDate(PersonEducation current)
        {
            if (current.HasChanges(Context))
            {
                current.ModifiedDate = GetCurrentDate();
            }
            return current;
        }
    }
}

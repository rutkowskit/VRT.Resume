using AutoMapper;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.MappingProfiles
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PersonEducationInListVM, PersonEducationViewModel>();
            CreateMap<ResumeVM, PersonResumeViewModel>();
        }
    }
}
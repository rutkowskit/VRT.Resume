using AutoMapper;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.MappingProfiles
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PersonEducationInListVM, PersonEducationViewModel>();
            CreateMap<ResumeVM, PersonResumeViewModel>();

            CreateMap<PersonExperienceVM, PersonExperienceViewModel>();
            CreateMap<PersonExperienceDutyVM, PersonExperienceDutyViewModel>();
                        
        }
    }
}
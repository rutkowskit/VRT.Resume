using AutoMapper;
using System.Web.Mvc;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Domain.Entities;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.MappingProfiles
{
    public sealed class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<PersonEducationInListVM, PersonEducationViewModel>();            
        }
    }
}
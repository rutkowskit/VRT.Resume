using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.DeletePersonEducation;
using VRT.Resume.Application.Persons.Commands.UpsertPersonEducation;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonEduController : PersonEditControllerBase
    {
        private readonly IMapper _mapper;

        public PersonEduController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }
        
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonEducationQuery(id);
            var result = await Mediator.Send(query)
                .Map(r => _mapper.Map<PersonEducationInListVM, PersonEducationViewModel>(r));
            
            return ToActionResult(result);            
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonEducationCommand data)
        {
            var result = await Send(data);
            if (result.IsFailure)
            {
                return View("Edit");
            }            
            return ToProfile(TabNames.Education);
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {
            var result = await Send(new DeletePersonEducationCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfile(TabNames.Education);
        }

        public override ActionResult Cancel() 
            => ToProfile(TabNames.Education);
    }
}
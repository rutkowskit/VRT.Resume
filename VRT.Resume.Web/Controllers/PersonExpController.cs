using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperience;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperience;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonExpController : PersonEditControllerBase
    {
        private readonly IMapper _mapper;

        public PersonExpController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonExperienceQuery()
            {
                ExperienceId = id
            };            
            var result = await Mediator.Send(query)
                .Map(m => _mapper.Map<PersonExperienceViewModel>(m));                
            return ToActionResult(result);            
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {
            var result = await Send(new DeletePersonExperienceCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfile(TabNames.WorkExp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonExperienceCommand data)
        {
            var result = await Send(data);

            if (result.IsFailure)
                return View(data.ExperienceId>0 ? nameof(Edit) : nameof(Add));
            return ToProfile(TabNames.WorkExp);
        }
        public override ActionResult Cancel() 
            => ToProfile(TabNames.WorkExp);
    }
}
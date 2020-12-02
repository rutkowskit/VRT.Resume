using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.DeletePersonExperienceDuty;
using VRT.Resume.Application.Persons.Commands.UpsertPersonExperienceDuty;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDuty;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonExpDutyController : PersonEditControllerBase
    {
        private readonly IMapper _mapper;

        public PersonExpDutyController(IMediator mediator, IMapper mapper) : base(mediator)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Renders view for adding duty to experience
        /// </summary>
        /// <param name="id">experienceId</param>
        /// <returns>Action result</returns>
        [HttpGet]
        public ActionResult Add(int id)
        {
            var model = new PersonExperienceDutyViewModel()
            {
                ExperienceId = id
            };            
            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonExperienceDutyQuery()
            {
                DutyId = id
            };            
            var result = await Mediator.Send(query)
                .Map(m => _mapper.Map<PersonExperienceDutyViewModel>(m));                
            return ToActionResult(result);            
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {
            var result = await Send(new DeletePersonExperienceDutyCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfileAfterSave(TabNames.WorkExp);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonExperienceDutyCommand data)
        {
            var result = await Send(data);

            if (result.IsFailure)
                return View(data.ExperienceId>0 ? nameof(Edit) : nameof(Add));
            return ToProfileAfterSave(TabNames.WorkExp);
        }
        public override ActionResult Cancel() => ToProfile(TabNames.WorkExp);
    }
}
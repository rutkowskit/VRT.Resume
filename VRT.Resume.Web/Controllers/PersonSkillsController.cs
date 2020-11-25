using System.Threading.Tasks;
using System.Web.Mvc;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.DeletePersonsEntity;
using VRT.Resume.Application.Persons.Commands.UpsertPersonSkill;
using VRT.Resume.Application.Persons.Queries.GetPersonSkills;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonSkillsController : PersonEditControllerBase
    {
        public PersonSkillsController(IMediator mediator) : base(mediator)
        {
        }

        public ActionResult Add() => View();
        
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonSkillQuery()
            {
                SkillId = id
            };

            var result = await Mediator.Send(query)
                .Map(m => new PersonSkillEditViewModel()
                {
                    SkillId = m.SkillId,
                    SkillLevel = m.Level,
                    SkillName = m.Name,
                    SkillType = m.Type
                });
            return ToActionResult(result);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {            
            var result = await Send(new DeletePersonSkillCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfileAfterSave(TabNames.Skills);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonSkillCommand data)
        {            
            var result = await Send(data);
            
            if(result.IsFailure)
                return View("Edit");            
            return ToProfileAfterSave(TabNames.Skills);
        }
        public override ActionResult Cancel() 
            => ToProfile(TabNames.Skills);
    }
}
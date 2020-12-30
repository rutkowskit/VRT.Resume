using System.Threading.Tasks;
using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Persons.Commands.DeletePersonsSkill;
using VRT.Resume.Application.Persons.Commands.UpsertPersonSkill;
using VRT.Resume.Application.Persons.Queries.GetPersonSkills;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers
{
    public sealed class PersonSkillsController : PersonEditControllerBase
    {
        public PersonSkillsController(IMediator mediator) : base(mediator)
        {
        }
                
        [HttpGet]
        public async Task<ActionResult> Edit(int id, string returnUrl = "")
        {
            TempData[TempDataKeys.ReturnUrl] = returnUrl;

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

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {            
            var result = await Send(new DeletePersonSkillCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfile(TabNames.Skills);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonSkillCommand data)
        {            
            var result = await Send(data);
            
            if(result.IsFailure)
                return View("Edit");            
            return ToProfile(TabNames.Skills);
        }
        public override ActionResult Cancel() 
            => ToProfile(TabNames.Skills);
    }
}
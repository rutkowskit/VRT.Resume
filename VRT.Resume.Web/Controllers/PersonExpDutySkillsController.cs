using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using VRT.Resume.Application.Persons.Commands.MergePersonDutySkills;
using VRT.Resume.Application.Persons.Queries.GetPersonExperienceDutySkillList;

namespace VRT.Resume.Web.Controllers
{
    public sealed class PersonExpDutySkillsController : ControllerBase
    {

        public PersonExpDutySkillsController(IMediator mediator) : base(mediator)
        {
        }        
        // GET: Resume        
        [HttpGet]        
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonExperienceDutySkillListQuery()
            {
                DutyId = id
            };
            var result = await Mediator.Send(query);
            return ToActionResult(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MergePersonDutySkillsCommand skills)
        {
            var result = await Send(skills);
            if (result.IsFailure)
            {
                return View(nameof(Edit));
            }
            return ToProfile(TabNames.WorkExp);
        }

        [HttpGet]        
        public ActionResult Cancel() => ToProfile(TabNames.WorkExp);
    }
}
using System.Web.Mvc;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using VRT.Resume.Application.Persons.Queries.GetPersonData;
using VRT.Resume.Application.Persons.Queries.GetPersonEducation;
using VRT.Resume.Application.Persons.Queries.GetPersonSkills;
using VRT.Resume.Application.Persons.Queries.GetPersonExperience;

namespace VRT.Resume.Web.Controllers
{
    public partial class PersonController
    {
        [HttpGet]
        [Route(TabNames.Profile)]
        public async Task<ActionResult> ProfileTab()
        {
            SetTabName(TabNames.Profile);                        
            var result = await Mediator.Send(new GetPersonDataQuery());                
            return ToActionResult(result);
        }

        [HttpGet]
        [Route(TabNames.Education)]
        public async Task<ActionResult> EduTab()
        {
            SetTabName(TabNames.Education);            
            var result = await Mediator.Send(new GetPersonEducationListQuery());
            return ToActionResult(result);
        }

        [HttpGet]
        [Route(TabNames.Skills)]
        public async Task<ActionResult> SkillsTab()
        {
            SetTabName(TabNames.Skills);
            var result = await Mediator.Send(new GetPersonSkillListQuery());
            return ToActionResult(result);
        }

        [HttpGet]        
        [Route(TabNames.WorkExp)]
        public async Task<ActionResult> WorkExpTab()
        {
            SetTabName(TabNames.WorkExp);
            var result = await Mediator.Send(new GetPersonExperienceListQuery());
            return ToActionResult(result);           
        }

        private void SetTabName(string tabName=null, [CallerMemberName] string memberName = "")
        {
            TempData["TabName"] = tabName ?? memberName;
        }        
    }
}
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;
using VRT.Resume.Application.Resumes.Commands.MergeResumeSkills;
using VRT.Resume.Application.Resumes.Queries.GetResumeSkillList;

namespace VRT.Resume.Web.Controllers
{
    [RoutePrefix("ResumeSkills")]
    public class ResumeSkillsController : ControllerBase
    {
        public ResumeSkillsController(IMediator mediator):base(mediator)
        {            
        }
        // GET: Resume        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult> Index(int id)
        {
            var query = new GetResumeSkillListQuery()
            {
                ResumeId = id
            };
            var result = await Mediator.Send(query);
            return ToActionResult(result);            
        }

        [HttpPost]        
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(MergeResumeSkillsCommand skills)
        {
            var result = await Send(skills);
            if(result.IsFailure)
            {
                return View("Index");
            }
            return ToResume(skills.ResumeId);
        }

        [HttpGet]
        [Route("{id:int}/cancel")]
        public ActionResult Cancel(int id) => ToResume(id);

        private ActionResult ToResume(int resumeId)
            => RedirectToAction("/", "Resumes", new { id = resumeId });        
    }
}
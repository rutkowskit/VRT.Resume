using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using VRT.Resume.Application.Resumes.Commands.MergeResumeSkills;
using VRT.Resume.Application.Resumes.Queries.GetResumeSkillList;

namespace VRT.Resume.Mvc.Controllers
{
    [Route("ResumeSkills")]
    public class ResumeSkillsController : ControllerBase
    {
        public ResumeSkillsController(IMediator mediator):base(mediator)
        {            
        }
        // GET: Resume        
        [HttpGet]
        [Route("{id:int}")]
        public async Task<ActionResult> Index(int id, string returnUrl)
        {
            TempData[TempDataKeys.ReturnUrl] = returnUrl;
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
        public ActionResult Cancel() => ToReturnUrl() ?? ToHome();

        private ActionResult ToResume(int resumeId)
            => RedirectToAction("Show", "Resumes", new { id = resumeId });        
    }
}
using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;
using VRT.Resume.Application.Resumes.Queries.GetResume;

namespace VRT.Resume.Web.Controllers
{
    public class ResumesController : ControllerBase
    {      
        public ResumesController(IMediator mediator):base(mediator)
        {
        }
        // GET: Resume
        [Route("resumes/{id?}")]
        [Route("resumes/index/{id?}")]
        public async Task<ActionResult> Index(int? id)
        {            
            var query = new GetResumeQuery()
            {
                ResumeId = id.GetValueOrDefault()
            };
            var resume = await Mediator.Send(query);
            return View(resume);
        }             
    }
}
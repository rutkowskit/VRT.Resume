using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Resumes.Queries.GetResumeList;
using Microsoft.AspNetCore.Authorization;

namespace VRT.Resume.Mvc.Controllers
{
    public class HomeController : ControllerBase
    {
        public HomeController(IMediator mediator) : base(mediator)
        {
        }

        public async Task<ActionResult> Index()
        {
            var query = new GetResumeListQuery();
            var result = await Mediator.Send(query);
            return View(result);
        }

        [AllowAnonymous]
        public ActionResult About()
        {            
            return View();
        }        
    }
}
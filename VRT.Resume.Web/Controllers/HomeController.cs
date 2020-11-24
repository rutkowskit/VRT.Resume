using MediatR;
using System.Threading.Tasks;
using System.Web.Mvc;
using VRT.Resume.Application.Resumes.Queries.GetResumeList;

namespace VRT.Resume.Web.Controllers
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

        public ActionResult About()
        {            
            return View();
        }

        public ActionResult Contact()
        {
            return View();
        }        
    }
}
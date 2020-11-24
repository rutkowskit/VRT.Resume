using MediatR;
using System.Web.Mvc;

namespace VRT.Resume.Web.Controllers
{
    [RoutePrefix("person")]
    public partial class PersonController : ControllerBase
    {
        public PersonController(IMediator mediator): base(mediator)
        {
        }
                
        public ActionResult Index()
        {                    
            return View();
        }        
    }
}
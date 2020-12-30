using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace VRT.Resume.Mvc.Controllers
{
    [Route("person")]
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
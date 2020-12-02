using System.Web.Mvc;
using System.Threading.Tasks;
using MediatR;
using VRT.Resume.Application.Persons.Commands.UpsertProfileImage;
using System.Web;

namespace VRT.Resume.Web.Controllers
{
    public partial class PersonImageController : PersonEditControllerBase
    {
        public PersonImageController(IMediator mediator) : base(mediator)
        {
        }

        [HttpGet]                
        public ActionResult Edit()
        {
            return View();
        }               

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(HttpPostedFileBase file)
        {
            if (null == file)
            {
                ModelState.AddModelError("", Resources.MessageResource.ImageNotProvided);
                return View("Edit");                
            }
                
            var cmd = new UpsertProfileImageCommand()
            {
                ImageData = await file.ReadAllBytes(),
                ImageType = file.ContentType
            };
            await Mediator.Send(cmd);
            return ToProfile();            
        }             
    }
}
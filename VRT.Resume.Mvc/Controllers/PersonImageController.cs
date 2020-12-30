using System.Threading.Tasks;
using MediatR;
using VRT.Resume.Application.Persons.Commands.UpsertProfileImage;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace VRT.Resume.Mvc.Controllers
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
        public async Task<ActionResult> Save(IFormFile file)
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
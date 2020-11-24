using MediatR;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;

namespace VRT.Resume.Web.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IMediator _mediator;

        public ImagesController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public async Task<ActionResult> ProfileImage()
        {
            var query = new GetProfileImageQuery();
            var img = await _mediator.Send(query);

            return img?.ImageData == null
                ? GetDefaultProfileImage()
                : File(img.ImageData, img.ImageType);            
        }

        private FileContentResult GetDefaultProfileImage()
        {
            var dir = Server.MapPath("~/Content/img");
            var path = Path.Combine(dir, "unknown.png");
            var bytes = System.IO.File.Exists(path)
                ? System.IO.File.ReadAllBytes(path)
                : new byte[0];
            return File(bytes, "image/png");
        }
    }
}
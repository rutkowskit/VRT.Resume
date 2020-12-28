using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;
using VRT.Resume.Application.Persons.Queries.GetProfileImage;

namespace VRT.Resume.Mvc.Controllers
{
    public class ImagesController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IWebHostEnvironment _hosting;

        public ImagesController(IMediator mediator, IWebHostEnvironment hosting)
        {
            _mediator = mediator;
            _hosting = hosting;
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
            var path = Path.Combine(_hosting.WebRootPath, "img", "unknown.png");                
            var bytes = System.IO.File.Exists(path)
                ? System.IO.File.ReadAllBytes(path)
                : System.Array.Empty<byte>();
            return File(bytes, "image/png");
        }
    }
}
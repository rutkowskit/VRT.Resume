using AutoMapper;
using CSharpFunctionalExtensions;
using MediatR;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Resumes.Commands.ClonePersonResume;
using VRT.Resume.Application.Resumes.Commands.DeletePersonResume;
using VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers
{
    [Route("resumes")]    
    public class ResumesController : ControllerBase
    {
        private readonly IMapper _mapper;

        public ResumesController(IMediator mediator, IMapper mapper):base(mediator)
        {
            _mapper = mapper;
        }
        // GET: Resume
        [Route("{id}")]
        [Route("show/{id}")]
        [HttpGet]
        public async Task<ActionResult> Show(int id)
        {            
            var query = new GetFullResumeQuery()
            {
                ResumeId = id
            };
            var resume = await Mediator.Send(query);
            return ToActionResult(resume);
        }

        [HttpGet]
        [Route(nameof(Add))]
        public ActionResult Add() => View();

        [HttpGet]
        [Route("Edit/{id:int}")]
        public async Task<ActionResult> Edit(int id, string returnUrl="")
        {
            TempData[TempDataKeys.ReturnUrl] = returnUrl;
            var query = new GetResumeQuery()
            {
                ResumeId = id
            };
            var resume = await Mediator.Send(query)
                .Map(m => _mapper.Map<PersonResumeViewModel>(m));
            return ToActionResult(resume);
        }

        [HttpPost]
        [Route("Clone")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Clone(int entityId)            
        {
            var cmd = new ClonePersonResumeCommand()
            {
                ResumeId = entityId
            };
            await Send(cmd);
            return ToReturnUrl() ?? ToHome();
        }

        [HttpPost]
        [Route(nameof(Save))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Save(UpsertPersonResumeCommand data)
        {
            var result = await Send(data);
            if (result.IsFailure)
            {
                return View(nameof(Edit));
            }
            return ToReturnUrl() ?? ToHome(); 
        }

        [HttpGet]
        [Route(nameof(ConfirmDelete))]
        public virtual ActionResult ConfirmDelete(int id)
        {
            var ctrl = ControllerContext.RouteData.Values["controller"]?.ToString();
            var data = new EditDeleteToolbarData(ctrl, id, LabelNames.PageResume);
            return View(nameof(ConfirmDelete), data);
        }

        [HttpDelete]
        [Route(nameof(Delete))]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {
            var result = await Send(new DeletePersonResumeCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToReturnUrl() ?? ToHome();
        }
        [HttpGet]
        [Route(nameof(Cancel))]
        public ActionResult Cancel() => ToReturnUrl() ?? ToHome();
    }
}
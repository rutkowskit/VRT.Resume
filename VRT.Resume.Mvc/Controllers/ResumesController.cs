using CSharpFunctionalExtensions;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Resumes.Commands.ClonePersonResume;
using VRT.Resume.Application.Resumes.Commands.DeletePersonResume;
using VRT.Resume.Application.Resumes.Commands.UpsertPersonResume;
using VRT.Resume.Application.Resumes.Queries.GetResume;
using VRT.Resume.Mvc.Models;

namespace VRT.Resume.Mvc.Controllers;

[Route("resumes")]
public class ResumesController : ControllerBase
{
    public ResumesController(IMediator mediator) : base(mediator)
    {
    }
    [Route("index")]
    [HttpGet]
    public Task<ActionResult> Index() => Task.FromResult(ToHome());

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
        var resume = await Mediator.Send(query).ConfigureAwait(false);
        return ToActionResult(resume);
    }

    [HttpGet]
    [Route(nameof(Add))]
    public ActionResult Add() => View();

    [HttpGet]
    [Route("Edit/{id:int}")]
    public async Task<ActionResult> Edit(int id, string returnUrl = "")
    {
        TempData[TempDataKeys.ReturnUrl] = returnUrl;
        var query = new GetResumeQuery()
        {
            ResumeId = id
        };
        var resume = await Mediator.Send(query)
            .Map(ToViewModel)
            .ConfigureAwait(false);
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
        await Send(cmd).ConfigureAwait(false);
        return ToReturnUrl() ?? ToHome();
    }

    [HttpPost]
    [Route(nameof(Save))]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Save(UpsertPersonResumeCommand data)
    {
        var result = await Send(data).ConfigureAwait(false);
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
        var result = await Send(new DeletePersonResumeCommand(entityId))
            .ConfigureAwait(false);

        return result.IsFailure
            ? ToRequestReferer()
            : ToReturnUrl() ?? ToHome();
    }
    [HttpGet]
    [Route(nameof(Cancel))]
    public ActionResult Cancel() => ToReturnUrl() ?? ToHome();

    private static PersonResumeViewModel ToViewModel(ResumeVM dto)
    {
        return new PersonResumeViewModel()
        {
            ResumeId = dto.ResumeId,
            Position = dto.Position,
            Summary = dto.Summary,
            ShowProfilePhoto = dto.ShowProfilePhoto,
            DataProcessingPermission = dto.DataProcessingPermission,
            Description = dto.Description
        };
    }
}
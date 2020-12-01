using System.Threading.Tasks;
using System.Web.Mvc;
using CSharpFunctionalExtensions;
using MediatR;
using VRT.Resume.Application.Persons.Commands.DeletePersonContact;
using VRT.Resume.Application.Persons.Commands.UpsertPersonContact;
using VRT.Resume.Application.Persons.Queries.GetPersonContacts;
using VRT.Resume.Web.Models;

namespace VRT.Resume.Web.Controllers
{
    [ValidateInput(false)]
    public sealed class PersonContactsController : PersonEditControllerBase
    {
        public PersonContactsController(IMediator mediator) : base(mediator)
        {
        }
                
        [HttpGet]
        public async Task<ActionResult> Edit(int id)
        {
            var query = new GetPersonContactQuery()
            {
                ContactId = id
            };

            var result = await Mediator.Send(query)
                .Map(m => new PersonContactViewModel()
                {
                    ContactId = m.ContactId,
                    Value = m.Value,
                    Icon = m.Icon,
                    Name = m.Name,
                    Url = m.Url
                });
            return ToActionResult(result);               
        }

        [HttpDelete]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Delete(int entityId)
        {            
            var result = await Send(new DeletePersonContactCommand(entityId));
            if (result.IsFailure)
                return ToRequestReferer();
            return ToProfileAfterSave(TabNames.Contact);            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]        
        public async Task<ActionResult> Save(UpsertPersonContactCommand data)
        {            
            var result = await Send(data);
            
            if(result.IsFailure)
                return View("Edit");            
            return ToProfileAfterSave(TabNames.Contact);
        }
        public override ActionResult Cancel() 
            => ToProfile(TabNames.Contact);
    }
}
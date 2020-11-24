using System.Web;
using System.Web.Mvc;
using MediatR;
using Microsoft.Owin.Security;
using VRT.Resume.Web.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Owin.Security.Cookies;
using VRT.Resume.Application.Persons.Commands.CreatePersonAccount;

namespace VRT.Resume.Web.Controllers
{
    [Authorize]
    public class AccountController : ControllerBase
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]        
        public void SignIn(string type = "")
        {
            if (Request.IsAuthenticated) return;
            
            if (type == "Google")
            {
                HttpContext.GetOwinContext().Authentication
                    .Challenge(new AuthenticationProperties
                    { 
                        RedirectUri = "account/signin-google"
                    }, type );
            }            
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public async Task<ActionResult> SignOut()
        {
            await Task.Yield();
            if(Request.IsAuthenticated)
            {                   
                HttpContext.GetOwinContext().Authentication.SignOut();                
            }
            return Redirect("~/"); ;
        }

        [AllowAnonymous]        
        [Route("account/signin-google")]
        public async Task<ActionResult> GoogleLoginCallback()
        {
            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;
            var loginInfo = UserLoginViewModel.Create(claimsPrincipal);
            
            if (null== loginInfo)
                return RedirectToAction("Index");
            
            var command = new CreatePersonAccountCommand()
            {
                Email = loginInfo.Email,
                FirstName = loginInfo.FirstName,
                LastName = loginInfo.LastName
            };
            var userIdResult = await Mediator.Send(command);
            if(userIdResult.IsFailure)
            {
                TempData["ErrorMessage"] = userIdResult.Error;
                return await SignOut();
            }
            SignIn(loginInfo, userIdResult.Value);
            return Redirect("~/");
        }

        private void SignIn(UserLoginViewModel loginInfo, int personId)
        {           
            var ident = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, loginInfo.Email),
                new Claim(
                    "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                    "ASP.NET Identity",
                    "http://www.w3.org/2001/XMLSchema#string"),
                new Claim(ClaimTypes.GivenName, loginInfo.FirstName),
                new Claim(ClaimTypes.Surname, loginInfo.LastName),
                new Claim(ClaimTypes.Email, loginInfo.Email),
                new Claim("PersonId", personId.ToString())
            }, CookieAuthenticationDefaults.AuthenticationType);

            HttpContext.GetOwinContext().Authentication
                .SignIn(new AuthenticationProperties { IsPersistent = false }, ident);
        }
    }
}
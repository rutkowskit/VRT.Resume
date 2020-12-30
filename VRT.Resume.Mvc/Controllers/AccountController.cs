using MediatR;
using VRT.Resume.Mvc.Models;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VRT.Resume.Application.Persons.Commands.CreatePersonAccount;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Linq;

namespace VRT.Resume.Mvc.Controllers
{
    [Authorize]
    [Route("account")]
    public class AccountController : ControllerBase
    {
        public AccountController(IMediator mediator) : base(mediator)
        {
        }

        [AllowAnonymous]
        [Route("{returnUrl?}")]
        public ActionResult Index(string returnUrl)
        {
            TempData[TempDataKeys.ReturnUrl] = returnUrl;
            return View();
        }

        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ChallengeResult SignIn(string type = "")
        {
            if (User.Identity.IsAuthenticated) return null;

            if (type == "Google")
            {
                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = "account/signin-google"
                }, type);
            }
            else if (type == "Github")
            {
                return Challenge(new AuthenticationProperties
                {
                    RedirectUri = "account/signin-github",                    
                }, type);
            }
            return null;
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        [Route("logout")]
        public IActionResult LogOut()
        {
            if (!User.Identity.IsAuthenticated)
                return ToRequestReferer();
            SignOut();
            Response.Cookies.Delete(Globals.AuthCookieName);
            return ToRequestReferer();
        }

        [AllowAnonymous]
        [Route("signin-google")]
        [Route("signin-github")]
        public async Task<IActionResult> SignInCallback()
        {
            var claimsPrincipal = HttpContext.User.Identity as ClaimsIdentity;
            var loginInfo = UserLoginViewModel.Create(claimsPrincipal);

            if (null == loginInfo)
            {
                LogOut();
                return RedirectToAction("Index");
            }                

            var command = new CreatePersonAccountCommand()
            {
                UserId = loginInfo.UserId,
                Email = loginInfo.Email,
                FirstName = loginInfo.FirstName ?? "?",
                LastName = loginInfo.LastName ?? "?"
            };
            var userIdResult = await Send(command);
            if (userIdResult.IsFailure)
            {
                TempData["ErrorMessage"] = userIdResult.Error;
                return LogOut();
            }
            SignIn(loginInfo, userIdResult.Value);
            return ToReturnUrl() ?? ToHome();
        }

        private void SignIn(UserLoginViewModel loginInfo, int personId)
        {
            var claimsList = new[]
            {
                ToClaim(ClaimTypes.NameIdentifier, loginInfo.UserId),
                new Claim(
                    "http://schemas.microsoft.com/accesscontrolservice/2010/07/claims/identityprovider",
                    "ASP.NET Identity",
                    "http://www.w3.org/2001/XMLSchema#string"),
                ToClaim(ClaimTypes.GivenName, loginInfo.FirstName),
                ToClaim(ClaimTypes.Surname, loginInfo.LastName),
                ToClaim(ClaimTypes.Email, loginInfo.Email),
                ToClaim("PersonId", personId.ToString())
            }.Where(w => w != null);

            var ident = new ClaimsIdentity(claimsList, 
                CookieAuthenticationDefaults.AuthenticationScheme);

            var props = new AuthenticationProperties { IsPersistent = false };
            var claims = new ClaimsPrincipal(ident);
            SignIn(claims,props);
        }
        
        private static Claim ToClaim(string claimType, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return new Claim(claimType, value);
        }
    }
}
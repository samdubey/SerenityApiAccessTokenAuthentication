
namespace SereneApp.Membership.Pages
{
    using Serenity;
    using Serenity.Services;
    using System;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.IdentityModel.Tokens;
    using System.Security.Claims;
    using Microsoft.AspNetCore.Authorization;
    using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
    using System.Text;

    [Route("Api/Account/[action]")]
    public partial class AccountApiController : Controller
    {
        public static bool UseAdminLTELoginBox = false;


        [JsonFilter]
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateToken(LoginRequest request)
        {
            bool loggedIn = false;
            if (ModelState.IsValid)
            {
                request.CheckNotNull();

                if (string.IsNullOrEmpty(request.Username))
                    throw new ArgumentNullException("username");

                await Task.Run(() =>
                {
                    var username = request.Username;
                    if (WebSecurityHelper.Authenticate(ref username, request.Password, false))
                    {
                        loggedIn = true;
                    }

                });
                if (loggedIn)
                {
                    var claims = new[]
                    {
                        new Claim(JwtRegisteredClaimNames.NameId,request.Username),
                        new Claim(ClaimTypes.Name,request.Username),
                        new Claim(ClaimTypes.NameIdentifier,request.Username),
                        new Claim(JwtRegisteredClaimNames.UniqueName,request.Username),
                        new Claim(JwtRegisteredClaimNames.Sub, request.Username),
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    };

                    //var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
                    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("kDDlhs8pVhNIqVUCxdAOX0D"));
                    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                    var token = new JwtSecurityToken("https://localhost:44310", "https://localhost:44310",
                      claims,
                      expires: DateTime.Now.AddDays(365),
                      signingCredentials: creds);

                    return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
                }
                else
                {
                    //var error = new ServiceError();
                    //error.Code = "400A";
                    //error.Message = "Admission already taken";

                    return BadRequest(Texts.Validation.AuthenticationError);

                }
            }
            return BadRequest("Could not create token");
        }

        [HttpGet]
        public ActionResult Login(string activated)
        {
            ViewData["Activated"] = activated;
            ViewData["HideLeftNavigation"] = true;

            if (UseAdminLTELoginBox)
                return View(MVC.Views.Membership.Account.AccountLogin_AdminLTE);
            else
                return View(MVC.Views.Membership.Account.AccountLogin);
        }

        [HttpGet]
        public ActionResult AccessDenied(string returnURL)
        {
            ViewData["HideLeftNavigation"] = !Authorization.IsLoggedIn;

            return View(MVC.Views.Errors.AccessDenied, (object)returnURL);
        }

        [HttpPost, JsonFilter]
        public Result<ServiceResponse> Login(LoginRequest request)
        {
            return this.ExecuteMethod(() =>
            {
                request.CheckNotNull();

                if (string.IsNullOrEmpty(request.Username))
                    throw new ArgumentNullException("username");

                var username = request.Username;
                if (WebSecurityHelper.Authenticate(ref username, request.Password, false))
                    return new ServiceResponse();

                throw new ValidationError("AuthenticationError", Texts.Validation.AuthenticationError);
            });
        }

        private ActionResult Error(string message)
        {
            return View(MVC.Views.Errors.ValidationError,
                new ValidationError(Texts.Validation.InvalidResetToken));
        }

        public ActionResult Signout()
        {
            WebSecurityHelper.LogOut();
            return new RedirectResult("~/");
        }
    }
}
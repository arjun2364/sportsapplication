using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.Cors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sports_Application.Models;
using SportsApplication.Models;

namespace SportsApplication.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : Controller
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IOptions<ApplicationSettings> _appSettings;

        public AccountController(IUnitOfWork unitOfWork, IOptions<ApplicationSettings> appSettings)
        {
            this.unitOfWork = unitOfWork;
            this._appSettings = appSettings;
        }

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public async Task<IActionResult> IsEmailInUse(string email)
        {
            var user = await unitOfWork.Data.FindByEmailAsync(email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }

        [HttpGet]
        [Route("UserProfile")]
        //[AllowAnonymous]
        public string UserProfile()
        {
            return "hello";
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Register")]
        public async Task<Object> Register(RegisterViewModel model)
        {
            var roleCheck = await unitOfWork.Data.RoleExistsAsync("Coach");
            if (!roleCheck)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = "Coach"
                };

                await unitOfWork.Data.CreateRoleAsync(identityRole);

            }
            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            try
            {
                var result = await unitOfWork.Data.CreateUserAsync(user, model.Password);
                await unitOfWork.Data.AddToRoleAsync(user, "Coach");
                return Ok(result);
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }

        [HttpPost]
        public async Task<IActionResult> LogOut()
        {
            await unitOfWork.Data.SignOutAsync();
            return RedirectToAction("Login");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity.IsAuthenticated)
            {

                return RedirectToAction("Index", "Home");

            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("Login")]
        public async Task<Object> Login(LoginViewModel model)
        {

            var LoggedInUser = await unitOfWork.Data.FindByEmailAsync(model.Email);
            if (LoggedInUser != null)
            {
                var result = await unitOfWork.Data.PasswordSignInAsync(
                    LoggedInUser.UserName, model.Password, model.RememberMe);

                if (result.Succeeded)
                {


                    bool isCoach = await unitOfWork.Data.IsInRoleAsync(LoggedInUser, "Coach");
                    
                        var tokenDescriptor = new SecurityTokenDescriptor
                        {
                            Subject = new ClaimsIdentity(new Claim[]
                             {
                                     new Claim("UserID", LoggedInUser.Id.ToString())
                             }),
                            Expires = DateTime.UtcNow.AddDays(1),
                            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_appSettings.Value.JWT_Secret)), SecurityAlgorithms.HmacSha256Signature)
                        };
                        var tokenHandler = new JwtSecurityTokenHandler();
                        var securityToken = tokenHandler.CreateToken(tokenDescriptor);
                        var token = tokenHandler.WriteToken(securityToken);
                    if (isCoach)
                    {
                        return Ok(new { token, Id = LoggedInUser.Id.ToString(), Role="Coach" });
                    }
                    else
                    {
                        return Ok(new { token, Id=LoggedInUser.Id.ToString(), Role = "Athelete" });
                    }

                }
                else
                {
                    return BadRequest(new { message = "Username or password is incorrect." });
                }
            }
            else
            {
                return BadRequest(new { message = "Username not exists." });
            }
        }
    }    
}
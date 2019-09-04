using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sports_Application.Models;
using SportsApplication.Models;

namespace SportsApplication.Controllers
{
    public class AtheleteController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public AtheleteController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet]
        [Authorize(Roles ="Athelete")]
        public IActionResult Index(string Id)
        {
            List<AtheleteViewModel> li= unitOfWork.Data.GetAtheleteData(Id);
            return View(li);
        }
        
        [HttpGet]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AtheleteList()
        {
            var users = await unitOfWork.Data.GetUsersInRoleAsync("Athelete");
            return View(users);
        }

        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> DeleteAthelete(string Id)
        {
            var user = await unitOfWork.Data.FindByIdAsync(Id);
            var rolesForUser = await unitOfWork.Data.GetRolesAsync(user);
            if (rolesForUser.Count() > 0)
            {
                foreach (var item in rolesForUser.ToList())
                {
                     var result = await unitOfWork.Data.RemoveFromRoleAsync(user, item);
                }
            }

            await unitOfWork.Data.DeleteAsync(user);
            unitOfWork.Commit();
            return RedirectToAction("AtheleteList");
        }

        [Authorize(Roles = "Coach")]
        public IActionResult AddAthelete()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> AddAthelete(RegisterViewModel model)
        {
            model.Password = model.UserName.ToUpper() + "athelete" + "@123";
            model.ConfirmPassword = model.Password;
            if (ModelState.IsValid)
            {
                var roleCheck = await unitOfWork.Data.RoleExistsAsync("Athelete");
                if (!roleCheck)
                {
                    IdentityRole identityRole = new IdentityRole
                    {
                        Name = "Athelete"
                    };

                    IdentityResult res = await unitOfWork.Data.CreateRoleAsync(identityRole);
                }
                var user = new ApplicationUser
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    FirstName = model.FirstName,
                    LastName = model.LastName
                };
                
                var result = await unitOfWork.Data.CreateUserAsync(user, model.Password);

                if (result.Succeeded)
                {
                    await unitOfWork.Data.AddToRoleAsync(user, "Athelete");
                    return RedirectToAction("AtheleteList");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }
    }
}
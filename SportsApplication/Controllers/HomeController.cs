using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sports_Application.Models;
using SportsApplication.Models;

namespace SportsApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        
        public async Task<IActionResult> Index()
        {
            var Tests = unitOfWork.Data.GetAllTestData();
            var user = await unitOfWork.Data.GetUserAsync(HttpContext.User);
            bool isCoach = await unitOfWork.Data.IsInRoleAsync(user, "Coach");
            if (isCoach)
            {
                return View(unitOfWork.Data.GetTests(user.Id));
            }
            else
            {
                return RedirectToAction("Index", "Athelete", user.Id);
            }
            
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}

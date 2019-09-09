﻿using System;
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
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : Controller
    {

        private readonly IUnitOfWork unitOfWork;

        public TestController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [Authorize(Roles = "Coach")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateTest()
        {
            Test test = new Test();
            var user = await unitOfWork.Data.GetUserAsync(HttpContext.User);
            test.CoachId = user.Id;
            return View(test);
        }

        [HttpPost]
        [Authorize]
        [Route("createTest")]
        public IActionResult CreateTest(Test test)
        {

            unitOfWork.Data.AddTest(test);
            unitOfWork.Commit();
            return Ok(new { test }); 
            
            
        }

        //[Authorize(Roles = "Coach")]
        [Route("deleteTest/{Id}")]
        public object DeleteTest(int Id)
        {
            unitOfWork.Data.DeleteTestByTestid(Id);
            unitOfWork.Commit();
            return Ok();
        }

        [Authorize(Roles = "Coach")]
        public IActionResult ConfirmDeleteTest(int Id)
        {  
            return View(unitOfWork.Data.GetTestByid(Id));
        }
    }
}
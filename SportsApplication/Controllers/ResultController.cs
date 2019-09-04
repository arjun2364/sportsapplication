using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sports_Application.Models;
using SportsApplication.Models;

namespace SportsApplication.Controllers
{
    public class ResultController : Controller
    {
        private readonly IUnitOfWork unitOfWork;

        public ResultController(IData data,IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }
        [Authorize(Roles = "Coach")]
        public IActionResult ViewResult(int TestId)
        {

            var test = unitOfWork.Data.GetTestByid(TestId);
            ViewResultModel obj = new ViewResultModel
            {
                Date = test.Date,
                TestType = test.TestType,
                TestId = TestId,
                AtheleteNames = unitOfWork.Data.GetAtheleteNamesWithDataByTestId(TestId)
            };
            return View(obj);
        }
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateResult(int TestId)
        {
            var resultmodel = new CreateResultModel
            {
                TestId = TestId,
                AtheleteList = await unitOfWork.Data.GetAllAtheleteList()
            };
            return View(resultmodel);
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> CreateResult(CreateResultModel resultmodel)
        {
            Result result = new Result
            {
                
                Id = resultmodel.Id,
                TestId = resultmodel.TestId,
                Data = resultmodel.Data,
                UserId = resultmodel.UserId
            };
            resultmodel.AtheleteList = await unitOfWork.Data.GetAllAtheleteList();
            int status = unitOfWork.Data.AddResult(result);
            if(status==0)
            {
                ViewBag.message = "Athelete already exists";
                return View(resultmodel);
            }
            else
            {

                unitOfWork.Data.IncrementCountByTestId(resultmodel.TestId);
                unitOfWork.Commit();
                return RedirectToAction("ViewResult", "Result", new { testid = resultmodel.TestId });
            }
            
        }

        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> EditResult(int Id)
        {
            var result = unitOfWork.Data.GetResultById(Id);
            var atheletes = unitOfWork.Data.GetAtheleteNamesWithDataByTestId(result.TestId);
            CreateResultModel ResultModel = new CreateResultModel
            {
                TestId = result.TestId,
                UserId = result.UserId,
                Data = result.Data,
                Id = result.Id,
                AtheleteList = await unitOfWork.Data.GetAllAtheleteList()
            };
            return View(ResultModel);
        }

        [HttpPost]
        [Authorize(Roles = "Coach")]
        public async Task<IActionResult> EditResult(CreateResultModel ResultModel)
        {
            ResultModel.AtheleteList = await unitOfWork.Data.GetAllAtheleteList();
            Result result = new Result
            {
                Id = ResultModel.Id,
                TestId = ResultModel.TestId,
                Data = ResultModel.Data,
                UserId = ResultModel.UserId
            };
            int status = unitOfWork.Data.Update(result);
            if (status==0)
            {
                ViewBag.message = "Athelete already exists";
                return View(ResultModel);
            }
            else
            {

                unitOfWork.Commit();
                return RedirectToAction("ViewResult", "Result", new { testid = ResultModel.TestId });
            }
            
           
            
        }

        [Authorize(Roles = "Coach")]
        public IActionResult DeleteResult(int Id,int TestId)
        {
            unitOfWork.Data.DeleteTestResultById(Id);
            unitOfWork.Data.DecrementCountByTestId(TestId);
            unitOfWork.Commit();
            return RedirectToAction("Index", "Home");
        }
        [Authorize(Roles = "Coach")]
        public IActionResult ConfirmDeleteResult(int Id)
        {
            return View(unitOfWork.Data.GetResultById(Id));
        }
    }
}
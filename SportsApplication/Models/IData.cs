using Microsoft.AspNetCore.Identity;
using SportsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sports_Application.Models
{
    public interface IData
    {
        IEnumerable<Test> GetAllTestData();
        Test AddTest(Test Test);
        List<AtheleteNameWithData> GetAtheleteNamesWithDataByTestId(int id);
        void DeleteTestByTestid(int Id);
        int AddResult(Result Result);
        Test GetTestByid(int Id);
        Task<List<Athelete>> GetAllAtheleteList();
        void IncrementCountByTestId(int Id);
        void DecrementCountByTestId(int Id);
        Result GetResultById(int Id);
        int Update(Result updatedResult);
        void DeleteTestResultById(int Id);
        List<AtheleteViewModel> GetAtheleteData(string Id);
        Task<ApplicationUser> FindByEmailAsync(string Email);
        Task<bool> RoleExistsAsync(string Role);
        Task<IdentityResult> CreateRoleAsync(IdentityRole identityRole);
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task SignOutAsync();
        Task<SignInResult> PasswordSignInAsync(string UserName, string Password, bool RememberMe);
        Task<bool> IsInRoleAsync(ApplicationUser LoggedInUser, string role);
        Task SignInAsync(ApplicationUser user, bool isPersistent);
        Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role);
        Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role);
        Task<ApplicationUser> FindByIdAsync(string id);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string item);
        Task<IdentityResult> DeleteAsync(ApplicationUser user);
        Task<ApplicationUser> GetUserAsync(ClaimsPrincipal HttpContextUser);
        IEnumerable<Test> GetTests(string Id);
    }
}

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SportsApplication.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Sports_Application.Models
{
    public class SqlData : IData
    {
        private readonly SportsApplicationDbContext db;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public SqlData(SportsApplicationDbContext db,
                        UserManager<ApplicationUser> userManager, 
                        RoleManager<IdentityRole> roleManager,
                        SignInManager<ApplicationUser> signInManager)
        {
            this.db = db;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.signInManager = signInManager;
        }

        public IEnumerable<Test> GetAllTestData()
        {
            return db.Tests.OrderByDescending(item=>item.Date).ToList();
        }

        public Test AddTest(Test test)
        {
            db.Tests.Add(test);
            return test;
        }

        public List<AtheleteNameWithData> GetAtheleteNamesWithDataByTestId(int id)
        {
            List<AtheleteNameWithData> li = new List<AtheleteNameWithData>();
            var query = from r in db.Results
                        where r.TestId.Equals(id)
                        select r;
            foreach (var result in query)
            {
                foreach (var athelete in db.Users)
                {
                    if(result.UserId==athelete.Id)
                    {
                        if(result.Data>3500)
                            li.Add(new AtheleteNameWithData{Id=result.Id,Name=athelete.FirstName+" "+athelete.LastName, Data=result.Data, FitnessRanking="Very good"});
                        if (result.Data > 2000 && result.Data < 3501)
                            li.Add(new AtheleteNameWithData { Id = result.Id, Name = athelete.FirstName + " " + athelete.LastName, Data = result.Data, FitnessRanking = "Good" });
                        if (result.Data > 1000 && result.Data < 2001)
                            li.Add(new AtheleteNameWithData { Id = result.Id, Name = athelete.FirstName + " " + athelete.LastName, Data = result.Data, FitnessRanking = "Average" });
                        if (result.Data<1001)
                            li.Add(new AtheleteNameWithData { Id = result.Id, Name = athelete.FirstName + " " + athelete.LastName, Data = result.Data, FitnessRanking = "Below Average" });
                    }
                }
            }

            return li.OrderByDescending(item => item.Data).ToList();
        }

        public void DeleteTestByTestid(int id)
        {
            var query = (from t in db.Tests
                        where t.Id.Equals(id)
                        select t).Single();
            db.Tests.Remove(query);

            var query2 = from r in db.Results
                        where r.TestId.Equals(id)
                        select r;
            if(query2!=null)
            {
                foreach (var item in query2)
                {
                    db.Results.Remove(item);
                }
            }
        }
        public int AddResult(Result result)
        {
            var query = from r in db.Results
                        where r.TestId.Equals(result.TestId) && r.UserId.Equals(result.UserId)
                        select r;
            if(query.FirstOrDefault() == null)
            {
                db.Results.Add(result);
                return 1;
            }
            else
            {
                return 0;
            }
            
            
        }

        public Test GetTestByid(int id)
        {
            var query = from t in db.Tests
                        where t.Id.Equals(id)
                        select t;
            return query.FirstOrDefault();
        }

        public async Task<List<Athelete>> GetAllAtheleteList()
        {
            List<Athelete> li = new List<Athelete>();
            foreach (var user in userManager.Users)
            {
                if (await userManager.IsInRoleAsync(user, "Athelete"))
                {
                    li.Add(new Athelete { id=user.Id, name = user.FirstName+" "+user.LastName });
                }
            }
            return li;
        }

        public void IncrementCountByTestId(int id)
        {
            var query = (from t in db.Tests
                        where t.Id.Equals(id)
                        select t).SingleOrDefault();
            query.Count++;
        }

        public void DecrementCountByTestId(int id)
        {
            var query = (from t in db.Tests
                         where t.Id.Equals(id)
                         select t).SingleOrDefault();
            query.Count--;
        }

        public Result GetResultById(int id)
        {
            var query =  from r in db.Results
                         where r.Id.Equals(id)
                         select r;
            return query.First();
        }

        public int Update(Result updatedResult)
        {
            var query2 = from r in db.Results
                        where r.Id != updatedResult.Id && r.TestId.Equals(updatedResult.TestId) && r.UserId.Equals(updatedResult.UserId)
                        select r;
            if(query2.FirstOrDefault()==null)
            {
                var query = from r in db.Results
                            where r.TestId.Equals(updatedResult.TestId) && r.UserId.Equals(updatedResult.UserId)
                            select r;
                var entity = db.Results.Attach(updatedResult);
                entity.State = EntityState.Modified;
                return 1;
            }
            else
            {
                return 0;
            }
            
        }


        public void DeleteTestResultById(int id)
        {
            var query = (from t in db.Results
                         where t.Id.Equals(id)
                         select t).Single();
            db.Results.Remove(query);
        }

        public List<AtheleteViewModel> GetAtheleteData(string Id)
        {
            List<AtheleteViewModel> li = new List<AtheleteViewModel>();
            var query = from r in db.Results
                        where r.UserId.Equals(Id)
                        select r;
            foreach (var item in query)
            {
                foreach (var item2 in db.Users)
                {
                    if(item2.Id==item.UserId)
                    {
                        var query2 = (from t in db.Tests
                                     where t.Id.Equals(item.TestId)
                                     select t).Single();
                        li.Add(new AtheleteViewModel { CoachName = item2.UserName, TestName = query2.TestType, Date = query2.Date, Data = item.Data });
                    }
                }
            }
            return li;
            
        }
        public async Task<ApplicationUser> FindByEmailAsync(string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            return user;
        }

        public async Task<bool> RoleExistsAsync(string Role)
        {
            return await roleManager.RoleExistsAsync(Role);
        }

        public async Task<IdentityResult> CreateRoleAsync(IdentityRole identityRole)
        {
            var r = await roleManager.CreateAsync(identityRole);
            return r;
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser User, string Password)
        {
            var result = await userManager.CreateAsync(User, Password);
            return result;
        }

        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }

        public Task<SignInResult> PasswordSignInAsync(string UserName, string Password, bool RememberMe)
        {
            var result = signInManager.PasswordSignInAsync(UserName, Password, RememberMe, false);
            return result;
        }

        public async Task<bool> IsInRoleAsync(ApplicationUser LoggedInUser, string role)
        {
            var result = await userManager.IsInRoleAsync(LoggedInUser, role);
            return result;
        }
        public async Task SignInAsync(ApplicationUser user, bool isPersistent)
        {
            await signInManager.SignInAsync(user, isPersistent);
        }

        public async Task<IdentityResult> AddToRoleAsync(ApplicationUser user, string role)
        {
            var result = await userManager.AddToRoleAsync(user, role); 
            return result;
        }

        public async Task<IList<ApplicationUser>> GetUsersInRoleAsync(string role)
        {
            var users = await userManager.GetUsersInRoleAsync(role);
            return users;
        }
        public async Task<ApplicationUser> FindByIdAsync(string id)
        {
            var user = await userManager.FindByIdAsync(id);
            return user;
        }
        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            var rolesForUser = await userManager.GetRolesAsync(user);
            return rolesForUser;
        }

        public async Task<IdentityResult> RemoveFromRoleAsync(ApplicationUser user, string item)
        {
            var result = await userManager.RemoveFromRoleAsync(user, item);
            return result;
        }

        public async Task<IdentityResult> DeleteAsync(ApplicationUser user)
        {
            var result = await userManager.DeleteAsync(user);
            return result;
        }

        public async Task<ApplicationUser> GetUserAsync(ClaimsPrincipal HttpContextUser)
        {
            var user = await userManager.GetUserAsync(HttpContextUser);
            return user;
        }

        public IEnumerable<Test> GetTests(string Id)
        {
            return db.Tests.Where(r => r.CoachId.Equals(Id));
        }

    }
}

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using ReservationSystem.Data.Models;

namespace ReservationSystem.Data.Services
{
    public class UserService
    {
        private UserManager<AppUser> _userManager;
        private RoleManager<IdentityRole> _roleManager;
        private ApplicationDbContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ApplicationDbContext dbContext, UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContextAccessor = httpContextAccessor;
            InitializeRoles();
        }

        private void InitializeRoles()
        {
            List<IdentityRole> rolesToCreate = new List<IdentityRole>()
            {
                new IdentityRole("Admin"),
                new IdentityRole("Registered"),
                new IdentityRole("Verified")
            };
            var roleStore = new RoleStore<IdentityRole>(_dbContext);
            List<IdentityRole> roles = roleStore.Roles.ToList();
            foreach (var r in rolesToCreate)
            {
                if (!roles.Contains(r))
                {
                    _roleManager.CreateAsync(r);
                }
            }
        }

        public async Task<AppUser> GetCurrentUser()
        {
            var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext.User);
            return user;
        }

        public List<AppUser> GetAllUsers()
        {
            return _userManager.Users.ToList();
        }

        public List<string> GetUserRoles(AppUser user)
        {
            return _userManager.GetRolesAsync(user).Result.ToList();
        }

        public async Task<IdentityResult> VerifyUser(AppUser user)
        {
            var result = _userManager.AddToRoleAsync(user, "Verified");
            return result.Result;
        }

        public void UpdateUserData(AppUser user)
        {
            _dbContext.Users.Update(user);
            _dbContext.SaveChanges();
        }
    }
}
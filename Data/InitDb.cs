using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace FillableFormWebApp.Data
{
    public class InitDb
    {
        public class createUserModel
        {
            public string Email { get; set; }
            public string Password { get; set; }
            public string? Role { get; set; }
        }
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IUserStore<IdentityUser> _userStore;
        private readonly IUserEmailStore<IdentityUser> _emailStore;
        private IServiceScope _serviceScope;
        public InitDb(IServiceScope serviceScope)
        {
            _serviceScope = serviceScope;
            _userManager = _serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            _userStore = _serviceScope.ServiceProvider.GetRequiredService<IUserStore<IdentityUser>>();
            _emailStore = GetEmailStore();
        }

        /// <summary>
        /// Apply the FormApp and Identity Migrations
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void ApplyMigrations()
        {
            if (_serviceScope == null)
                throw new Exception("No service scope");

            _serviceScope.ServiceProvider.GetService<FillableFormWebAppContext>().Database.Migrate();
            _serviceScope.ServiceProvider.GetService<ApplicationDbContext>().Database.Migrate();
        }

        public async Task createRoles(string[]? roles = null)
        {
            var roleManager = _serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        /// <summary>
        /// Create user with provided Email, password and role.<br />
        /// This method won't check if the Email exist in the Employee table
        /// </summary>
        /// <param name="createUsers"></param>
        /// <returns></returns>
        public async Task CreateTestUsers(List<createUserModel> createUsers)
        {
            foreach (var user in createUsers)
            {
                if (await _userManager.FindByEmailAsync(user.Email) == null)
                {
                    IdentityUser identityUser = new();
                    identityUser.Email = user.Email;
                    identityUser.UserName = user.Email;
                    identityUser.EmailConfirmed = true;

                    await _userStore.SetUserNameAsync(identityUser, identityUser.Email, CancellationToken.None);
                    await _emailStore.SetEmailAsync(identityUser, identityUser.Email, CancellationToken.None);

                    var result = await _userManager.CreateAsync(identityUser, user.Password);
                    if (result.Succeeded)
                    {
                        if(user.Role != null)
                            await _userManager.AddToRoleAsync(identityUser, user.Role);
                    }
                    else
                    {
                        throw new Exception(result.Errors.ToString());  
                    }
                }
            }
        }

        private IUserEmailStore<IdentityUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<IdentityUser>)_userStore;
        }
    }
}

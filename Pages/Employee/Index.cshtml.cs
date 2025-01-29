using FillableFormWebApp.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace FillableFormWebApp.Pages.employee
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FillableFormWebAppContext _context;
        public Models.Form[] forms { get; set; }

        public IndexModel(
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            FillableFormWebAppContext context
            )
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _context = context;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return NotFound();
            
            forms = _context.Forms.Include(f => f.FormType)
                                .Include(f => f.Supervisor.Employee)
                                .Where(f => f.Employee.Email == user.Email)
                                .ToArray();

            foreach (var form in forms)
            {
                if (form.TypeOfLeave == "Other")
                    form.TypeOfLeave = form.Other;
            }

            return Page();
        }
    }
}

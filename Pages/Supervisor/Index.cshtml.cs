using FillableFormWebApp.Data;
using FillableFormWebApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace FillableFormWebApp.Pages.Supervisor
{
    [Authorize(Roles = "Supervisor, Admin")]
    public class IndexModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly FillableFormWebAppContext _context;        
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
        public Form[] Forms { get; set; }
        public Department[] Departments { get; set; }
        public FormType[] FormTypes { get; set; }
        public string[] Status { get; set; }
        public Models.Employee[] Employees { get; set; }

        [BindProperty(SupportsGet = true)]
        public InputModel Input { get; set; }

        public class InputModel()
        {
            [Display(Name = "Employee")]
            public int? EmployeeId { get; set; }

            [Display(Name = "Form Type")]
            public int? FormTypeId { get; set; }

            [Display(Name = "Status")]
            public string? Status { get; set; }

            [Display(Name = "Department")]
            public int? DepartmentId { get; set; }
        }

        public ActionResult OnGet()
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            if (user == null) return NotFound();

            Forms = _context.Forms.Include(f => f.FormType)
                                .Include(f => f.Supervisor.Employee)
                                .Where(f => f.Supervisor.Employee.Email == user.Email)
                                .Include(f => f.Employee)
                                .Include(f => f.Employee.Department)
                                .ToArray();
            if(Forms != null)
            {
                Status = Forms.GroupBy(f => f.Status).Select(g => g.First()).Select(f => f.Status).ToArray();
                Employees = Forms.Select(f => f.Employee).GroupBy(f => f.EmployeeId).Select(g => g.First()).ToArray();
                Departments = Forms.Select(f => f.Employee.Department).GroupBy(f => f.DepartmentId).Select(g => g.First()).ToArray();
                FormTypes = Forms.Select(f => f.FormType).GroupBy(f => f.FormTypeId).Select(g => g.First()).ToArray();
            }
            
            if (Input != null)
            {
                if (Input.EmployeeId != null)
                    Forms = Forms.Where(f => f.Employee.EmployeeId == Input.EmployeeId).ToArray();

                if (Input.FormTypeId != null)
                    Forms = Forms.Where(f => f.FormTypeId == Input.FormTypeId).ToArray();

                if (Input.Status != null)
                    Forms = Forms.Where(f => f.Status == Input.Status).ToArray();

                if (Input.DepartmentId != null)
                    Forms = Forms.Where(f => f.Employee.DepartmentId == Input.DepartmentId).ToArray();
            }
            
            return Page();
        }
    }
}

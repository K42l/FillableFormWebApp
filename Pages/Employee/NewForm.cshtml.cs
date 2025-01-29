using FillableFormWebApp.Data;
using FillableFormWebApp.Models;
using FillableFormWebApp.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using static FillableFormWebApp.Models.Form;

namespace FillableFormWebApp.Pages.Employee
{
    [Authorize]
    public class NewFormModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly FillableFormWebAppContext _context;
        public List<SelectListItem> TypeOfLeave { get; set; }
        public Models.Employee? employee { get; set; }
        private IQueryable<DepartmentSupervisor?> departmentSupervisors { get; set; }
        public List<Models.Supervisor> supervisors { get; set; }
        private int _formTypeId = 1;

        public NewFormModel(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            FillableFormWebAppContext context,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _emailSender = emailSender;

            TypeOfLeave = new()
            {
                new SelectListItem {Value = "Annual Leave", Text = "Annual Leave"},
                new SelectListItem {Value = "Sick Leave", Text = "Sick Leave"},
                new SelectListItem {Value = "Compensatory Time Off", Text = "Compensatory Time Off"},
                new SelectListItem {Value = "Unpaid Absence", Text = "Unpaid Absence"},
                new SelectListItem {Value = "Other", Text = "Other"}
            };
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required]
            [Display(Name = "Supervisor")]
            public int Supervisor { get; set; }

            [Required]
            [Display(Name = "Purpose")]
            [MaxLength(298)]
            public string Purpose { get; set; }

            [Required]
            [Display(Name = "DatesOfLeave")]
            [MaxLength(88)]
            public string DatesOfLeave { get; set; }

            [Required]
            [Display(Name = "TypeOfLeave")]
            public string TypeOfLeave { get; set; }

            [Display(Name = "Other")]
            [MaxLength(70)]
            public string? Other { get; set; } = null;

            [Display(Name = "AdditionalRemarks")]
            [MaxLength(296)]
            public string? AdditionalRemarks { get; set; } = null;
        }
        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                await SetEmployee();
                return Page();
            }
            return NotFound();
        }

        public async Task<IActionResult> OnPost()
        {
            if (ModelState.IsValid)
            {
                string? error = await IsFormValid();
                if (error != null)
                {
                    ModelState.AddModelError(string.Empty, error);
                    return Page();
                }
                else
                {

                    Models.Form form = new()
                    {
                        EmployeeId = employee.EmployeeId,
                        SupervisorId = Input.Supervisor,
                        FormTypeId = _formTypeId,
                        FormDate = DateTime.Now.Date,
                        Justification = Input.Purpose,
                        Dates = Input.DatesOfLeave,
                        TypeOfLeave = Input.TypeOfLeave,
                        Other = Input.Other,
                        AdditionalRemarks = Input.AdditionalRemarks,
                        Status = "Open"
                    };

                    await _context.Forms.AddAsync(form);

                    var result = await _context.SaveChangesAsync();

                    if (result > 0)
                    {
                        ViewData["IsSuccess"] = "true";
                        ModelState.Clear();

                        var superVisor = _context.Supervisors.Where(e => e.EmployeeId == form.SupervisorId).Include(e => e.Employee).SingleOrDefault();
                        var employeeName = _context.Employees.Where(e => e.EmployeeId == employee.EmployeeId).SingleOrDefault();
                        var url = "FillableFormWebAppUrl";
                        var htmlMessage = $"The employee, {employeeName}, created a new form. <br /> <a href=\"{url}\" targe=\"_blank\">Click here</a> to access it.";

                        await _emailSender.SendEmailAsync(superVisor.Employee.Email, $"New Form from {employeeName}", "The employee ");
                        return Page();
                    }
                }
            }
            return Page();
        }

        private async Task SetEmployee()
        {
            try
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    employee = _context.Employees.Include(e => e.Department)
                                                .Where(e => e.Email == user.Email).SingleOrDefault();

                    departmentSupervisors = _context.DepartmentSupervisors.Include(d => d.Supervisor)
                                                                          .Where(d => d.DepartmentId == employee.DepartmentId);
                    supervisors = new List<Models.Supervisor>();
                    foreach (var departmentSupervisor in departmentSupervisors)
                    {
                        departmentSupervisor.Supervisor.Employee = _context.Employees.Where(e => e.EmployeeId == departmentSupervisor.Supervisor.EmployeeId).SingleOrDefault();
                        supervisors.Add(departmentSupervisor.Supervisor);
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }

        private async Task<string?> IsFormValid()
        {
            await SetEmployee();
            if (Input.TypeOfLeave.Length == 0)
            {
                return "Choose the type of leave";
            }
            else
            {
                if (Input.TypeOfLeave == "Other" && String.IsNullOrEmpty(Input.Other))
                {
                    return "Specify the type of leave";
                }
                else
                {
                    if (!TypeOfLeave.Any(v => v.Value == Input.TypeOfLeave))
                        return "Invalid Type of leave";
                }
               
                if (!supervisors.Any(s => s.SupervisorId == Input.Supervisor))
                    return "Invalid Supervisor";
                
                return null;
            }
        }
    }
}

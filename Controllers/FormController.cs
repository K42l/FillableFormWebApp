using FillableFormWebApp.Data;
using FillableFormWebApp.Interfaces;
using FillableFormWebApp.Models;
using FillableFormWebApp.Util;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PdfSharp.Charting;

namespace FillableFormWebApp.Controllers
{
    [ApiController]
    [Route("Api/Form")]
    public class FormController : Controller
    {
        private readonly FillableFormWebAppContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IPdfUtil _pdfUtil;

        public FormController(FillableFormWebAppContext context, UserManager<IdentityUser> userManager, IPdfUtil pdfUtil)
        {
            _context = context;
            _userManager = userManager;
            _pdfUtil = pdfUtil;
        }

        [HttpPatch]
        [Route("AlterFormSupervisor/{formId}")]
        [Authorize(Roles = "Supervisor")]
        public ActionResult AlterFormSupervisor(int formId, [FromBody]Form form)
        {
            try
            {
                var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();

                var userEmployee = _context.Employees.Where(e => e.Email == user.Email).SingleOrDefault();
                if (userEmployee is null) 
                    return Unauthorized("User not found as employee");

                var userSupervisor = _context.Supervisors.Where(s => s.EmployeeId == userEmployee.EmployeeId).SingleOrDefault();
                if (userSupervisor is null) 
                    return Unauthorized("User not found as a supervisor");

                var oForm = _context.Forms.Where(f => f.FormId == formId).SingleOrDefault();
                if (oForm is null)
                    return NotFound("Form id not found");

                if (userSupervisor.SupervisorId == oForm.SupervisorId)
                {
                    if (oForm.Status != "Open")
                        return Unauthorized("The form is not open");

                    if(form.Decision == "Disapprove")
                    {
                        if (String.IsNullOrEmpty(form.Decision))
                            return BadRequest("Provide a Deicison for disapproval");
                    }

                    form.Decision = form.Decision + "d";
                    form.FormId = formId;
                    form.Status = form.Decision;

                    _context.Forms.Entry(oForm).State = EntityState.Detached;
                    _context.Forms.Entry(form).Property(p => p.Decision).IsModified = true;
                    _context.Forms.Entry(form).Property(p => p.Reason).IsModified = true;
                    _context.Forms.Entry(form).Property(p => p.Status).IsModified = true;

                    var result = _context.SaveChanges();

                    if (result == 1) 
                        return Ok("Form updated successfully");

                    return StatusCode(500, $"Update returned 0 or more than 1 result. Update result: {result}");
                }
                else
                {
                    return Unauthorized("The form's supervisor id doesn't match your user id");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }           
        }

        [HttpGet("GetForm/{formId}")]
        [Authorize]
        public IActionResult GetForm(int formId)
        {
            var user = _userManager.GetUserAsync(User).GetAwaiter().GetResult();
            var form = _context.Forms.Where(f => f.FormId == formId).SingleOrDefault();
            form.Employee = _context.Employees.Where(e => e.EmployeeId == form.EmployeeId).SingleOrDefault();
            
            var userEmployee = _context.Employees.Where(e => e.Email == user.Email).SingleOrDefault();
            if (userEmployee is null)
                return Unauthorized("User not found as employee");

            form.Supervisor = _context.Supervisors.Where(s => s.EmployeeId == userEmployee.EmployeeId).SingleOrDefault();

            form.Employee.Department = _context.Departments.Where(d => d.DepartmentId == form.Employee.DepartmentId).SingleOrDefault();

            var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            if (!roles.Contains("Admin") || !roles.Contains("HR"))
            {
                if (roles.Contains("Supervisor"))
                {
                    var userSupervisor = _context.Supervisors.Where(s => s.EmployeeId == userEmployee.EmployeeId).SingleOrDefault();
                    if(userSupervisor.SupervisorId != form.SupervisorId)
                    {
                        return Unauthorized("The form's supervisor id doesn't match your user id");
                    }
                }
                else if (form.EmployeeId != userEmployee.EmployeeId)
                {
                    return Unauthorized("The form's employee id doesn't match your user id");
                }
            }
            try
            {
                using (var stream = _pdfUtil.FillForm(form))
                {
                    byte[] buffer = new byte[stream.Length];
                    for (int totalBytes = 0; totalBytes < stream.Length;)
                    {
                        totalBytes += stream.Read(buffer, totalBytes, Convert.ToInt32(stream.Length) - totalBytes);
                    }
                    return File(buffer, "application/pdf", $"form-{formId}.pdf");
                }
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
        }
    }
}

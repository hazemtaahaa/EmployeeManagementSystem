using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.BL;
using EmployeeManagement.DAL;


namespace EmployeeManagement.API.Controllers
{
    [Route("api/admin")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAttendanceService _attendanceService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(
            IEmployeeService employeeService,
            IAttendanceService attendanceService,
            IUnitOfWork unitOfWork,
            UserManager<IdentityUser> userManager)
        {
            _employeeService = employeeService;
            _attendanceService = attendanceService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

     

        [HttpPut("employees/{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, [FromBody] UpdateEmployeeModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (string.IsNullOrEmpty(model.FirstName) || string.IsNullOrEmpty(model.LastName) ||
                string.IsNullOrEmpty(model.PhoneNumber) || string.IsNullOrEmpty(model.NationalId) ||
                model.Age < 18 || model.Age > 100)
            {
                return BadRequest("Invalid employee details. Ensure all fields are provided and Age is 18–100.");
            }

            var employeeDto = new UpdateEmployeeDto
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                NationalId = model.NationalId,
                Age = model.Age,
                SignatureUrl = model.SignatureUrl
            };

            try
            {
                var updatedEmployee = await _employeeService.UpdateEmployeeAsync(id, employeeDto);
                return Ok(updatedEmployee);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating employee: {ex.Message}");
            }
        }

        [HttpDelete("employees/{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            var employee = await _unitOfWork.Employees.GetByIdAsync(id);
            if (employee == null)
                return NotFound($"Employee with ID {id} not found.");

            var user = await _userManager.FindByIdAsync(employee.UserId);
            if (user == null)
                return NotFound($"Associated user not found for employee ID {id}.");

            try
            {
                await _employeeService.DeleteEmployeeAsync(id);
                var identityResult = await _userManager.DeleteAsync(user);
                if (!identityResult.Succeeded)
                {
                    return StatusCode(500, $"Failed to delete associated user: {string.Join(", ", identityResult.Errors.Select(e => e.Description))}");
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error deleting employee: {ex.Message}");
            }

            return Ok("Employee deleted successfully.");
        }

        [HttpGet("employees")]
        public async Task<IActionResult> GetEmployees(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string sortBy = "name",
            [FromQuery] string filter = "")
        {
            if (page < 1 || pageSize < 1)
                return BadRequest("Invalid page or pageSize.");

            try
            {
                var employees = await _employeeService.GetEmployeesAsync(page, pageSize, sortBy, filter);
                var totalCount = employees.Count();
                var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

                var employeeDtos = employees.Select(e => new EmployeeDto
                {
                    Id = e.Id,
                    FirstName = e.FirstName,
                    LastName = e.LastName,
                    PhoneNumber = e.PhoneNumber,
                    NationalId = e.NationalId,
                    Age = e.Age,
                    SignatureUrl = e.SignatureUrl
                }).ToList();

                return Ok(new
                {
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                    CurrentPage = page,
                    PageSize = pageSize,
                    Employees = employeeDtos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving employees: {ex.Message}");
            }
        }

        [HttpGet("attendance/daily")]
        public async Task<IActionResult> GetDailyAttendance([FromQuery] DateTime date)
        {
            try
            {
                var attendances = await _attendanceService.GetDailyAttendanceAsync(date);
                return Ok(attendances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving daily attendance: {ex.Message}");
            }
        }

        [HttpGet("attendance/weekly/{employeeId}")]
        public async Task<IActionResult> GetWeeklyAttendanceSummary(int employeeId, [FromQuery] DateTime startDate)
        {
            try
            {
                var summary = await _attendanceService.GetWeeklyAttendanceSummaryAsync(employeeId, startDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving weekly summary: {ex.Message}");
            }
        }
    }

   

   

   
}
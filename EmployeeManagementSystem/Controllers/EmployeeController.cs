using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using EmployeeManagement.BL;
using EmployeeManagement.DAL;

using System.Security.Claims;

namespace EmployeeManagement.API.Controllers
{
    [Route("api/employee")]
    [ApiController]
    [Authorize(Roles = "Employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAttendanceService _attendanceService;
        private readonly IUnitOfWork _unitOfWork;

        public EmployeeController(
            IEmployeeService employeeService,
            IAttendanceService attendanceService,
            IUnitOfWork unitOfWork)
        {
            _employeeService = employeeService;
            _attendanceService = attendanceService;
            _unitOfWork = unitOfWork;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user.");

                var employee = (await _unitOfWork.Employees.GetAllAsync())
                    .FirstOrDefault(e => e.UserId == userId);
                if (employee == null)
                    return NotFound("Employee not found.");

                var employeeDto = await _employeeService.GetEmployeeByIdAsync(employee.Id);
                return Ok(employeeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving profile: {ex.Message}");
            }
        }

        [HttpPost("checkin")]
        public async Task<IActionResult> CheckIn()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user.");

                var employee = (await _unitOfWork.Employees.GetAllAsync())
                    .FirstOrDefault(e => e.UserId == userId);
                if (employee == null)
                    return NotFound("Employee not found.");

                var result = await _attendanceService.CheckInAsync(employee.Id, DateTime.Now);
                if (!result.Success)
                    return BadRequest(result.Message);

                return Ok(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error checking in: {ex.Message}");
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetAttendanceHistory()
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user.");

                var employee = (await _unitOfWork.Employees.GetAllAsync())
                    .FirstOrDefault(e => e.UserId == userId);
                if (employee == null)
                    return NotFound("Employee not found.");

                var attendances = await _attendanceService.GetAttendanceHistoryAsync(employee.Id);
                return Ok(attendances);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving history: {ex.Message}");
            }
        }

        [HttpGet("weekly-summary")]
        public async Task<IActionResult> GetWeeklyAttendanceSummary([FromQuery] DateTime startDate)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user.");

                var employee = (await _unitOfWork.Employees.GetAllAsync())
                    .FirstOrDefault(e => e.UserId == userId);
                if (employee == null)
                    return NotFound("Employee not found.");

                var summary = await _attendanceService.GetWeeklyAttendanceSummaryAsync(employee.Id, startDate);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving weekly summary: {ex.Message}");
            }
        }

        [HttpPut("signature")]
        public async Task<IActionResult> AddOrUpdateSignature([FromBody] UpdateSignatureModel model)
        {
            try
            {
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized("Invalid user.");

                var employee = (await _unitOfWork.Employees.GetAllAsync())
                    .FirstOrDefault(e => e.UserId == userId);
                if (employee == null)
                    return NotFound("Employee not found.");

                if (!string.IsNullOrEmpty(employee.SignatureUrl))
                    return BadRequest("Signature already exists.");

                var success = await _employeeService.AddOrUpdateSignatureAsync(employee.Id, model.SignatureUrl);
                if (!success)
                    return StatusCode(500, "Failed to update signature.");

                return Ok("Signature updated successfully.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error updating signature: {ex.Message}");
            }
        }
    }


 
}

namespace EmployeeManagement.BL;

public interface IAttendanceService
{
    // Check-in by employee (only once per day, within allowed time)
    Task<CheckInResult> CheckInAsync(int employeeId, DateTime checkInTime);

    // Get daily attendance list for Admin
    Task<IEnumerable<AttendanceDto>> GetDailyAttendanceAsync(DateTime date);

    // Get attendance history for an employee
    Task<IEnumerable<AttendanceDto>> GetAttendanceHistoryAsync(int employeeId);

    // Get weekly attendance summary for employee
    Task<WeeklyAttendanceSummaryDto> GetWeeklyAttendanceSummaryAsync(int employeeId, DateTime weekStartDate);

}

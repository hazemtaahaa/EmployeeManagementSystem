
using EmployeeManagement.DAL;

namespace EmployeeManagement.BL;

public class AttendanceService : IAttendanceService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TimeSpan CheckInStartTime = new TimeSpan(7, 30, 0); // 7:30 AM
    private readonly TimeSpan CheckInEndTime = new TimeSpan(9, 0, 0);    // 9:00 AM

    public AttendanceService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    
    public async Task<CheckInResult> CheckInAsync(int employeeId, DateTime checkInTime)
    {
        var localTime = checkInTime;// checkInTime.ToLocalTime();
        var checkInTimeOfDay = localTime.TimeOfDay;

        if (checkInTimeOfDay < CheckInStartTime || checkInTimeOfDay > CheckInEndTime)
        {
            return new CheckInResult
            {
                Success = false,
                Message = "Check-in allowed only between 7:30 AM and 9:00 AM."
            };
        }
        var today = localTime.Date;

        // Check if already checked in today
        var hasCheckedIn = await _unitOfWork.Attendances.HasCheckedInTodayAsync(employeeId);
        if (hasCheckedIn)
        {
            return new CheckInResult
            {
                Success = false,
                Message = "Already checked in today."
            };
        }
        // Create new attendance record
        var attendance = new Attendance
        {
            EmployeeId = employeeId,
            CheckInTime = localTime,     
        };
        await _unitOfWork.Attendances.AddAsync(attendance);
        await _unitOfWork.CompleteAsync();
        return new CheckInResult
        {
            Success = true,
            Message = "Check-in successful."
        };
    }

    public async Task<IEnumerable<AttendanceDto>> GetAttendanceHistoryAsync(int employeeId)
    {
        var attendances = await _unitOfWork.Attendances.GetAttendancesByUserIdAsync(employeeId);
        
        return attendances.Select(a => new AttendanceDto
        {
            CheckInTime = a.CheckInTime,    
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee.FirstName +" "+a.Employee.LastName 
        });
    }

    public async Task<IEnumerable<AttendanceDto>> GetDailyAttendanceAsync(DateTime date)
    {

        var attendances = await _unitOfWork.Attendances
               .FindAsync(a => a.CheckInTime.Date == date.Date);

        return attendances.Select(a => new AttendanceDto
        {
            CheckInTime = a.CheckInTime,
            EmployeeId = a.EmployeeId,
            EmployeeName = a.Employee.FirstName + " " + a.Employee.LastName
        });
    }

   

    public async Task<WeeklyAttendanceSummaryDto> GetWeeklyAttendanceSummaryAsync(int employeeId, DateTime weekStartDate)
    {
        var weekEndDate = weekStartDate.AddDays(6);
        var attendances = await _unitOfWork.Attendances
            .FindAsync(a => a.EmployeeId == employeeId 
            && a.CheckInTime.Date >= weekStartDate
            && a.CheckInTime.Date <= weekEndDate);
        int expectedWorkDays = 5;

        int daysCheckedIn =attendances
                .Select(a => a.CheckInTime.Date)
                .Distinct()
                .Count();

        int missedDays = expectedWorkDays - daysCheckedIn;
        if (missedDays < 0) missedDays = 0;

        // Calculate total working hours for the week
        double attendancePercentage = expectedWorkDays > 0
               ? ((double)daysCheckedIn / expectedWorkDays) * 100
               : 0;

        return new WeeklyAttendanceSummaryDto
        {
            EmployeeId = employeeId,
            WeekStartDate = weekStartDate,
            DaysCheckedIn = daysCheckedIn,
            ExpectedWorkDays = expectedWorkDays,
            MissedDays = missedDays
        };

    }
}

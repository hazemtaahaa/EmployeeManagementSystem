namespace EmployeeManagement.BL;

public class AttendanceDto
{
    public DateTime CheckInTime { get; set; }
    public int EmployeeId { get; set; }
    public string EmployeeName { get; set; }
}

public class WeeklyAttendanceSummaryDto
{
    public int EmployeeId { get; set; }
    public DateTime WeekStartDate { get; set; }
    public int DaysCheckedIn { get; set; }

    public int ExpectedWorkDays { get; set; } //  5 

    public int MissedDays { get; set; } // ExpectedWorkDays - DaysCheckedIn

    public double AttendancePercentage => ExpectedWorkDays > 0 ? ((double)DaysCheckedIn / ExpectedWorkDays) * 100 : 0;


    public string AttendanceStatus
    {
        get
        {
            if (AttendancePercentage >= 90) return "Excellent";
            if (AttendancePercentage >= 75) return "Good";
            if (AttendancePercentage >= 50) return "Fair";
            return "Poor";
        }
    }
}

public class CheckInResult
{
    public bool Success { get; set; }
    public string Message { get; set; }  // "Already checked in today" or error messages
}
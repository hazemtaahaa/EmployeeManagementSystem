namespace EmployeeManagement.DAL;

public interface IAttendanceRepository : IGenericRepository<Attendance>
{
    Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int employeeId);

    Task<IEnumerable<Attendance>> GetDailyAttendanceAsync(DateTime date);


    Task<bool> HasCheckedInTodayAsync(int employeeId);

   // Task<Dictionary<int, double>> GetWeeklyHoursAsync();

}

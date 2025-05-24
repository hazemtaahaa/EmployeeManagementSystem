
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DAL;

public class AttendanceRepository : GenericRepository<Attendance>, IAttendanceRepository
{
    private readonly ApplicationDbContext _context;
    public AttendanceRepository(ApplicationDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Attendance>> GetAttendancesByUserIdAsync(int employeeId)
    {

        return await _context.Attendances
                  .Where(a => a.Id == employeeId)
                  .OrderByDescending(a => a.CheckInTime)
                  .ToListAsync();
    }


    public async Task<bool> HasCheckedInTodayAsync(int employeeId)
    {

        var today = DateTime.Today;
        return await _context.Attendances
            .AnyAsync(a => a.Id == employeeId && a.CheckInTime.Date == today.Date);
    }

   
    public async Task<IEnumerable<Attendance>> GetDailyAttendanceAsync(DateTime date)
    {
       var today = date.Date;
        return await _context.Attendances
            .Where(a => a.CheckInTime.Date == today)
            .Include(a => a.Employee)
            .ToListAsync();
    }

   

    //public async Task<Dictionary<int, double>> GetWeeklyHoursAsync()
    //{
    //    var startOfWeek = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);

    //    return await _context.Attendances
    //        .Where(a => a.CheckInTime >= startOfWeek)
    //        .GroupBy(a => a.EmployeeId)
    //        .Select(g => new { EmployeeId = g.Key, Count = g.Count() })
    //        .ToDictionaryAsync(g => g.EmployeeId, g => g.Count * 8.0); // Assume 8 hours per check-in
    //}
}

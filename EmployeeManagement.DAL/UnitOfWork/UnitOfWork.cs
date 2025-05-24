namespace EmployeeManagement.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    public IEmployeeRepository Employees { get; private set; }
    public IAttendanceRepository Attendances { get; private set; }

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Employees = new EmployeeRepository(context);
        Attendances = new AttendanceRepository(context);
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

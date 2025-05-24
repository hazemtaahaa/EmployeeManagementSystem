namespace EmployeeManagement.DAL;

public interface IUnitOfWork : IDisposable
{
    IEmployeeRepository Employees { get; }
    IAttendanceRepository Attendances { get; }

    Task<int> CompleteAsync();
}

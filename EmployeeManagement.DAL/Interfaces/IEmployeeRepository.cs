namespace EmployeeManagement.DAL;

public interface IEmployeeRepository : IGenericRepository<Employee>
{

     Task<IEnumerable<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy, string filter);
   // Task<IQueryable<Employee>> Query();
}

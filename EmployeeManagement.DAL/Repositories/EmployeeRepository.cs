using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.DAL;

public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
{
    public EmployeeRepository(ApplicationDbContext context) : base(context)
    {
    }

    public Task<IQueryable<Employee>> Query()
    {
        throw new NotImplementedException();
    }


    public async Task<IEnumerable<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy, string filter)
    {
        var query = _context.Employees.AsQueryable();
        if (!string.IsNullOrEmpty(filter))
            query = query.Where(e => e.FirstName.Contains(filter) || e.LastName.Contains(filter));

        if (sortBy == "name")
            query = query.OrderBy(e => e.FirstName);
        else if (sortBy == "age")
            query = query.OrderBy(e => e.Age);

        return await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
    }
}



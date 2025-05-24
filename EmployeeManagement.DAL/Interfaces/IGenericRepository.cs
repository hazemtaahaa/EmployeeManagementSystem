using System.Linq.Expressions;

namespace EmployeeManagement.DAL;

public interface IGenericRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id);
    Task<IEnumerable<T>> GetAllAsync();
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(int id);

    Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate);
}

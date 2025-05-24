using EmployeeManagement.DAL;

namespace EmployeeManagement.BL;

public interface IEmployeeService
{
    Task<bool> AddEmployeeAsync(CreateEmployeeDto employeeDto);
    Task<EmployeeDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto);
    Task<bool> DeleteEmployeeAsync(int employeeId);

    // Get employees list with pagination, sorting, filtering
    Task<IEnumerable<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy, string filter);
    

        // Employee profile view (for employee role)
        Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId);

    // Signature management
    Task<bool> AddOrUpdateSignatureAsync(int employeeId, string? signatureUrl);
    Task<string?> GetSignatureAsync(int employeeId);
}

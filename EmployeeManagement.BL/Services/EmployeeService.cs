

using EmployeeManagement.DAL;
using Microsoft.AspNetCore.Identity;

namespace EmployeeManagement.BL;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public EmployeeService(IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
        _roleManager = roleManager;
    }
    public async Task<bool> AddEmployeeAsync(CreateEmployeeDto employeeDto)
    {
        
        var employee = new Employee
        {
            FirstName = employeeDto.FirstName,
            LastName = employeeDto.LastName,
            PhoneNumber = employeeDto.PhoneNumber,
            NationalId = employeeDto.NationalId,
            Age = employeeDto.Age??0,
            SignatureUrl = employeeDto.SignatureUrl
           
        };
        await _unitOfWork.Employees.AddAsync(employee);
        await _unitOfWork.CompleteAsync();
        return true;

    }

    public async Task<bool> AddOrUpdateSignatureAsync(int employeeId, string? signatureUrl)
    {
          var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
        {
            return false;
        }
        employee.SignatureUrl = signatureUrl;
        await _unitOfWork.Employees.UpdateAsync(employee);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<bool> DeleteEmployeeAsync(int employeeId)
    {
        await _unitOfWork.Employees.DeleteAsync(employeeId);
        await _unitOfWork.CompleteAsync();
        return true;
    }

    public async Task<EmployeeDto> GetEmployeeByIdAsync(int employeeId)
    {
        var employee =await  _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new Exception("Employee not found");
        }
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            PhoneNumber = employee.PhoneNumber,
            NationalId = employee.NationalId,
            Age = employee.Age,
            SignatureUrl = employee.SignatureUrl
        };
    }


    public async Task<string?> GetSignatureAsync(int employeeId)
    {
      
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
        {
            return null;
        }
        return employee.SignatureUrl;
    }

    public async Task<EmployeeDto> UpdateEmployeeAsync(int employeeId, UpdateEmployeeDto employeeDto)
    {
        var employee = await _unitOfWork.Employees.GetByIdAsync(employeeId);
        if (employee == null)
        {
            throw new Exception("Employee not found");
        }

       
        employee.FirstName = employeeDto.FirstName;
        employee.LastName = employeeDto.LastName;
        employee.PhoneNumber = employeeDto.PhoneNumber;
        employee.NationalId = employeeDto.NationalId;
        employee.Age = employeeDto.Age??0;
        employee.SignatureUrl = employeeDto.SignatureUrl;
        
        await _unitOfWork.Employees.UpdateAsync(employee);
        await _unitOfWork.CompleteAsync();
        return new EmployeeDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            PhoneNumber = employee.PhoneNumber,
            NationalId = employee.NationalId,
            Age = employee.Age,
            SignatureUrl = employee.SignatureUrl
        };
    }

    public async Task<IEnumerable<Employee>> GetEmployeesAsync(int page, int pageSize, string sortBy, string filter)
    {
        return await _unitOfWork.Employees.GetEmployeesAsync(page, pageSize, sortBy, filter);
    }

}

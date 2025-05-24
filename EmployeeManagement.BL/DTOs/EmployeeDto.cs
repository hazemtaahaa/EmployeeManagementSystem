namespace EmployeeManagement.BL;

public class EmployeeDto
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalId { get; set; }
    public int Age { get; set; }
    public string? SignatureUrl { get; set; }
}

public class CreateEmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalId { get; set; }
    public int? Age { get; set; }
    public string? SignatureUrl { get; set; }
}

public class UpdateEmployeeDto : CreateEmployeeDto
{
    
}


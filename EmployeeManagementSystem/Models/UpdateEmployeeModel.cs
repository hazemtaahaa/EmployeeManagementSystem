namespace EmployeeManagement.API;

public class UpdateEmployeeModel
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string PhoneNumber { get; set; }
    public string NationalId { get; set; }
    public string? SignatureUrl { get; set; }
    public int Age { get; set; }
}
public class CreateEmployeeModel :UpdateEmployeeModel
{
   
}
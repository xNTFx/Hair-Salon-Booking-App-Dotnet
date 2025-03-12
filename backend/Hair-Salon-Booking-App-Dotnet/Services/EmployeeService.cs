using System.Collections.Generic;
using System.Threading.Tasks;

public class EmployeeService : IEmployeeService
{
    private readonly IEmployeeRepository _employeeRepository;

    public EmployeeService(IEmployeeRepository employeeRepository)
    {
        _employeeRepository = employeeRepository;
    }

    public async Task<IEnumerable<Employee>> GetAllEmployees()
    {
        return await _employeeRepository.GetAllEmployees();
    }
}

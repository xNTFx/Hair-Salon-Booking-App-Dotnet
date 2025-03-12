using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeService
{
    // retrieves all employees
    Task<IEnumerable<Employee>> GetAllEmployees();
}

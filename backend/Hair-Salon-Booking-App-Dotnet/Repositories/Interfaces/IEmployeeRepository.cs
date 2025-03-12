using System.Collections.Generic;
using System.Threading.Tasks;

public interface IEmployeeRepository
{
    Task<IEnumerable<Employee>> GetAllEmployees();
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServicesService
{
    // retrieves all available services
    Task<IEnumerable<Service>> GetAllServices();
}

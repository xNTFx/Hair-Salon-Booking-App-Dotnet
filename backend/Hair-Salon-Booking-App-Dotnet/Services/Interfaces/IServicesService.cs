using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServicesService
{
    Task<IEnumerable<Service>> GetAllServices();
}

using System.Collections.Generic;
using System.Threading.Tasks;

public interface IServicesRepository
{
    Task<IEnumerable<Service>> GetAllServices();
}

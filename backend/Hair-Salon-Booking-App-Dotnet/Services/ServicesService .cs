using System.Collections.Generic;
using System.Threading.Tasks;

public class ServicesService : IServicesService
{
    private readonly IServicesRepository _servicesRepository;

    public ServicesService(IServicesRepository servicesRepository)
    {
        _servicesRepository = servicesRepository;
    }

    public async Task<IEnumerable<Service>> GetAllServices()
    {
        return await _servicesRepository.GetAllServices();
    }
}

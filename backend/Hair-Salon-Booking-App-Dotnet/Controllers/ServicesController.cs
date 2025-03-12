using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("services")]
public class ServicesController : ControllerBase
{
    private readonly IServicesService _servicesService;

    public ServicesController(IServicesService servicesService)
    {
        _servicesService = servicesService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Service>>> GetAllServices()
    {
        var services = await _servicesService.GetAllServices();
        return Ok(services);
    }
}

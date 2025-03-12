using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ServicesRepository : IServicesRepository
{
    private readonly AppDbContext _context;

    public ServicesRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Service>> GetAllServices()
    {
        return await _context.Services.ToListAsync();
    }
}

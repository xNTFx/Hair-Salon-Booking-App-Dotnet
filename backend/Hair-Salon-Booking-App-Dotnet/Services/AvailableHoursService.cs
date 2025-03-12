using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class AvailableHoursService : IAvailableHoursService
{
    private readonly IAvailableHoursRepository _availableHoursRepository;

    public AvailableHoursService(IAvailableHoursRepository availableHoursRepository)
    {
        _availableHoursRepository = availableHoursRepository;
    }

    public async Task<IEnumerable<AvailableHours>> GetAllEmployees()
    {
        return await _availableHoursRepository.GetAllEmployees();
    }

    public async Task<IEnumerable<AvailableHours>> GetAvailableHoursByEmployeeId(int employeeId, DateTime reservationDate, string duration)
    {
        return await _availableHoursRepository.GetAvailableHoursByEmployeeId(employeeId, reservationDate, duration);
    }
}

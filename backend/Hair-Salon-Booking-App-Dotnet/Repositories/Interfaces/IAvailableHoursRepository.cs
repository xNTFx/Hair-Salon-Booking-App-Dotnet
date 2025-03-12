using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAvailableHoursRepository
{
    Task<IEnumerable<AvailableHours>> GetAllEmployees();
    Task<IEnumerable<AvailableHours>> GetAvailableHoursByEmployeeId(int employeeId, DateTime reservationDate, string duration);
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAvailableHoursService
{
    Task<IEnumerable<AvailableHours>> GetAllEmployees();
    Task<IEnumerable<AvailableHours>> GetAvailableHoursByEmployeeId(int employeeId, DateTime reservationDate, string duration);
}

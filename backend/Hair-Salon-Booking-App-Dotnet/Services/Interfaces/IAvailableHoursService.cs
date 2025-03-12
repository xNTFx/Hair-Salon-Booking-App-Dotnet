using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IAvailableHoursService
{
    // retrieves all available hours for all employees
    Task<IEnumerable<AvailableHours>> GetAllEmployees();

    // retrieves available hours for a specific employee on a given date and duration
    Task<IEnumerable<AvailableHours>> GetAvailableHoursByEmployeeId(int employeeId, DateTime reservationDate, string duration);
}

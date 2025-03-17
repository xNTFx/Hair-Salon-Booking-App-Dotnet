using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public class AvailableHoursRepository : IAvailableHoursRepository
{
    private readonly AppDbContext _context;

    public AvailableHoursRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<AvailableHours>> GetAllEmployees()
    {
        return await _context.AvailableHours.ToListAsync();
    }

    public async Task<IEnumerable<AvailableHours>> GetAvailableHoursByEmployeeId(int employeeId, DateTime reservationDate, string duration)
    {
        if (_context.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory")
        {
            return await _context.AvailableHours
                .Where(ah =>
                    (employeeId == 0 || ah.EmployeeId == employeeId) &&
                    reservationDate >= DateTime.UtcNow.Date)
                .ToListAsync();
        }

        return await _context.AvailableHours
            .FromSqlRaw(@"
                SELECT DISTINCT ON (ah.start_time, ah.end_time) ah.id, ah.start_time, ah.end_time, ah.employee_id
                FROM available_hours ah 
                LEFT JOIN reservations r 
                ON r.employee_id = ah.employee_id 
                AND (r.start_time + CAST({0} AS INTERVAL) > ah.end_time) 
                AND r.reservation_date = {1} 
                WHERE r.start_time IS NULL 
                AND ({2} = 0 OR ah.employee_id = {2}) 
                AND ({1} > CURRENT_DATE 
                    OR (ah.start_time > (CURRENT_TIME + INTERVAL '1 hour') AND {1} = CURRENT_DATE)) 
                ORDER BY ah.start_time, ah.end_time, ah.employee_id ASC",
                duration, reservationDate, employeeId)
            .ToListAsync();
    }
}

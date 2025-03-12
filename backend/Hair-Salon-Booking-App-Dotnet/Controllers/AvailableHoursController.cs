﻿using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("available_hours")]
public class AvailableHoursController : ControllerBase
{
    private readonly IAvailableHoursService _availableHoursService;

    public AvailableHoursController(IAvailableHoursService availableHoursService)
    {
        _availableHoursService = availableHoursService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AvailableHours>>> GetAllEmployees()
    {
        var availableHours = await _availableHoursService.GetAllEmployees();
        return Ok(availableHours);
    }

    [HttpGet("employee/{employeeId}/reservation_date/{reservationDate}/duration/{duration}")]
    public async Task<ActionResult<IEnumerable<AvailableHours>>> GetAvailableHoursByEmployeeId(
        int employeeId, DateTime reservationDate, string duration)
    {
        var availableHours = await _availableHoursService.GetAvailableHoursByEmployeeId(employeeId, reservationDate.ToUniversalTime(), duration);
        return Ok(availableHours);
    }

}

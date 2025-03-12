using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System;
using System.Threading.Tasks;

[ApiController]
[Route("reservations")]
public class ReservationsController : ControllerBase
{
    private readonly IReservationsService _reservationsService;
    private readonly JwtUtil _jwtUtil;

    public ReservationsController(IReservationsService reservationsService, JwtUtil jwtUtil)
    {
        _reservationsService = reservationsService;
        _jwtUtil = jwtUtil;
    }

    // retrieves all reservations
    [HttpGet]
    public async Task<ActionResult<List<Reservation>>> GetAllReservations()
    {
        var reservations = await _reservationsService.GetAllReservationsAsync();
        return Ok(reservations);
    }

    // retrieves all active reservations for the authenticated user
    [HttpGet("active")]
    public async Task<ActionResult<List<Reservation>>> GetAllActiveReservations([FromHeader(Name = "Authorization")] string token)
    {
        string userId = GetUserIdFromToken(token);
        var reservations = await _reservationsService.GetAllActiveReservationsAsync(userId);
        return Ok(reservations);
    }

    // retrieves the reservation history for the authenticated user
    [HttpGet("history")]
    public async Task<ActionResult<List<Reservation>>> GetAllHistoryReservations([FromHeader(Name = "Authorization")] string token)
    {
        string userId = GetUserIdFromToken(token);
        var reservations = await _reservationsService.GetAllHistoryReservationsAsync(userId);
        return Ok(reservations);
    }

    // cancels a reservation if the authenticated user is the owner
    [HttpPut("cancel/{id}")]
    public async Task<ActionResult<Reservation>> CancelReservation([FromHeader(Name = "Authorization")] string token, int id)
    {
        string userId = GetUserIdFromToken(token);
        var reservation = await _reservationsService.GetReservationByIdAsync(id);
        if (reservation == null)
        {
            return NotFound(new { message = "Reservation not found" });
        }

        if (reservation.UserId != userId)
        {
            return BadRequest(new { message = "User is not authorized to cancel this reservation." });
        }

        var cancelledReservation = await _reservationsService.CancelReservationAsync(id);
        return Ok(cancelledReservation);
    }

    // creates a reservation for an authenticated user
    [HttpPost("createWithAuth")]
    public async Task<ActionResult<Reservation>> CreateReservationWithAuth(
       [FromBody] CreateReservationDto dto,
       [FromHeader(Name = "Authorization")] string token)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        string userId = GetUserIdFromToken(token);

        var utcReservationDate = DateTime.SpecifyKind(dto.ReservationDate, DateTimeKind.Utc);

        var reservation = new Reservation
        {
            UserId = userId,
            ReservationDate = utcReservationDate,
            StartTime = TimeSpan.Parse(dto.StartTime),
            EndTime = TimeSpan.Parse(dto.EndTime),
            ServiceId = dto.Service.Id,
            EmployeeId = dto.Employee.EmployeeId,
            Status = ReservationStatus.PENDING
        };

        var createdReservation = await _reservationsService.SaveReservationAsync(reservation);
        return Ok(createdReservation);
    }

    // creates a reservation for an anonymous user
    [HttpPost("createWithoutAuth")]
    public async Task<ActionResult<Reservation>> CreateReservationWithoutAuth([FromBody] CreateReservationDto dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var utcReservationDate = DateTime.SpecifyKind(dto.ReservationDate, DateTimeKind.Utc);

        var reservation = new Reservation
        {
            UserId = "0", // anonymous user
            ReservationDate = utcReservationDate,
            StartTime = TimeSpan.Parse(dto.StartTime),
            EndTime = TimeSpan.Parse(dto.EndTime),
            ServiceId = dto.Service.Id,
            EmployeeId = dto.Employee.EmployeeId,
            Status = ReservationStatus.PENDING
        };

        var createdReservation = await _reservationsService.SaveReservationAsync(reservation);
        return Ok(createdReservation);
    }

    // extracts user ID from the access token
    private string GetUserIdFromToken(string token)
    {
        if (string.IsNullOrEmpty(token) || !token.StartsWith("Bearer "))
        {
            throw new ArgumentException("Invalid token");
        }

        token = token.Substring("Bearer ".Length).Trim();
        ClaimsPrincipal claims;
        try
        {
            claims = _jwtUtil.ValidateAccessToken(token);
        }
        catch (Exception)
        {
            throw new ArgumentException("Invalid or expired token");
        }

        var idClaim = claims.FindFirst("id")?.Value;
        if (string.IsNullOrEmpty(idClaim))
        {
            throw new ArgumentException("Invalid token");
        }
        return idClaim;
    }
}

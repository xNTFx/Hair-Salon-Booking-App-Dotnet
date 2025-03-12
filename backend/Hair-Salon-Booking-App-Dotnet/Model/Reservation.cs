using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Text.Json.Serialization;

public enum ReservationStatus
{
    PENDING,
    CANCELLED,
    COMPLETED
}

[Table("reservations")]
public class Reservation
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [BindNever]
    [Column("user_id")]
    [JsonIgnore]
    public string UserId { get; set; }

    [Required]
    [Column("reservation_date")]
    [JsonPropertyName("reservationDate")]
    public DateTime ReservationDate { get; set; } = DateTime.UtcNow.Date;

    [Required]
    [Column("start_time")]
    [JsonPropertyName("startTime")]
    public TimeSpan StartTime { get; set; }

    [Required]
    [Column("end_time")]
    [JsonPropertyName("endTime")]
    public TimeSpan EndTime { get; set; }

    [BindNever]
    [Column("status")]
    [JsonIgnore]
    public ReservationStatus Status { get; set; } = ReservationStatus.PENDING;

    [Required]
    [Column("service_id")]
    [JsonPropertyName("serviceId")]
    public int ServiceId { get; set; }

    [ForeignKey("ServiceId")]
    [JsonPropertyName("service")]
    public Service Service { get; set; }

    [Required]
    [Column("employee_id")]
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    [ForeignKey("EmployeeId")]
    [JsonPropertyName("employee")]
    public Employee Employee { get; set; }
}

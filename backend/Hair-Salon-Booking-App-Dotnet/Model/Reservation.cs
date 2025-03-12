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

    // Używamy BindNever, aby nie oczekiwać, że klient prześle to pole,
    // ale nie blokujemy jego serializacji (możesz opcjonalnie dodać JsonIgnore, jeśli frontend nie potrzebuje userId)
    [BindNever]
    [Column("user_id")]
    [JsonIgnore] // Jeżeli frontend nie potrzebuje tego pola – w przeciwnym razie usuń JsonIgnore.
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

    // Podobnie status ustawiany jest na serwerze – nie oczekujemy go od klienta
    [BindNever]
    [Column("status")]
    [JsonIgnore] // Opcjonalnie, jeśli frontend nie potrzebuje statusu
    public ReservationStatus Status { get; set; } = ReservationStatus.PENDING;

    [Required]
    [Column("service_id")]
    [JsonPropertyName("serviceId")]
    public int ServiceId { get; set; }

    // Usuwamy JsonIgnore, aby obiekt Service był serializowany
    [ForeignKey("ServiceId")]
    [JsonPropertyName("service")]
    public Service Service { get; set; }

    [Required]
    [Column("employee_id")]
    [JsonPropertyName("employeeId")]
    public int EmployeeId { get; set; }

    // Usuwamy JsonIgnore, aby obiekt Employee był serializowany
    [ForeignKey("EmployeeId")]
    [JsonPropertyName("employee")]
    public Employee Employee { get; set; }
}

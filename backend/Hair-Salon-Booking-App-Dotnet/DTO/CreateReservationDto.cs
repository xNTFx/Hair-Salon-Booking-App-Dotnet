public class CreateReservationDto
{
    public DateTime ReservationDate { get; set; }
    public string StartTime { get; set; }
    public string EndTime { get; set; }
    public ServiceDto Service { get; set; }
    public EmployeeDto Employee { get; set; }
}

public class ServiceDto
{
    public int Id { get; set; }
}

public class EmployeeDto
{
    public int EmployeeId { get; set; }
}

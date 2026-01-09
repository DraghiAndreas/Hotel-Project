namespace P6_Hotel;

public record Reservation
{
    public int Id { get; init; }
    public string ClientUsername {get; init;}
    public int RoomId {get; init;}
    public DateTime StartDate {get; init;}
    public DateTime EndDate {get; init;}
    public bool IsCheckedIn {get; set;}
    public bool IsCompleted {get; set; }
}
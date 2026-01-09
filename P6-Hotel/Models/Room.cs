namespace P6_Hotel;

public class Room
{
    public int Id { get; init; }
    public RoomType Type { get; set; }
    public double Price { get; set; }
    public RoomStatus Status { get; set; }

    public Room(int id, RoomType type, double price, RoomStatus status)
    {
        Id = id;
        Type = type;
        Price = price;
        Status = status;
    }
}    
using Microsoft.Data.Sqlite;

namespace P6_Hotel;

public static class DatabaseHandler
{
    private static string DbPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        @"..\..\..\hotel.db"
    );

    private static string ConnectionString => $"Data Source={DbPath}";

    static DatabaseHandler()
    {
        EnsureDatabase();
    }

    private static void EnsureDatabase()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        CREATE TABLE IF NOT EXISTS Rooms (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Number INTEGER,
            Type TEXT,
            Price REAL,
            Available INTEGER
        );

        CREATE TABLE IF NOT EXISTS Customers (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            Name TEXT,
            Email TEXT
        );

        CREATE TABLE IF NOT EXISTS Reservations (
            Id INTEGER PRIMARY KEY AUTOINCREMENT,
            RoomId INTEGER,
            CustomerId INTEGER,
            StartDate TEXT,
            EndDate TEXT,
            FOREIGN KEY(RoomId) REFERENCES Rooms(Id),
            FOREIGN KEY(CustomerId) REFERENCES Customers(Id)
        );
        """;

        cmd.ExecuteNonQuery();
    }
    

    public static List<Room> LoadRooms()
    {
        var rooms = new List<Room>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT * FROM Rooms";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            rooms.Add(new Room
            {
                Id = reader.GetInt32(0),
                Number = reader.GetInt32(1),
                Type = reader.GetString(2),
                Price = reader.GetDouble(3),
                Available = reader.GetInt32(4) == 1
            });
        }

        return rooms;
    }

    public static void SaveRoom(Room room)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        INSERT INTO Rooms(Number, Type, Price, Available)
        VALUES (@n, @t, @p, @a)
        """;

        cmd.Parameters.AddWithValue("@n", room.Number);
        cmd.Parameters.AddWithValue("@t", room.Type);
        cmd.Parameters.AddWithValue("@p", room.Price);
        cmd.Parameters.AddWithValue("@a", room.Available ? 1 : 0);

        cmd.ExecuteNonQuery();
    }


    public static void AddReservation(int roomId, int customerId, string start, string end)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        INSERT INTO Reservations(RoomId, CustomerId, StartDate, EndDate)
        VALUES (@r, @c, @s, @e);

        UPDATE Rooms SET Available = 0 WHERE Id = @r;
        """;

        cmd.Parameters.AddWithValue("@r", roomId);
        cmd.Parameters.AddWithValue("@c", customerId);
        cmd.Parameters.AddWithValue("@s", start);
        cmd.Parameters.AddWithValue("@e", end);

        cmd.ExecuteNonQuery();
    }
}
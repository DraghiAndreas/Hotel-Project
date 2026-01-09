using Microsoft.Data.Sqlite;

namespace P6_Hotel;

public static class DatabaseHandler
{
    private static readonly string DbPath =
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\hotel.db");

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
            Id INTEGER PRIMARY KEY,
            Type INTEGER NOT NULL,
            Price REAL NOT NULL,
            Status INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Users (
            Username TEXT PRIMARY KEY,
            Password TEXT NOT NULL,
            Role INTEGER NOT NULL
        );

        CREATE TABLE IF NOT EXISTS Reservations (
            Id INTEGER PRIMARY KEY,
            ClientUsername TEXT NOT NULL,
            RoomId INTEGER NOT NULL,
            StartDate TEXT NOT NULL,
            EndDate TEXT NOT NULL,
            IsCheckedIn INTEGER NOT NULL,
            IsCompleted INTEGER NOT NULL,
            FOREIGN KEY(RoomId) REFERENCES Rooms(Id),
            FOREIGN KEY(ClientUsername) REFERENCES Users(Username)
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
        cmd.CommandText = "SELECT Id, Type, Price, Status FROM Rooms";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            rooms.Add(new Room(
                reader.GetInt32(0),
                (RoomType)reader.GetInt32(1),
                reader.GetDouble(2),
                (RoomStatus)reader.GetInt32(3)
            ));
        }

        return rooms;
    }

    public static void SaveRoom(Room room)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        INSERT OR REPLACE INTO Rooms (Id, Type, Price, Status)
        VALUES (@id, @type, @price, @status)
        """;

        cmd.Parameters.AddWithValue("@id", room.Id);
        cmd.Parameters.AddWithValue("@type", (int)room.Type);
        cmd.Parameters.AddWithValue("@price", room.Price);
        cmd.Parameters.AddWithValue("@status", (int)room.Status);

        cmd.ExecuteNonQuery();
    }

  

    public static List<AUser> LoadUsers()
    {
        var users = new List<AUser>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = "SELECT Username, Password, Role FROM Users";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            string username = reader.GetString(0);
            string password = reader.GetString(1);
            int role = reader.GetInt32(2);

            users.Add(role == 0
                ? new Admin(username, password)
                : new Client(username, password));
        }

        return users;
    }

    public static void SaveUser(AUser user)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        INSERT OR REPLACE INTO Users (Username, Password, Role)
        VALUES (@u, @p, @r)
        """;

        cmd.Parameters.AddWithValue("@u", user.Username);
        cmd.Parameters.AddWithValue("@p", user.Password);
        cmd.Parameters.AddWithValue("@r", user is Admin ? 0 : 1);

        cmd.ExecuteNonQuery();
    }

   

    public static List<Reservation> LoadReservations()
    {
        var list = new List<Reservation>();

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        SELECT Id, ClientUsername, RoomId, StartDate, EndDate, IsCheckedIn, IsCompleted
        FROM Reservations
        """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new Reservation
            {
                Id = reader.GetInt32(0),
                ClientUsername = reader.GetString(1),
                RoomId = reader.GetInt32(2),
                StartDate = DateTime.Parse(reader.GetString(3)),
                EndDate = DateTime.Parse(reader.GetString(4)),
                IsCheckedIn = reader.GetInt32(5) == 1,
                IsCompleted = reader.GetInt32(6) == 1
            });
        }

        return list;
    }

    public static void SaveReservation(Reservation res)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        var cmd = connection.CreateCommand();
        cmd.CommandText = """
        INSERT OR REPLACE INTO Reservations
        (Id, ClientUsername, RoomId, StartDate, EndDate, IsCheckedIn, IsCompleted)
        VALUES (@i,@u,@r,@s,@e,@c,@d)
        """;

        cmd.Parameters.AddWithValue("@i", res.Id);
        cmd.Parameters.AddWithValue("@u", res.ClientUsername);
        cmd.Parameters.AddWithValue("@r", res.RoomId);
        cmd.Parameters.AddWithValue("@s", res.StartDate.ToString("O"));
        cmd.Parameters.AddWithValue("@e", res.EndDate.ToString("O"));
        cmd.Parameters.AddWithValue("@c", res.IsCheckedIn ? 1 : 0);
        cmd.Parameters.AddWithValue("@d", res.IsCompleted ? 1 : 0);

        cmd.ExecuteNonQuery();
    }
}

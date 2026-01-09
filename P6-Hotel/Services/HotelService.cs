namespace P6_Hotel;
public class HotelService
{
    private List<Room> _rooms = new List<Room>();
    private List<Reservation> _reservations = new List<Reservation>();
    private List<AUser> _users = new List<AUser>();

    public HotelService()
    {
        FileHandler.EnsureFileExists("Rooms.json");
        FileHandler.EnsureFileExists("Users.json");
        FileHandler.EnsureFileExists("Reservations.json");
        
        _rooms = FileHandler.LoadFile<Room>("Rooms.json");
        _users = FileHandler.LoadFile<AUser>("Users.json");
        _reservations = FileHandler.LoadFile<Reservation>("Reservations.json");

        if (_users.Count == 0)
        {
            _users.Add(new Admin(Username: "admin", Password: "admin"));
            _users.Add(new Client(Username: "client", Password: "client"));
            FileHandler.SaveFile("Users.json", _users);
        }
    }

    public AUser Login(string username, string password)
    {
        return _users.FirstOrDefault(x => x.Username == username && x.Password == password);
    }

    public void CreateAccount(string username, string password)
    {
        if (_users.Any(x => x.Username == username))
        {
            Console.WriteLine("Error: Username already exists!");
        }
        
        _users.Add(new Client(username, password));
        FileHandler.SaveFile("Users.json", _users);
    }
    
    // ADMIN CMDS
    
    public void ViewAllRooms()
    {
        foreach (var r in _rooms)
        {
            Console.WriteLine($"Room ID: {r.Id} |  Room Type: {r.Type} | Room Status: {r.Status}");
        }
    }
    
    
    public void AddRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
    {
        if (_rooms.Any(x => x.Id == id))
        {
            Console.WriteLine("Error: Room ID already exists!");
            return;
        }
        
        _rooms.Add(new Room(id, roomType, price, roomStatus));
        FileHandler.SaveFile("Rooms.json", _rooms);
        Console.WriteLine($"Room {id} created!");
    }

    public void RemoveRoom(int id)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == id);
        if (room == null)
        {
            Console.WriteLine("Error: Room not found!");
            return;
        }
        _rooms.Remove(room);
        FileHandler.SaveFile("Rooms.json", _rooms);
        Console.WriteLine($"Room {id} has been removed!");
    }

    public void ModifyRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == id);
        if (room == null)
        {
            Console.WriteLine("Error: Room not found!");
            return;
        }
        room.Type = roomType;
        room.Price = price;
        room.Status = roomStatus;
        FileHandler.SaveFile("Rooms.json", _rooms);
    }
    
    public void ViewAllReservations()
    {
        Console.WriteLine("Viewing all reservations :");
        foreach (var r in _reservations)
        {
            Console.WriteLine($"Username : {r.ClientUsername} | Id : {r.Id}");
        }
    }
    
    // CLIENT CMDS

    public void SearchAvailableRooms(RoomType roomType)
    {
        List<Room> availableRooms = _rooms.Where(x => x.Type == roomType && x.Status == RoomStatus.Free ).ToList();
        foreach (Room room in availableRooms)
        {
            Console.WriteLine($"{room.Id} - {room.Type} - {room.Status}");
        }
    }
    
    public void MakeReservation(string username, int roomId, int nights)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == roomId);
        if (room == null)
        {
            Console.WriteLine("Error: Room not found!");
            return;
        }

        if (room.Status != RoomStatus.Free)
        {
            Console.WriteLine("Error: Room is not available!");
            return;
        }
        
        
        var newRes = new Reservation
        {
            Id = ReservationIdGenerator(),
            ClientUsername = username,
            RoomId = room.Id,
            StartDate = DateTime.Now,
            EndDate = DateTime.Now.AddDays(nights),
            IsCheckedIn = false,
            IsCompleted = false
        };
        
        _reservations.Add(newRes);
        FileHandler.SaveFile("Reservations.json", _reservations);

        Console.WriteLine($"Success! Reservation {newRes.Id} created!");
    }

    public void RemoveReservation(string username, int reservationId)
    {
        var reservation = _reservations.FirstOrDefault(x => x.Id == reservationId);
        if (reservation == null)
        {
            Console.WriteLine("Error: Reservation not found!");
            return;
        }

        if (reservation.IsCheckedIn ||  reservation.IsCompleted)
        {
            Console.WriteLine("Error: Reservation cannot be canceled after you have Checked-In!");
            return;
        }
        
        _reservations.Remove(reservation);
        _rooms.Where(x => x.Id == reservation.RoomId).ToList().ForEach(x => x.Status = RoomStatus.Free);
        FileHandler.SaveFile("Reservations.json", _reservations);
        FileHandler.SaveFile("Rooms.json", _rooms);
    }
    

    public void ViewReservation(string username)
    {
        List<Reservation> reservations = _reservations.Where(x => x.ClientUsername == username).ToList();
        if (reservations.Count == 0)
        {
            Console.WriteLine("Error: You have no reservations to view!");
            return;
        }

        foreach (Reservation res in reservations)
        {
            Console.WriteLine($"Reservation ID : {res.Id} | Room ID : {res.RoomId} | Checked-IN : {res.IsCheckedIn}");
        }
        
    }

    public void SelfCheckIn(int reservationId)
    {
        var res = _reservations.FirstOrDefault(x => x.Id == reservationId);
        if (res == null)
        {
            Console.WriteLine("Error: Reservation not found!");
            return;
        }

        if (res.IsCheckedIn)
        {
            Console.WriteLine("Error: Reservation is already checked!");
            return;
        }

        res.IsCheckedIn = true;

        var RoomIndex = _rooms.FindIndex(x => x.Id == res.RoomId);
        _rooms[RoomIndex].Status = RoomStatus.Occupied;
        FileHandler.SaveFile("Rooms.json", _rooms);
        FileHandler.SaveFile("Reservations.json", _reservations);
        Console.WriteLine("Check-in confirmed!");
    }
    
    // HELPER METHODS
    
    private int ReservationIdGenerator()
    {
        int rId;
        do
        {
            rId = new Random().Next(1000, 9999);
        }    while(_reservations.Any(x => x.Id == rId));
        return rId;
    }


}
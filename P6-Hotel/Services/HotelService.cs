using System.Runtime.InteropServices.JavaScript;
using Microsoft.Extensions.Logging;

namespace P6_Hotel;
public class HotelService
{
    
    private readonly ILogger<HotelService> _logger;
    
    private List<Room> _rooms = new List<Room>();
    private List<Reservation> _reservations = new List<Reservation>();
    private List<User> _users = new List<User>();
    private HotelConfig _config;
    public HotelService(ILogger<HotelService> logger)
    {
        _logger = logger;
        _logger.LogInformation("Hotel Service started");
        
        //FISIERE EXISTA SI LOAD-DATA
        
        FileHandler.EnsureFileExists("Rooms.json");
        FileHandler.EnsureFileExists("Users.json");
        FileHandler.EnsureFileExists("Reservations.json");
        FileHandler.EnsureFileExists("Config.json");

        _rooms = FileHandler.LoadFile<Room>("Rooms.json");
        _users = FileHandler.LoadFile<User>("Users.json");
        _reservations = FileHandler.LoadFile<Reservation>("Reservations.json");
        
        var configs = FileHandler.LoadFile<HotelConfig>("Config.json");
        _config = configs.FirstOrDefault() ?? new HotelConfig();
        if(!configs.Any()) FileHandler.SaveFile("Config.json", new List<HotelConfig>{_config});
        
        //CREEAREA ACCOUNT-URILOR DE BAZA (ADMIN, CLIENT)
        
        if (_users.Count == 0)
        {
            _logger.LogWarning("No users found! Creating default accounts.");
            _users.Add(new User(Username: "admin", Password: "admin",UserRole.Admin, Token: "hotel_tok_admin"));
            _users.Add(new User(Username: "client", Password: "client", UserRole.Client, Token: "hotel_tok_client"));
            FileHandler.SaveFile("Users.json", _users);
        }
    }
    

    public User Login(string username, string password)
    {
        User us = _users.FirstOrDefault(x => x.Username == username && x.Password == password);
        if (us == null)
        {
            throw new Exception("Username or password is incorrect!");
        }

        return us;
    }

    public User ForgotPassword(string token)
    {
        User us = _users.FirstOrDefault(x => x.Token == token);
        if (us == null)
        {
            throw new Exception("Incorrect token!");
        }

        return us;
    }

    public void CreateAccount(string username, string password)
    {
        if (_users.Any(x => x.Username == username))
        {
            throw new Exception("Username already exists");
        }


        string tok = UniqueTokenGenerator();
        _users.Add(new User(username, password, UserRole.Client, tok));
        FileHandler.SaveFile("Users.json", _users);
        _logger.LogInformation($"New account created: {username}.");
        _logger.LogInformation($"New account token: {tok}");
    }
    
    // ADMIN CMDS
    
    public void ViewAllRooms()
    {
        if (_rooms.Any())
        {
        foreach (var r in _rooms)
        {
            Console.WriteLine($"Room ID: {r.Id} |  Room Type: {r.Type} | Room Status: {r.Status} | Price per Night : {r.Price}$");
            if (_reservations.Any(x => x.RoomId == r.Id))
            {
                List<Reservation> reservations = _reservations.Where(x => x.RoomId == r.Id).ToList();
                int counter = 1;
                foreach (var res in reservations)
                {
                    Console.WriteLine($"#{counter++} Start-date : {res.StartDate} => End-date : {res.EndDate}");
                }
            }
        }
        }
        else
        {
            Console.WriteLine("\nNO EXISTING ROOMS!");
        }
    }
    
    
    public void AddRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
    {
        if (_rooms.Any(x => x.Id == id))
        {
            throw new Exception("Error: Room ID already exists!");
        }
        
        _rooms.Add(new Room(id, roomType, price, roomStatus));
        FileHandler.SaveFile("Rooms.json", _rooms);
        Console.WriteLine($"Room {id} created!");
        _logger.LogInformation($"Room {id} created!");
    }

    public void RemoveRoom(int id)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == id);
        if (room == null)
        {
            throw new Exception("Error: Room not found!");
        }

        _reservations.RemoveAll(x => x.RoomId == id);
        _rooms.Remove(room);
        FileHandler.SaveFile("Rooms.json", _rooms);
        FileHandler.SaveFile("Reservations.json", _reservations);
        Console.WriteLine($"Room {id} has been removed!");
        _logger.LogInformation($"Room {id} has been removed!");

    }

    public void ModifyRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == id);
        if (room == null)
        {
            throw new Exception("Error: Room not found!");
        }
        room.Type = roomType;
        room.Price = price;
        room.Status = roomStatus;
        FileHandler.SaveFile("Rooms.json", _rooms);
        _logger.LogInformation($"Room {id} has been modified!");

    }
    
    public void ViewAllReservations()
    {

        if (_reservations.Any())
        {
            Console.WriteLine("Viewing all reservations :");
            foreach (var r in _reservations)
            {
                Console.WriteLine($"Username : {r.ClientUsername} | Id : {r.Id}");
            }
        }
        else
        {
            Console.WriteLine("\nNO EXISTING RESERVATIONS!");
        }

    }
    
    // CLIENT CMDS

    public void SearchAvailableRooms(RoomType roomType, DateTime startDate,  DateTime endDate)
    {
        bool flag = false;
        
        List<Room> availableRooms = _rooms.Where(x => x.Type == roomType && x.Status == RoomStatus.Available).ToList();
        foreach (Room room in availableRooms)
        {
            var list = _reservations.Where(x => x.RoomId == room.Id &&
                                                        (startDate < x.EndDate && endDate > x.StartDate )
                                                        && !x.IsCompleted);
            if (!list.Any())
            {
                if (flag == false)
                {
                    flag = true;
                    Console.WriteLine("\n-------------\nAvailable Rooms: ");                    
                }
                Console.WriteLine($"{room.Id} - {room.Type}");
            }
        }
        if (flag == false)
        {
            Console.WriteLine("\nUnfortunately, there are no rooms with your prefrences!");
        }
    }
    
    
    public void MakeReservation(string username, int roomId, DateTime startDate, int nights)
    {
        var room = _rooms.FirstOrDefault(x => x.Id == roomId && x.Status == RoomStatus.Available);
        if (room == null)
        {
            throw new  Exception("Error: Room not found!");
        }

        if (nights < _config.MinBookingDays || nights > _config.MaxBookingDays)
        {
            throw new Exception($"Error: Nights must be between {_config.MinBookingDays} and {_config.MaxBookingDays} days!.");
        }
        
        DateTime endDate = startDate.AddDays(nights);
        bool isBooked = _reservations.Any(x => 
                        x.RoomId == roomId && 
                        !x.IsCompleted && 
                        (startDate < x.EndDate && endDate > x.StartDate));

        if (isBooked)
        {
            throw new Exception("Error: Reservation is already booked form the specified dates!");
        }
        
        var newRes = new Reservation
        {
            Id = ReservationIdGenerator(),
            ClientUsername = username,
            RoomId = room.Id,
            StartDate = startDate,
            EndDate = endDate,
            IsCheckedIn = false,
            IsCompleted = false
        };
        
        _reservations.Add(newRes);
        FileHandler.SaveFile("Reservations.json", _reservations);

        Console.WriteLine($"Success! Reservation {newRes.Id} created!");
        _logger.LogInformation($"Reservation {newRes.Id} has been created!");

    }

    public void RemoveReservation(string username, int reservationId)
    {
        var reservation = _reservations.FirstOrDefault(x => x.Id == reservationId && x.ClientUsername == username);
        if (reservation == null)
        {
            throw new Exception("Error: Reservation not found!");
        }

        if (reservation.IsCheckedIn ||  reservation.IsCompleted)
        {
            throw new Exception("Error: Reservation cannot be canceled after you have Checked-In!");
        }
        
        _reservations.Remove(reservation);
        _logger.LogInformation($"Reservation {reservationId} has been removed!");
        FileHandler.SaveFile("Reservations.json", _reservations);
        FileHandler.SaveFile("Rooms.json", _rooms);
    }
    

    public void ViewReservation(string username)
    {
        List<Reservation> reservations = _reservations.Where(x => x.ClientUsername == username).ToList();
        if (reservations.Count == 0)
        {
            Console.WriteLine("You have no reservations to view!");
            return;
        }

        foreach (Reservation res in reservations)
        {
            Console.WriteLine($"Reservation ID : {res.Id} | Room ID : {res.RoomId} | Start-date : {res.StartDate} | End-date {res.EndDate} | Checked-IN : {res.IsCheckedIn} | Is-Completed : {res.IsCompleted}");
        }
        
    }

    public void SelfCheckIn(int reservationId, string username)
    {
        var res = _reservations.FirstOrDefault(x => x.Id == reservationId && x.ClientUsername == username);
        if (res == null)
        {
            throw new Exception("Error: Reservation not found!");
        }

        if (res.IsCheckedIn)
        {
            throw new Exception("Error: Reservation is already checked in!");
        }

        res.IsCheckedIn = true;
        
        FileHandler.SaveFile("Reservations.json", _reservations);
        Console.WriteLine("Check-in confirmed!");
        _logger.LogInformation($"Check-in confirmed for reservation {reservationId}.");

    }
    
    public void SelfCheckOut(int reservationId, string username)
    {
        var res = _reservations.FirstOrDefault(x => x.Id == reservationId && x.ClientUsername == username);
        if (res == null)
        {
            throw new Exception("Error: Reservation not found!");
        }

        if (!res.IsCheckedIn)
        {
            throw new Exception("Error: You have not yet checked in!");
        }

        res.IsCompleted = true;


        FileHandler.SaveFile("Reservations.json", _reservations);
        Console.WriteLine("Check-out confirmed!");
        _logger.LogInformation($"Check-out confirmed for reservation {reservationId}.");

    }
    
    // HELPER METHODS

    #region Helpers
    
    private int ReservationIdGenerator()
    {
        int rId;
        do
        {
            rId = new Random().Next(1000, 9999);
        }    while(_reservations.Any(x => x.Id == rId));
        return rId;
    }

    public DateTime InputToDate(int day, int  month, int year)
    {
        DateTime new_date;
        try
        {
             new_date = new DateTime(year, month, day);
        }
        catch (Exception e)
        {
            throw new Exception("Date is invalid.");
        }
        
        if (new_date < DateTime.Today)
        {
            throw new Exception("Date cannot be in the past!");
        }
        return new_date;
    }

    public int GetMinBookingDays()
    {
        return _config.MinBookingDays;
    }

    public int GetMaxBookingDays()
    {
        return _config.MaxBookingDays;
    }

    public void SetMinBookingDays(int days)
    {
        if (days < 1) throw new Exception("Error: Min days must be greater 1.");
        if (days >= _config.MaxBookingDays) throw new Exception("Error: Max days must be less than " + _config.MinBookingDays);
        
        _config.MinBookingDays = days;
        FileHandler.SaveFile("Config.json", new List<HotelConfig> { _config });
        _logger.LogInformation($"Set min days to {days}.");
    }
    
    public void SetMaxBookingDays(int days)
    {
        if (days <= _config.MinBookingDays) throw new Exception("Error: Max days must be greater than Min days.");
        
        _config.MaxBookingDays = days;
        FileHandler.SaveFile("Config.json", new List<HotelConfig> { _config });
        _logger.LogInformation($"Set max days to {days}.");
    }

    public static string UniqueTokenGenerator()
    {
        const string chars = "qwertyuiopasdfghjklzxcvbnm1234567890!@#$%^&*.";
        Random rnd = new Random();

        string token = "hotel_tok_";
        for (int i = 0; i < 11; i++)
        {
            token += chars[rnd.Next(chars.Length)];
        }

        return token;
    }
}



#endregion

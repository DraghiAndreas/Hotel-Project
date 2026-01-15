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
            _users.Add(new User(Username: "admin", Password: "admin",UserRole.Admin));
            _users.Add(new User(Username: "client", Password: "client", UserRole.Client));
            FileHandler.SaveFile("Users.json", _users);
        }
    }

    public void Run()
    {
        _logger.LogInformation("Application interaction started.");
        User currentUser = null;
        int id,logInOpt;
        
        while (true)
        {
            if (currentUser == null)
            {
                Console.WriteLine("\n--------- PICK AN OPTION ---------");
                Console.WriteLine("1. Log-in (to an existing account)\n2.Create Acconut");
                Console.Write("Option: ");
                logInOpt = int.Parse(Console.ReadLine());
            }
            else
            {
                logInOpt = 1;
            }
            switch (logInOpt)
            {
                case 1:
                    if (currentUser == null)
                    {
                        Console.WriteLine("\n--------- WELCOME ---------");
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();

                        currentUser = Login(username, password);
                        if (currentUser == null)
                        {
                            Console.WriteLine("Invalid username or password!");
                            _logger.LogWarning($"Failed login attempt for user: {username}.");
                        }
                        else
                        {
                            Console.WriteLine("You successfully logged in!");
                            _logger.LogWarning($"User {username} logged in.");
                        }
                    }
                    else
                    {
                        if (currentUser.Role is UserRole.Admin)
                        {
                            Console.WriteLine("\n[ADMIN MENU]\n1. Room-Manager\n2. View Reservations\n3. General Rules\n0. Logout");
                            int input = int.Parse(Console.ReadLine());
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine(
                                        "\n[ROOM-MANAGER MENU]\n1.View ALL Rooms\n2. Add Room\n3. Modify Room\n4. Remove Room");
                                    int input2 = int.Parse(Console.ReadLine());
                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("---- ALL ROOMS ----");
                                            ViewAllRooms();
                                            break;

                                        case 2:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType = (RoomType)int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Price: ");
                                            double price = double.Parse(Console.ReadLine());
                                            
                                            try
                                            {
                                                AddRoom(id, roomType, price, RoomStatus.Available);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to add room.");
                                            }
                                            break;

                                        case 3:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType1 = (RoomType)int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Price: ");
                                            double price1 = double.Parse(Console.ReadLine());
                                            Console.WriteLine(
                                                "Room Status (0 = Available, 1 = Unavailable)");
                                            RoomStatus roomStatus1 = (RoomStatus)int.Parse(Console.ReadLine());
                                            try
                                            {
                                                ModifyRoom(id, roomType1, price1, roomStatus1);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to modify room.");
                                            }
                                            break;

                                        case 4:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                RemoveRoom(id);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to remove room.");                                             
                                            }
                                            break;
                                    }

                                    break;
                                case 2:
                                    ViewAllReservations();
                                    break;
                                case 3:
                                    Console.WriteLine("[GENERAL RULES]");
                                    Console.WriteLine("1. Set MIN booking days\n2. Set MAX booking days");
                                    Console.Write("Option");
                                    int input3 = int.Parse(Console.ReadLine());

                                    switch (input3)
                                    {
                                        case 1:
                                            Console.WriteLine($"Minimttum amount of days (current is {_config.MinBookingDays}) : ");
                                            int temp =  int.Parse(Console.ReadLine());
                                            if (temp > 0 && temp < _config.MaxBookingDays)
                                            {
                                                _config.MinBookingDays = temp;
                                                _logger.LogWarning($"MIN booking changed to {_config.MinBookingDays}.");
                                                FileHandler.SaveFile("Config.json", new List<HotelConfig>{_config});
                                            }
                                            else
                                            {
                                                _logger.LogWarning("Invalid MIN amount.");
                                            }
                                            break;
                                        case 2:
                                            Console.WriteLine($"Maximum amount of days (current is {_config.MaxBookingDays}) : ");
                                            int temp1 =  int.Parse(Console.ReadLine());
                                            if (temp1 > 0 && temp1 > _config.MinBookingDays)
                                            { 
                                                _config.MaxBookingDays = temp1;
                                                _logger.LogWarning($"MAX booking changed to {_config.MaxBookingDays}.");
                                                FileHandler.SaveFile("Config.json", new List<HotelConfig>{_config});
                                            }
                                            else
                                            {
                                                _logger.LogWarning("Invalid MAX amount.");
                                            }
                                            break;
                                    }
                                    break;
                                
                                case 0:
                                    currentUser = null;
                                    break;
                            }
                        }
                        else if (currentUser.Role is UserRole.Client)
                        {
                            Console.WriteLine("\n---- CLIENT MENU ----");
                            Console.WriteLine(
                                "1. Search Rooms\n2. Reservation Hub\n3. Check-in/out\n0. Logout");
                            int input = int.Parse(Console.ReadLine());
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                    RoomType roomType = (RoomType)int.Parse(Console.ReadLine());
                                    
                                    Console.WriteLine("Start Date :");
                                    Console.Write("Day: ");
                                    int day1 = int.Parse(Console.ReadLine());
                                    Console.Write("Month: ");
                                    int month1 = int.Parse(Console.ReadLine());
                                    Console.Write("Year: ");
                                    int  year1 = int.Parse(Console.ReadLine());

                                    DateTime startDate1;
                                    try
                                    {
                                        startDate1 = InputToDate(day1, month1, year1);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        break;
                                    }               
                                    
                                    Console.Write("Number of Nights: ");
                                    int nrNights = int.Parse(Console.ReadLine());
                                    SearchAvailableRooms(roomType, startDate1, startDate1.AddDays(nrNights));
                                    break;
                                case 2:
                                    
                                    Console.WriteLine("---- RESERVATION HUB ----");
                                    Console.WriteLine("1. Create Reservation\n2. View Reservations\n3. Delete Reservation");
                                    Console.Write("Option: ");
                                    
                                    int input2 = int.Parse(Console.ReadLine());

                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            
                                            Console.WriteLine("Start Date :");
                                            Console.Write("Day: ");
                                            int day = int.Parse(Console.ReadLine());
                                            Console.Write("Month: ");
                                            int month = int.Parse(Console.ReadLine());
                                            Console.Write("Year: ");
                                            int  year = int.Parse(Console.ReadLine());

                                            DateTime startDate;
                                            try
                                            {
                                                 startDate = InputToDate(day, month, year);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                break;
                                            }
                                            
                                            Console.WriteLine("Number of Nights: ");
                                            int nights = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                MakeReservation(currentUser.Username, id,startDate, nights);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to create reservation.");
                                            }
                                            break;
                                        
                                        case 2:
                                            ViewReservation(currentUser.Username);
                                            break;
                                        
                                        case 3:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                RemoveReservation(currentUser.Username, id);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to delete reservation.");
                                            }
                                            break;
                                    }

                                    break;

                                case 3:
                                    Console.WriteLine("---- CHECK-IN/OUT ---- ");
                                    Console.WriteLine("1. Check-in\n2. Check-out");
                                    Console.Write("Option: ");
                                    input2 = int.Parse(Console.ReadLine());
                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("Reservation ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                SelfCheckIn(id,currentUser.Username);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to check in.");
                                            }
                                            break;
                                        
                                        case 2:
                                            Console.WriteLine("Reservation ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                SelfCheckOut(id,currentUser.Username);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to check in.");
                                            }
                                            break;
                                    }
                                    
                                    break;
                                case 0:
                                    currentUser = null;
                                    break;
                            }
                        }
                    }
                    break;
                case 2:
                    Console.Write("Enter your NEW Username: ");
                    string newUsername = Console.ReadLine();
                    Console.Write("Enter your NEW Password: ");
                    string newPassword = Console.ReadLine();
                    try
                    {
                        CreateAccount(newUsername, newPassword);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        _logger.LogWarning("Failed to create account.");
                    }
                    break;
            }
        }
    }

    private User Login(string username, string password)
    {
        return _users.FirstOrDefault(x => x.Username == username && x.Password == password);
    }

    private void CreateAccount(string username, string password)
    {
        if (_users.Any(x => x.Username == username))
        {
            throw new Exception("Username already exists");
        }
        
        _users.Add(new User(username, password, UserRole.Client));
        FileHandler.SaveFile("Users.json", _users);
        _logger.LogInformation($"New account created: {username}.");
    }
    
    // ADMIN CMDS
    
    private void ViewAllRooms()
    {
        foreach (var r in _rooms)
        {
            Console.WriteLine($"Room ID: {r.Id} |  Room Type: {r.Type} | Room Status: {r.Status}");
            
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
    
    
    private void AddRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
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

    private void RemoveRoom(int id)
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

    private void ModifyRoom(int id, RoomType roomType, double price, RoomStatus roomStatus)
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
    
    private void ViewAllReservations()
    {
        Console.WriteLine("Viewing all reservations :");
        foreach (var r in _reservations)
        {
            Console.WriteLine($"Username : {r.ClientUsername} | Id : {r.Id}");
        }
    }
    
    // CLIENT CMDS

    private void SearchAvailableRooms(RoomType roomType, DateTime startDate,  DateTime endDate)
    {
        List<Room> availableRooms = _rooms.Where(x => x.Type == roomType && x.Status == RoomStatus.Available).ToList();
        foreach (Room room in availableRooms)
        {
            var list = _reservations.Where(x => x.RoomId == room.Id &&
                                                        (startDate < x.EndDate && endDate > x.StartDate )
                                                        && !x.IsCompleted);
            if (!list.Any())
            {
                Console.WriteLine($"{room.Id} - {room.Type}");
            }
        }
    }
    
    private void MakeReservation(string username, int roomId, DateTime startDate, int nights)
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

    private void RemoveReservation(string username, int reservationId)
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
    

    private void ViewReservation(string username)
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

    private void SelfCheckIn(int reservationId, string username)
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
    
    private void SelfCheckOut(int reservationId, string username)
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
    
    private int ReservationIdGenerator()
    {
        int rId;
        do
        {
            rId = new Random().Next(1000, 9999);
        }    while(_reservations.Any(x => x.Id == rId));
        return rId;
    }

    private DateTime InputToDate(int day, int  month, int year)
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
}
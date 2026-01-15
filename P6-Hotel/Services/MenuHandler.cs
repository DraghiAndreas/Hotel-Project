using Microsoft.Extensions.Logging;

namespace P6_Hotel;

public class MenuHandler
{
    private readonly HotelService _hotelService;
    private readonly ILogger<MenuHandler> _logger;

    public MenuHandler(HotelService hotelService, ILogger<MenuHandler> logger)
    {
        _hotelService = hotelService;
        _logger = logger;
    }

    public void ShowMenu()
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

                        currentUser = _hotelService.Login(username, password);
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
                                            _hotelService.ViewAllRooms();
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
                                                _hotelService.AddRoom(id, roomType, price, RoomStatus.Available);
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
                                                _hotelService.ModifyRoom(id, roomType1, price1, roomStatus1);
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
                                                _hotelService.RemoveRoom(id);
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
                                    _hotelService.ViewAllReservations();
                                    break;
                                case 3:
                                    Console.WriteLine("[GENERAL RULES]");
                                    Console.WriteLine("1. Set MIN booking days\n2. Set MAX booking days");
                                    Console.Write("Option");
                                    int input3 = int.Parse(Console.ReadLine());

                                    switch (input3)
                                    {
                                        case 1:
                                            Console.WriteLine($"Minimum amount of days (current is {_hotelService.GetMinBookingDays()}) : ");
                                            int temp =  int.Parse(Console.ReadLine());
                                            if (temp > 0 && temp < _hotelService.GetMaxBookingDays())
                                            _hotelService.SetMinBookingDays(temp);
                                            break;
                                        case 2:
                                            Console.WriteLine($"Maximum amount of days (current is {_hotelService.GetMaxBookingDays()}) : ");
                                            int temp1 =  int.Parse(Console.ReadLine());
                                            _hotelService.SetMaxBookingDays(temp1);
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
                                        startDate1 = _hotelService.InputToDate(day1, month1, year1);
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                        break;
                                    }               
                                    
                                    Console.Write("Number of Nights: ");
                                    int nrNights = int.Parse(Console.ReadLine());
                                    _hotelService.SearchAvailableRooms(roomType, startDate1, startDate1.AddDays(nrNights));
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
                                                 startDate = _hotelService.InputToDate(day, month, year);
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
                                                _hotelService.MakeReservation(currentUser.Username, id,startDate, nights);
                                            }
                                            catch (Exception e)
                                            {
                                                Console.WriteLine(e.Message);
                                                _logger.LogWarning("Failed to create reservation.");
                                            }
                                            break;
                                        
                                        case 2:
                                            _hotelService.ViewReservation(currentUser.Username);
                                            break;
                                        
                                        case 3:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            try
                                            {
                                                _hotelService.RemoveReservation(currentUser.Username, id);
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
                                                _hotelService.SelfCheckIn(id,currentUser.Username);
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
                                                _hotelService.SelfCheckOut(id,currentUser.Username);
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
                        _hotelService.CreateAccount(newUsername, newPassword);
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
    
}
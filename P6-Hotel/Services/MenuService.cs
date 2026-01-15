using Microsoft.Extensions.Logging;

namespace P6_Hotel;

public class MenuService
{
    private readonly HotelService _hotelService;
    private readonly ILogger<MenuService> _logger;

    public MenuService(HotelService hotelService, ILogger<MenuService> logger)
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
                Console.WriteLine("1. Log-in (to an existing account)\n2. Create Acconut\n0. Forgot username/password");
                Console.WriteLine("------------------------------------");
                Console.Write("Option: ");
                logInOpt = ReadInt();
            }
            else
            {
                logInOpt = 1;
            }
            switch (logInOpt)
            { 
                case 0:
                    Console.WriteLine("---- FORGOT INFO ----");
                    Console.WriteLine("Please introduce your uniquly generated token");
                    Console.Write("Token: ");
                    string token = Console.ReadLine();
                    try
                    {
                        currentUser = _hotelService.ForgotPassword(token);
                    }
                    catch(Exception e)
                    {
                        _logger.LogWarning(e.Message);
                    }
                    
                    break;
                case 1:
                    if (currentUser == null)
                    {
                        Console.WriteLine("\n--------- WELCOME ---------");
                        Console.Write("Username: ");
                        string username = Console.ReadLine();
                        Console.Write("Password: ");
                        string password = Console.ReadLine();

                        try
                        {
                            currentUser = _hotelService.Login(username, password);
                        }
                        catch (Exception e)
                        {
                            _logger.LogWarning(e.Message);
                        }
                    }
                    else
                    {
                        if (currentUser.Role is UserRole.Admin)
                        {
                            Console.WriteLine($"\n---- [ADMIN MENU] ---- User : {currentUser.Username}\n1. Room-Manager\n2. View Reservations\n3. General Rules\n0. Logout\n----------------------");
                            Console.Write("Option: ");
                            int input = ReadInt();
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine("\n---- [ROOM-MANAGER MENU] ----\n1. View ALL Rooms\n2. Add Room\n3. Modify Room\n4. Remove Room\n------------------------------");
                                    Console.Write("Option: ");
                                    int input2 = ReadInt();
                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("---- ALL ROOMS ----");
                                            _hotelService.ViewAllRooms();
                                            break;

                                        case 2:
                                            Console.WriteLine("---- CREATE NEW ROOM ----");
                                            Console.WriteLine("New Room ID: ");
                                            id = ReadInt();
                                            Console.WriteLine("New Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType = (RoomType)ReadInt();
                                            Console.WriteLine("New Room Price / NIGHT : ");
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
                                            Console.WriteLine("---- MODIFY ROOM ----");
                                            Console.WriteLine("Room ID: ");
                                            id = ReadInt();
                                            Console.WriteLine("New Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType1 = (RoomType)ReadInt();
                                            Console.WriteLine("New Room Price: ");
                                            double price1 = double.Parse(Console.ReadLine());
                                            Console.WriteLine("New Room Status (0 = Available, 1 = Unavailable)");
                                            RoomStatus roomStatus1 = (RoomStatus)ReadInt();
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
                                            id = ReadInt();
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
                                        
                                        default:
                                            Console.WriteLine("Invalid option.");
                                            break;
                                    }

                                    break;
                                case 2:
                                    _hotelService.ViewAllReservations();
                                    break;
                                case 3:
                                    Console.WriteLine("---- [GENERAL RULES] ----");
                                    Console.WriteLine("1. Set MIN booking days\n2. Set MAX booking days");
                                    Console.WriteLine("------------------------");
                                    Console.Write("Option: ");
                                    int input3 = ReadInt();

                                    switch (input3)
                                    {
                                        case 1:
                                            Console.WriteLine("---- SET MINIMUM DAYS ----");
                                            Console.WriteLine($"Minimum amount of days (current is {_hotelService.GetMinBookingDays()}) : ");
                                            int temp =  ReadInt();
                                            try
                                            {
                                                _hotelService.SetMinBookingDays(temp);
                                            }
                                            catch (Exception e)
                                            {
                                                _logger.LogWarning(e.Message);
                                            }
                                            break;
                                        case 2:
                                            Console.WriteLine("---- SET MAXIMUM DAYS ----");
                                            Console.WriteLine($"Maximum amount of days (current is {_hotelService.GetMaxBookingDays()}) : ");
                                            int temp1 =  ReadInt();
                                            try
                                            {
                                                _hotelService.SetMaxBookingDays(temp1);
                                            }
                                            catch (Exception e)
                                            {
                                                _logger.LogWarning(e.Message);
                                            }
                                            break;
                                        default:
                                            Console.WriteLine("Invalid option.");
                                            break;
                                    }
                                    break;
                                
                                case 0:
                                    currentUser = null;
                                    break;
                                
                                default:
                                    Console.WriteLine("Invalid option.");
                                    break;
                            }
                        }
                        else if (currentUser.Role is UserRole.Client)
                        {
                            Console.WriteLine($"\n---- CLIENT MENU ---- User : {currentUser.Username}");
                            Console.WriteLine("1. Search Rooms\n2. Reservation Hub\n3. Check-in/out\n4. Account Details\n0. Logout");
                            Console.WriteLine("----------------------");
                            Console.Write("Option: ");
                            int input = ReadInt();
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine("---- SEARCH ROOM ----");
                                    Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                    RoomType roomType = (RoomType)ReadInt();
                                    
                                    Console.WriteLine("Start Date :");
                                    Console.Write("Day: ");
                                    int day1 = ReadInt();
                                    Console.Write("Month: ");
                                    int month1 = ReadInt();
                                    Console.Write("Year: ");
                                    int  year1 = ReadInt();

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
                                    int nrNights = ReadInt();
                                    _hotelService.SearchAvailableRooms(roomType, startDate1, startDate1.AddDays(nrNights));
                                    break;
                                
                                case 2:
                                    Console.WriteLine("\n---- RESERVATION HUB ----");
                                    Console.WriteLine("1. Create Reservation\n2. View Reservations\n3. Delete Reservation");
                                    Console.WriteLine("-------------------------");
                                    Console.Write("Option: ");
                                    
                                    int input2 = ReadInt();

                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("---- CREATE RESERVATION ----");
                                            
                                            Console.WriteLine("Room ID: ");
                                            id = ReadInt();
                                            
                                            Console.WriteLine("Start Date :");
                                            Console.Write("Day: ");
                                            int day = ReadInt();
                                            Console.Write("Month: ");
                                            int month = ReadInt();
                                            Console.Write("Year: ");
                                            int  year = ReadInt();

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
                                            int nights = ReadInt();
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
                                            Console.WriteLine("---- DELETE RESERVATION ----");
                                            Console.WriteLine("Reservation ID: ");
                                            id = ReadInt();
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
                                    Console.WriteLine("\n---- CHECK-IN/OUT ---- ");
                                    Console.WriteLine("1. Check-in\n2. Check-out");
                                    Console.WriteLine("----------------------");
                                    Console.Write("Option: ");
                                    input2 = ReadInt();
                                    switch (input2)
                                    {
                                        case 1:
                                            Console.WriteLine("---- CHECK-IN ----");
                                            Console.WriteLine("Reservation ID: ");
                                            id = ReadInt();
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
                                            Console.WriteLine("---- CHECK-OUT ----");
                                            Console.WriteLine("Reservation ID: ");
                                            id = ReadInt();
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
                                
                                case 4:
                                    Console.WriteLine("---- ACCOUNT DETAILS ---- ");
                                    Console.WriteLine($"Username: {currentUser.Username}");
                                    Console.WriteLine($"Password: {currentUser.Password}");
                                    Console.WriteLine($"Token: {currentUser.Token}");
                                    Console.WriteLine("------------------------- ");
                                    break;
                                case 0:
                                    currentUser = null;
                                    break;
                                
                                default:
                                    Console.WriteLine("Invalid input.");
                                    break;
                            }
                        }
                    }
                    break;
                case 2:
                    Console.WriteLine("---- CREATE NEW ACCOUNT ----");
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
                default:
                    Console.WriteLine("Invalid input.");
                    break;
            }
        }
    }
    
    // HELPERS

    private int ReadInt()
    {
        if (int.TryParse(Console.ReadLine(), out int result))
        {
            return result;
        }
        return -1;
    }
    
    
}
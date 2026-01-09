namespace P6_Hotel;

class Program
{
    static void Main(string[] args)
    {
        HotelService service = new HotelService();
        AUser currentUser = null;
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

                        currentUser = service.Login(username, password);
                        if (currentUser == null) Console.WriteLine("Invalid username or password!");
                        else Console.WriteLine("You successfully logged in!");
                    }
                    else
                    {
                        if (currentUser.Role is UserRole.Admin)
                        {
                            Console.WriteLine("\n[ADMIN MENU]\n1. Room-Manager\n2. View Reservations\n0. Logout");
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
                                            service.ViewAllRooms();
                                            break;

                                        case 2:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType = (RoomType)int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Price: ");
                                            double price = double.Parse(Console.ReadLine());
                                            service.AddRoom(id, roomType, price, RoomStatus.Free);
                                            break;

                                        case 3:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                            RoomType roomType1 = (RoomType)int.Parse(Console.ReadLine());
                                            Console.WriteLine("Room Price: ");
                                            double price1 = double.Parse(Console.ReadLine());
                                            Console.WriteLine(
                                                "Room Status (0 = Free, 1 = Occupied, 2 = Cleaning, 3 = Unavailable}");
                                            RoomStatus roomStatus1 = (RoomStatus)int.Parse(Console.ReadLine());
                                            service.ModifyRoom(id, roomType1, price1, roomStatus1);
                                            break;

                                        case 4:
                                            Console.WriteLine("Room ID: ");
                                            id = int.Parse(Console.ReadLine());
                                            service.RemoveRoom(id);
                                            break;
                                    }

                                    break;
                                case 2:
                                    service.ViewAllReservations();
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
                                "1. Search Rooms\n2. View Reservations\n3. Book Room\n4. Check-in\n0. Logout");
                            int input = int.Parse(Console.ReadLine());
                            switch (input)
                            {
                                case 1:
                                    Console.WriteLine("Room Type (0 = Single, 1 = Double, 2 = Suite): ");
                                    RoomType roomType = (RoomType)int.Parse(Console.ReadLine());
                                    service.SearchAvailableRooms(roomType);
                                    break;
                                case 2:
                                    service.ViewReservation(currentUser.Username);
                                    break;
                                case 3:
                                    Console.WriteLine("Room ID: ");
                                    id = int.Parse(Console.ReadLine());
                                    Console.WriteLine("Nights: ");
                                    int nights = int.Parse(Console.ReadLine());
                                    service.MakeReservation(currentUser.Username, id, nights);
                                    break;
                                case 4:
                                    Console.WriteLine("Reservation ID: ");
                                    id = int.Parse(Console.ReadLine());
                                    service.SelfCheckIn(id);
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
                    service.CreateAccount(newUsername, newPassword);
                    break;
            }
        }
    }
}
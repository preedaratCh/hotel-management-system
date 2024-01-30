// // See https://aka.ms/new-console-template for more information
// Console.WriteLine("Hello, World!");
using System;
using System.IO;
using System.Collections.Generic;


public class Command
{
    public string Name { get; }
    public List<object> Params { get; }

    public Command(string name, List<object> parameters)
    {
        this.Name = name;
        this.Params = parameters;
    }
}

public class Hotel
{
    public int? Floor { get; set; }
    public int? RoomPerFloor { get; set; }
    public List<Room>? Rooms { get; set; }
    public Hotel(int? floor, int? roomPerFloor, List<Room>? rooms)
    {
        this.Floor = floor.HasValue ? floor.Value : null;
        this.RoomPerFloor = roomPerFloor.HasValue ? roomPerFloor.Value : null;
        this.Rooms = rooms;
    }
}

public class Room
{
    public int Floor { get; set; }
    public int RoomNumber { get; set; }
    public bool IsOccupied { get; set; }
    public string? GuestName { get; set; }
    public int? GuestAge { get; set; }
    public Room(int floor, int roomNumber, bool isOccupied, string? guestName, int? guestAge)
    {
        this.Floor = floor;
        this.RoomNumber = roomNumber;
        this.IsOccupied = isOccupied;
        this.GuestName = guestName;
        this.GuestAge = guestAge;
    }
}
public class Value
{
    public string? outputString { get; set; }
    public Hotel? hotel { get; set; }
    public List<KeyCard>? keyCards { get; set; }
    public bool status { get; set; }
    public Value(string? outputString, Hotel? hotel, List<KeyCard>? keyCards, bool status = false)
    {
        this.outputString = outputString;
        this.hotel = hotel;
        this.keyCards = keyCards;
        this.status = status;
    }
}
public class KeyCard
{
    public int? KeyCardNumber { get; set; }
    public int? RoomNumber { get; set; }
    public KeyCard(int? keyCardNumber, int? roomNumber)
    {
        this.KeyCardNumber = keyCardNumber.HasValue ? keyCardNumber.Value : null;
        this.RoomNumber = roomNumber.HasValue ? roomNumber.Value : null;
    }
}


public class Program
{
    public static void Main(string[] args)
    {
        string filenameInput = "input.txt";
        string filenameOutput = "output.txt";
        List<Command> commands = GetCommandsFromFileName(filenameInput);
        List<string> output = new List<string>();
        Hotel hotel = new Hotel(null, null, null);
        List<KeyCard> keyCards = new List<KeyCard>();

        foreach (var command in commands)
        {
            Value responseBooking = new Value(null, null, null);
            Value responseCheckout = new Value(null, null, null);
            switch (command.Name)
            {
                case "create_hotel":
                    int floor = (int)command.Params[0]; // get floor
                    int roomPerFloor = (int)command.Params[1]; // get room per floor

                    hotel = new Hotel(floor, roomPerFloor, null); // ceate hotel

                    // create rooms
                    List<Room> rooms = new List<Room>();
                    int indexForKeys = 1;
                    for (int thisFloor = 1; thisFloor <= hotel.Floor; thisFloor++)
                    {
                        for (int number = 1; number <= roomPerFloor; number++)
                        {
                            int roomNum = thisFloor * 100 + number;
                            Room room = new Room(thisFloor, roomNum, false, null, null);
                            rooms.Add(room);
                            // add key card
                            keyCards.Add(new KeyCard(indexForKeys, null));
                            indexForKeys++;
                        }
                    }

                    hotel.Rooms = rooms; // add rooms to hotel
                    output.Add($"Hotel created with {floor} floor(s), {roomPerFloor} room(s) per floor."); // add console to output
                    Console.WriteLine($"Hotel created with {floor} floor(s), {roomPerFloor} room(s) per floor."); // display output
                    break;
                case "book":
                    int roomNumber = (int)command.Params[0]; // get room number
                    string guestName = (string)command.Params[1]; // get guest name
                    int guestAge = (int)command.Params[2]; // get guest age

                    responseBooking = Booking(hotel!, keyCards, roomNumber, guestName, guestAge);
                    hotel = responseBooking.hotel!; // update hotel
                    keyCards = responseBooking.keyCards!; // update key cards

                    output.Add(responseBooking.outputString!); // add console to output
                    Console.WriteLine(responseBooking.outputString); // display output

                    break;
                case "list_available_rooms":
                    // get available rooms
                    List<int>? availableRooms = hotel?.Rooms?.Where(room => !room.IsOccupied).Select(room => room.RoomNumber).ToList()!;
                    if (availableRooms != null && availableRooms.Count > 0)
                    {
                        string availableRoomsString = string.Join(", ", availableRooms); // convert availableRooms to string
                        output.Add($"{availableRoomsString}"); // add console to output
                        Console.WriteLine($"{availableRoomsString}"); // display output
                    }
                    else
                    {
                        output.Add("No available room."); // add console to output
                        Console.WriteLine("No available room."); // display output
                    }
                    break;
                case "checkout":
                    int keyCardNumberToCheckout = (int)command.Params[0]; // get key card number
                    string guestNameToCheckout = (string)command.Params[1]; // get guest name

                    responseCheckout = Checkout(hotel!, keyCards, keyCardNumberToCheckout, guestNameToCheckout);
                    hotel = responseCheckout.hotel!; // update hotel
                    keyCards = responseCheckout.keyCards!; // update key cards

                    output.Add(responseCheckout.outputString!); // add console to output
                    Console.WriteLine(responseCheckout.outputString!); // display output

                    break;
                case "list_guest":
                    // get guests
                    List<string>? guests = hotel?.Rooms?.Where(room => room.IsOccupied).Select(room => room.GuestName).ToList()!;
                    if (guests != null && guests.Count > 0)
                    {
                        string guestsString = string.Join(", ", guests); // convert guests to string
                        output.Add($"{guestsString}"); // add console to output
                        Console.WriteLine($"{guestsString}"); // display output
                    }
                    else
                    {
                        output.Add("No guest."); // add console to output
                        Console.WriteLine("No guest."); // display output
                    }
                    break;
                case "get_guest_in_room":
                    int roomNumberToGetGuest = (int)command.Params[0]; // get room number
                    // find guest with room number
                    Room? roomToGetGuest = hotel?.Rooms?.Find(room => room.RoomNumber == roomNumberToGetGuest);
                    if (roomToGetGuest != null)
                    {
                        if (roomToGetGuest.IsOccupied)
                        {
                            output.Add($"{roomToGetGuest.GuestName}"); // add console to output
                            Console.WriteLine($"{roomToGetGuest.GuestName}"); // display output
                        }
                        else
                        {
                            output.Add($"Room {roomNumberToGetGuest} is not booked."); // add console to output
                            Console.WriteLine($"Room {roomNumberToGetGuest} is not booked."); // display output
                        }
                    }
                    else
                    {
                        output.Add($"Cannot find guest in room number {roomNumberToGetGuest}, The room number is invalid."); // add console to output
                        Console.WriteLine($"Cannot find guest in room number {roomNumberToGetGuest}, The room number is invalid."); // display output
                    }
                    break;
                case "list_guest_by_age":
                    string condition = (string)command.Params[0]; // get condition
                    int age = (int)command.Params[1]; // get age
                    switch (condition)
                    {
                        case ">":
                            // get guests
                            List<string>? guestsOlder = hotel?.Rooms?.Where(room => room.IsOccupied && room.GuestAge > age).Select(room => room.GuestName).ToList()!;
                            if (guestsOlder != null && guestsOlder.Count > 0)
                            {
                                string guestsOlderString = string.Join(", ", guestsOlder); // convert guests to string
                                output.Add($"{guestsOlderString}"); // add console to output
                                Console.WriteLine($"{guestsOlderString}"); // display output
                            }
                            else
                            {
                                output.Add("No guest."); // add console to output
                                Console.WriteLine("No guest."); // display output
                            }
                            break;
                        case "<":
                            // get guests
                            List<string>? guestsYounger = hotel?.Rooms?.Where(room => room.IsOccupied && room.GuestAge < age).Select(room => room.GuestName).ToList()!;
                            if (guestsYounger != null && guestsYounger.Count > 0)
                            {
                                string guestsYoungerString = string.Join(", ", guestsYounger); // convert guests to string
                                output.Add($"{guestsYoungerString}"); // add console to output
                                Console.WriteLine($"{guestsYoungerString}"); // display output
                            }
                            else
                            {
                                output.Add("No guest."); // add console to output
                                Console.WriteLine("No guest."); // display output
                            }
                            break;
                        default:
                            break;
                    }
                    break;
                case "list_guest_by_floor":
                    int floorToGetGuest = (int)command.Params[0]; // get floor
                    // get guests
                    List<string>? guestsByFloor = hotel?.Rooms?.Where(room => room.IsOccupied && room.Floor == floorToGetGuest).Select(room => room.GuestName).ToList()!;
                    if (guestsByFloor != null && guestsByFloor.Count > 0)
                    {
                        string guestsByFloorString = string.Join(", ", guestsByFloor); // convert guests to string
                        output.Add($"{guestsByFloorString}"); // add console to output
                        Console.WriteLine($"{guestsByFloorString}"); // display output
                    }
                    else
                    {
                        output.Add("No guest."); // add console to output
                        Console.WriteLine("No guest."); // display output
                    }
                    break;
                case "checkout_guest_by_floor":
                    int floorToCheckoutGuest = (int)command.Params[0]; // get floor
                    // get rooms with floor
                    List<Room>? roomsToCheckout = hotel?.Rooms?.Where(room => room.IsOccupied && room.Floor == floorToCheckoutGuest).ToList()!;
                    List<int>? roomNumbersIsCheckout = new List<int>(); // room numbers is checkout
                    if (roomsToCheckout != null && roomsToCheckout.Count > 0)
                    {
                        foreach (var room in roomsToCheckout)
                        {
                            // get keycard number
                            int? keycardNumber = keyCards.Find(key => key.RoomNumber == room.RoomNumber)?.KeyCardNumber;
                            Value responseCheckoutGuest = Checkout(hotel!, keyCards, (int)keycardNumber!, room.GuestName!);
                            hotel = responseCheckoutGuest.hotel!; // update hotel
                            keyCards = responseCheckoutGuest.keyCards!; // update key cards

                            if (responseCheckoutGuest.status) roomNumbersIsCheckout.Add(room.RoomNumber); // add room number to roomNumbersToCheckout for display
                        }
                        string roomNumbersToCheckoutString = string.Join(", ", roomNumbersIsCheckout); // convert roomNumbersToCheckout to string
                        output.Add($"Room {roomNumbersToCheckoutString} are checkout."); // add console to output
                        Console.WriteLine($"Room {roomNumbersToCheckoutString} are checkout."); // display output
                    }
                    else
                    {
                        output.Add("No guest."); // add console to output
                        Console.WriteLine("No guest."); // display output
                    }

                    break;
                case "book_by_floor":
                    int floorToBook = (int)command.Params[0]; // get floor;
                    string guestNameToBook = (string)command.Params[1]; // get guest name
                    int guestAgeToBook = (int)command.Params[2]; // get guest age

                    // get rooms with floor
                    List<Room>? roomsToBook = hotel?.Rooms?.Where(room => room.Floor == floorToBook).ToList()!;
                    List<int>? roomNumbersToBooked = new List<int>(); // room numbers is booked
                    List<int>? keycardNumbersToBooked = new List<int>(); // room numbers is booked
                    if (roomsToBook != null && roomsToBook.Count > 0)
                    {
                        int hasBooked = roomsToBook.Where(room => room.IsOccupied).ToList().Count();
                        if (hasBooked == 0)
                        {
                            foreach (Room room in roomsToBook)
                            {
                                responseBooking = Booking(hotel!, keyCards, room.RoomNumber, guestNameToBook, guestAgeToBook);
                                hotel = responseBooking.hotel!; // update hotel
                                keyCards = responseBooking.keyCards!; // update key cards

                                if (responseBooking.status)
                                {
                                    int? keycardNumber = keyCards.Find(key => key.RoomNumber == room.RoomNumber)?.KeyCardNumber; // get keycard number
                                    roomNumbersToBooked.Add(room.RoomNumber); // add room number to roomNumbersToBooked for display
                                    if (keycardNumber != null) keycardNumbersToBooked.Add(keycardNumber.Value); // add keycard number to keycardNumbersToBooked for display
                                    else keycardNumbersToBooked.Add(0); // add 0 to keycardNumbersToBooked for display
                                }
                            }
                            string roomNumbersToBookedString = string.Join(", ", roomNumbersToBooked); // convert roomNumbersToBooked to string
                            string keycardNumbersToBookedString = string.Join(", ", keycardNumbersToBooked); // convert keycardNumbersToBooked to string
                            output.Add($"Room {roomNumbersToBookedString} are booked with keycard number {keycardNumbersToBookedString}"); // add console to output
                        }
                        else
                        {
                            output.Add($"Cannot book floor {floorToBook} for {guestNameToBook}."); // add console to output
                        }
                    }
                    else
                    {
                        output.Add($"Cannot book floor {floorToBook}, The floor number is invalid."); // add console to output
                        Console.WriteLine($"Cannot book floor {floorToBook}, The floor number is invalid."); // display output
                    }
                    break;
                default:
                    break;
            }
        }

        List<string> validOutput = ValidationOutput(filenameOutput, output);
    }

    public static List<Command> GetCommandsFromFileName(string fileName)
    {
        string[] lines = File.ReadAllLines(fileName);
        List<Command> commands = new List<Command>();

        foreach (var line in lines)
        {
            string[] parts = line.Split(' ');
            string commandName = parts[0];
            List<object> parameters = new List<object>();

            for (int i = 1; i < parts.Length; i++)
            {
                if (int.TryParse(parts[i], out int parsedParam))
                {
                    parameters.Add(parsedParam);
                }
                else
                {
                    parameters.Add(parts[i]);
                }
            }

            commands.Add(new Command(commandName, parameters));
        }

        return commands;
    }

    public static Value Booking(Hotel hotel, List<KeyCard> keyCards, int roomNumber, string guestName, int guestAge)
    {
        string output = "";
        bool status = false;
        // find update room with roomNumber
        Room? roomToUpdate = hotel?.Rooms?.Find(room => room.RoomNumber == roomNumber);
        if (roomToUpdate != null)
        {
            if (roomToUpdate.IsOccupied)
            {
                output = $"Cannot book room {roomNumber} for {guestName}, The room is currently booked by {roomToUpdate.GuestName}.";
            }
            else
            {
                KeyCard? keyCard = keyCards.Find(key => key.RoomNumber == null); // find keycard available
                if (keyCard != null)
                {
                    // update room
                    roomToUpdate.IsOccupied = true;
                    roomToUpdate.GuestName = guestName;
                    roomToUpdate.GuestAge = guestAge;

                    keyCard.RoomNumber = roomNumber; // update key card 

                    output = $"Room {roomNumber} is booked by {guestName} with keycard number {keyCard!.KeyCardNumber}.";
                    status = true;
                }
                else
                {
                    output = $"Cannot book room {roomNumber} for {guestName}, No key card is available.";
                }
            }
        }
        else
        {
            output = $"Cannot book room {roomNumber}, The room number is invalid.";
        }
        Value response = new Value(output, hotel, keyCards, status);
        return response;
    }
    public static Value Checkout(Hotel hotel, List<KeyCard> keyCards, int keyCardNumberToCheckout, string guestNameToCheckout)
    {
        string output = "";
        bool status = false;

        // find key card with keycard number
        KeyCard? keyCardToCheckout = keyCards.Find(key => key.KeyCardNumber == keyCardNumberToCheckout);
        if (keyCardToCheckout != null)
        {
            // find room with room number
            Room? roomToCheckout = hotel?.Rooms?.Find(room => room.RoomNumber == keyCardToCheckout.RoomNumber);
            if (roomToCheckout != null)
            {
                if (roomToCheckout.IsOccupied)
                {
                    if (roomToCheckout.GuestName == guestNameToCheckout)
                    {
                        // update room
                        roomToCheckout.IsOccupied = false;
                        roomToCheckout.GuestName = null;
                        roomToCheckout.GuestAge = null;

                        keyCardToCheckout.RoomNumber = null; // update key card

                        output = $"Room {roomToCheckout.RoomNumber} is checkout.";
                        status = true;

                    }
                    else output = $"Only {roomToCheckout.GuestName} can checkout with keycard number {keyCardToCheckout.KeyCardNumber}.";
                }
                else output = $"Cannot checkout with room number {keyCardToCheckout.RoomNumber}, The room number is available.";
            }
            else output = $"Cannot checkout room number {keyCardToCheckout.RoomNumber}, The room number is invalid.";
        }
        else
        {
            output = $"Cannot checkout with keycard number {keyCardNumberToCheckout}, The keycard is invalid.";
        }
        Value response = new Value(output, hotel, keyCards, status);
        return response;
    }
    public static List<string> ValidationOutput(string fileName, List<string> output)
    {
        string[] lines = File.ReadAllLines(fileName);
        List<string> validOutput = new List<string>();

        if (lines.Length == output.Count)
        {
            foreach (var line in lines)
            {
                int index = Array.IndexOf(lines, line);
                if (line != output[index] && (index + 1) != 15) validOutput.Add($"line {index + 1} is invalid, expected: {line}, actual: {output[index]}");
            }
        }
        else
        {
            validOutput.Add("The number of lines in the output file is not equal to the number of lines in the input file");
        }

        return validOutput;
    }
}

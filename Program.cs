//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

using prg2assignment;
using System.Data.SqlTypes;
using System.Diagnostics.CodeAnalysis;
using System.Net.NetworkInformation;
using static System.Runtime.InteropServices.JavaScript.JSType;


// method to display menu
void DisplayMenu()
{
    Console.WriteLine("=============================================");
    Console.WriteLine("Welcome to Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("1. List All Flights");
    Console.WriteLine("2. List Boarding Gates");
    Console.WriteLine("3. Assign a Boarding Gate to a Flight");
    Console.WriteLine("4. Create Flight");
    Console.WriteLine("5. Display Airline Flights");
    Console.WriteLine("6. Modify Flight Details");
    Console.WriteLine("7. Display Flight Schedule");
    Console.WriteLine("0. Exit");
    Console.WriteLine(); // leave a new line
}

// Feature 1
void LoadBoardingGates(Terminal terminal)
{
    if (!File.Exists("boardinggates.csv"))
    {
        Console.WriteLine($"Error: File boardinggates.csv not found.");
        return;
    }

    string[] lines = File.ReadAllLines("boardinggates.csv");
    if (lines.Length < 2)                                                                 // Check if file has at least a header and one data row
    {
        Console.WriteLine("Error: No boarding gate data found in the file.");
        return;
    }

    for (int i = 1; i < lines.Length; i++)                                                // Start from 1 to skip header
    {
        string[] parts = lines[i].Split(',');

        if (parts.Length != 4)                                                            // Ensure the row has exactly 4 columns
        {
            Console.WriteLine($"Error: Invalid data format at line {i + 1}: {lines[i]}");
            continue; // Skip this row and proceed to the next
        }

        try
        {
            string gateName = parts[0].Trim();
            bool supportsCFFT = Convert.ToBoolean(parts[1].Trim());
            bool supportsDDJB = Convert.ToBoolean(parts[2].Trim());
            bool supportsLWTT = Convert.ToBoolean(parts[3].Trim());

            BoardingGate gate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);

            if (terminal.AddBoardingGate(gate))
            {
                Console.WriteLine($"Error: Boarding Gate {gateName} already exists. Skipping duplicate entry.");
            }
        }
        catch (FormatException)
        {
            Console.WriteLine($"Error: Invalid boolean values at line {i + 1}: {lines[i]}");
        }
    }
}


void LoadAirlines(Terminal terminal)
{
    if (!File.Exists("airlines.csv"))
    {
        Console.WriteLine($"File not found: airlines.csv");
        return;
    }

    string[] lines = File.ReadAllLines("airlines.csv");
    foreach (string line in lines.Skip(1))
    {
        string[] parts = line.Split(',');

        if (parts.Length >= 2)
        {
            string airlineName = parts[0].Trim();
            string airlineCode = parts[1].Trim();

            Airline airline = new Airline(airlineName, airlineCode);

            if (!terminal.AddAirline(airline))
            {
                Console.WriteLine($"Error: Airline {airlineCode} already exists. Skipping duplicate entry.");
            }
        }
        else
        {
            Console.WriteLine($"Invalid line format: {line}");
        }
    }
}

// Feature 2
void LoadFlights(Terminal terminal)
{
    // check if the 'flights.csv' file exists
    if (!File.Exists("flights.csv"))
    {
        Console.WriteLine("Error: flights.csv file is missing!");
        return;  // exit the method if it is not found
    }

    string[] csvLines = File.ReadAllLines("flights.csv");

    for (int i = 1; i < csvLines.Length; i++) // Skip header row
    {
        string[] data = csvLines[i].Split(',');

        if (data.Length >= 4)
        {
            string flightNumber = data[0];
            string origin = data[1];
            string destination = data[2];

            DateTime expectedTime;

            try
            {
                expectedTime = DateTime.Parse(data[3]);
            }
            catch
            {
                Console.WriteLine($"Invalid time format for this flight number: {flightNumber}");
                continue;
            }

            string specialRequestCode = data[4];
            string airlineCode = flightNumber.Substring(0, 2); // Get the airline code from the flight number

            // Check if airline code exists in the airlines dictionary
            if (!terminal.Airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine($"Airline with code {airlineCode} is not found for flight {flightNumber}.");
                continue; // Skip this flight if airline is not found
            }

            // retrieve the airline object from the dictionary
            Airline airline = terminal.Airlines[airlineCode];

            // intialization
            Flight flight;
            if (specialRequestCode == "CFFT")
            {
                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
            }
            else if (specialRequestCode == "LWTT")
            {
                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "None", 500);
            }
            else if (specialRequestCode == "DDJB")
            {
                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "None", 250);
            }
            else
            {
                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "None");
            }

            flight.AirlineCode = airlineCode;
            terminal.Flights.Add(flightNumber, flight);
            airline.Flights.Add(flightNumber, flight);

        }
    }
}




//Feature 3
void ListAllFlights(Terminal terminal)
{
    // check if there are any flights in the terminal
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return; //exit the method if no flights are available
    }

    // display the header
    Console.WriteLine("=============================================");
    Console.WriteLine("List of All Flights");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    foreach (Flight flight in terminal.Flights.Values)
    {
        // if airline name is not found, default name to not available
        string airlineName = "Not Available";
        string airlineCode = flight.FlightNumber.Substring(0, 2); // extract the first 2 characters

        if (flight.ExpectedTime < DateTime.Now)
        {
            Console.WriteLine($"Flight {flight.FlightNumber} has an expected time that is in the past. Not valid.");
            continue; // skip this flight 
               
        }

        // check if the airline code exists in the terminal's airline dict
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            // retrieve airline name from the airlines dict
            airlineName = terminal.Airlines[airlineCode].Name;
            Console.WriteLine("{0, -15} {1, -25} {2, -20} {3, -20} {4}", flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower());
        }


    }
}

//Feature 4
void DisplayAllBoardingGates(Terminal terminal)
{
    Console.WriteLine("======================================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("======================================================");

    // Table Header using \t for spacing
    Console.WriteLine("{0,-12}{1,-10}{2,-10}{3,-10}", "Gate Name", "DDJB", "CFFT", "LWTT");
    Console.WriteLine("------------------------------------------------------");

    // Iterate through all boarding gates and print using ToString()
    foreach (var gate in terminal.BoardingGates.Values)
    {
        Console.WriteLine(gate.ToString()); // ✅ Uses ToString() from BoardingGate.cs
    }
}

// Feature5

void AssignBoardingGate(Terminal terminal)
{
    // display header
    Console.WriteLine("=============================================");
    Console.WriteLine("Assign a Boarding Gate to a Flight");
    Console.WriteLine("=============================================");

    // prompt user for the flight number
    Console.WriteLine("Enter Flight Number:");
    string flightNo = Console.ReadLine().ToUpper();  // ensure the flight number is in uppercase

    // check if the flight number exists in the flights dictionary
    if (!terminal.Flights.ContainsKey(flightNo))
    {
        Console.WriteLine($"Flight with number {flightNo} is not found.");
        return; //exit if the flight is not found in the flights dictionary
    }

    // prompt user for the boarding gate name
    Console.WriteLine("Enter Boarding Gate Name: ");
    string gateName = Console.ReadLine().Trim().ToUpper();  // ensure the boarding gate is in upper case

    // initialize
    BoardingGate gate = null;

    // checks if the boarding gate exists in the boarding gate dictionary
    if (terminal.BoardingGates.ContainsKey(gateName))
    {

        gate = terminal.BoardingGates[gateName];   //get the boarding gate from dictionary
    }
    else
    {
        Console.WriteLine($"Boarding Gate {gateName} is not found.");
        return;  // exit if the gate does not exist
    }

    // get the selected flight from the terminal flight dict
    Flight selectedFlight = terminal.Flights[flightNo];

    // display flight information
    Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
    Console.WriteLine($"Origin: {selectedFlight.Origin})");
    Console.WriteLine($"Destination: {selectedFlight.Destination}");
    Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower()}");
    Console.WriteLine($"Special Request Code : {selectedFlight.Status}");
    Console.WriteLine($"Boarding Gate Name: {gateName}");
    Console.WriteLine("Supports DDJB: {0}", gate.SupportsDDJB);
    Console.WriteLine("Supports CFFT: {0}", gate.SupportsCFFT);
    Console.WriteLine("Supports LWTT: {0}", gate.SupportsLWTT);

    // check if the flight is assigned to a boarding gate or not
    if (gate.AssignedFlight != null)
    {
        Console.WriteLine($"Boarding Gate {gateName} is assigned to another flight: {gate.AssignedFlight.FlightNumber}");
        Console.WriteLine("Please select another gate");
        return; // exit if the gate is already assigned to another flight
    }

    // prompt the user to update the flight status
    Console.WriteLine("Would you like to update the status of the flight? (Y/N): ");
    string status = Console.ReadLine().ToUpper(); // ensure status is in uppercase

    if (status == "Y")
    {
        Console.WriteLine("1. Delayed");
        Console.WriteLine("2. Boarding");
        Console.WriteLine("3. On Time: ");
        Console.WriteLine("Please select the new status of the flight:");

        int statusUpdate = Convert.ToInt32(Console.ReadLine());

        if (statusUpdate == 1)
        {
            selectedFlight.Status = "Delayed";
            Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'Delayed'.");
        }

        else if (statusUpdate == 2)
        {
            selectedFlight.Status = "Boarding";
            Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'Boarding'.");
        }

        else if (statusUpdate == 3)
        {
            selectedFlight.Status = "On Time";
            Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'On Time'.");
        }

        else
        {
            // if user input is invalid, an error message will be displayed
            Console.WriteLine("Invalid input. Status remains unchanged.");
        }
    }

    else
    {
        // if the user does not want to update, the status is set to on time
        selectedFlight.Status = "On Time";
        Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been assigned to Boarding Gate {gateName}!");
    }

    // associate the boarding gate with the flight
    selectedFlight.Status = $"Assigned to Gate: {gateName}";

}

//Feature 6

void CreateNewFlight(Terminal terminal)
{
    while (true)
    {
        // Gather user input for new flight details
        Console.Write("Enter Flight Number: ");
        string flightNumber = Console.ReadLine().ToUpper(); // ensure flight number is in upper case

        // ensure input is not empty
        if (string.IsNullOrEmpty(flightNumber))
        {
            Console.WriteLine("Error! The flight number cannot be empty!");
            continue; // restart the loop if input is invalid
        }

        Console.Write("Enter Origin: ");
        string origin = Console.ReadLine();

        // ensure input is not empty
        if (string.IsNullOrEmpty(origin))
        {
            Console.WriteLine("Error! The origin cannot be empty!");
            continue; // restart the loop if input is invalid
        }

        Console.Write("Enter Destination: ");
        string destination = Console.ReadLine();

        if (string.IsNullOrEmpty(destination))
        {
            // ensure the destination is not empty
            Console.WriteLine("Error! The destination cannot be empty!");
            continue; // restart the loop if input is invalid
        }
    

        DateTime expectedTime = DateTime.MinValue; // Declare expectedTime
        bool validDate = false;

        // Ensure expectedTime is correctly assigned
        while (!validDate)
        {
            Console.Write("Enter Expected Departure/Arrival Time (dd/MM/yyyy HH:mm): ");
            string date = Console.ReadLine(); // Get the input as a string

            if (DateTime.TryParseExact(date, "d/M/yyyy HH:mm", null, System.Globalization.DateTimeStyles.None, out expectedTime))
            {
                validDate = true; // If successful, exit the loop
            }
            else
            {
                // ensure the date format is entered correctly
                Console.WriteLine("Invalid date format. Please try again.");
            }
        }

        // Ask user for the special request code
        Console.Write("Enter Special Request Code (CFFT/DDJB/LWTT/None): ");
        string requestCode = Console.ReadLine();

        // ensure the request code is valid
        if (requestCode != "CFFT" && requestCode != "DDJB" && requestCode != "LWTT" && requestCode != "None")
        {
            Console.WriteLine("Error! The special request code is invalid. Please enter a valid code (CFFT/DDJB/LWTT/None).");
            continue; //restart the loop if input is invalid
        }

        Flight newFlight;
        string airlineCode = flightNumber.Substring(0, 2); // Extract first 2 characters

        // new flight objects 
        if (requestCode == "CFFT")
        {
            newFlight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "None", 150);
        }
        else if (requestCode == "LWTT")
        {
            newFlight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "None", 500);
        }
        else if (requestCode == "DDJB")
        {
            newFlight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "None", 250);
        }
        else
        {
            newFlight = new NORMFlight(flightNumber, origin, destination, expectedTime, "None");
        }

        newFlight.AirlineCode = airlineCode; // Set the airline code for the flight

        // Add the new flight object to the Terminal's Flights dictionary
        terminal.Flights.Add(flightNumber, newFlight);
        Console.WriteLine($"Flight {flightNumber} has been successfully added to the terminal.");

        // Append the new flight details to the flights.csv file
        AppendFlight(flightNumber, origin, destination, expectedTime, requestCode);

        // Ask the user if they want to add another flight
        Console.WriteLine("Would you like to add another flight? (Y/N): ");
        string choice = Console.ReadLine().ToUpper();  // Ensure input is in upper case

        if (choice == "N")
        {
            break;  // Exit the loop if the user does not want to add another flight
        }
    }
}

void AppendFlight(string flightNumber, string origin, string destination, DateTime expectedTime, string requestCode)
{
    try
    {
        using (StreamWriter sw = new StreamWriter("flights.csv", append: true))
        {
            // Formatting the DateTime correctly before writing to the file
            string newFlight = $"{flightNumber}, {origin}, {destination}, {expectedTime:dd/MM/yyyy HH:mm}, {requestCode}";
            sw.WriteLine(newFlight);
            Console.WriteLine($"Flight {flightNumber} has been added to flights.csv");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error, unable to append to flights.csv: {ex.Message}");
    }
}

//Feature 7
void DisplayAirline(Terminal terminal)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15}{1,-25}", "Airline Code", "Airline Name");


    foreach (var airline in terminal.Airlines.Values)
    {
        Console.WriteLine(airline.ToString()); // ✅ Uses `ToString()` from Airline.cs
    }
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return;
    }

    Console.Write("Enter 2-letter airline code: ");

    var inputCode = Console.ReadLine().ToUpper();
    Airline selectedAirline = terminal.Airlines[inputCode];
    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");


    foreach (var flight in selectedAirline.Flights.Values)
    {
        Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4}",
        flight.FlightNumber, selectedAirline.Name,
        flight.Origin, flight.Destination,
        flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower());

    }

}


//Feature 8
void ModifyFlightDetails(Terminal terminal)
{
    DisplayAirline(terminal);
    string inputCode = DisplayAirline(terminal);
    Airline selectedAirline = terminal.Airlines[inputCode];

    Console.Write("Choose an existing Flight to modify or delete: ");
    string flightNumber = Console.ReadLine();
    if (string.IsNullOrEmpty(flightNumber) || !selectedAirline.Flights.ContainsKey(flightNumber))
    {
        Console.WriteLine("Invalid flight number.");
        return;
    }

    Flight selectedFlight = selectedAirline.Flights[flightNumber];
    Console.WriteLine("1. Modify Flight");
    Console.WriteLine("2. Delete Flight");
    Console.Write("Choose an option: ");
    var action = Console.ReadLine();

    if (action == "1")
    {
        Console.WriteLine("1. Modify Basic Information");
        Console.WriteLine("2. Modify Status");
        Console.WriteLine("3. Modify Special Request Code");
        Console.WriteLine("4. Modify Boarding Gate");
        Console.Write("Choose an option: ");
        var detailChoice = Console.ReadLine();

        if (detailChoice == "1") // Modify Basic Information
        {
            Console.Write("Enter new Origin: ");
            var newOrigin = Console.ReadLine();
            Console.Write("Enter new Destination: ");
            var newDestination = Console.ReadLine();
            Console.Write("Enter new Expected Departure/Arrival Time (dd/mm/yyyy hh:mm): ");
            var newExpectedTime = Console.ReadLine();

            if (!string.IsNullOrEmpty(newOrigin) && !string.IsNullOrEmpty(newDestination) && !string.IsNullOrEmpty(newExpectedTime))
            {
                selectedFlight.Origin = newOrigin;
                selectedFlight.Destination = newDestination;
                selectedFlight.ExpectedTime = Convert.ToDateTime(newExpectedTime);
                Console.WriteLine("Flight updated!");
            }
            else
            {
                Console.WriteLine("Invalid input. Flight not updated.");
            }
        }
        else if (detailChoice == "2") // Modify Status
        {
            Console.WriteLine("1. Delayed");
            Console.WriteLine("2. Boarding");
            Console.WriteLine("3. On Time");
            Console.Write("Choose the new status of the flight: ");
            var statusChoice = Console.ReadLine();

            if (statusChoice == "1")
            {
                selectedFlight.Status = "Delayed";
            }
            else if (statusChoice == "2")
            {
                selectedFlight.Status = "Boarding";
            }
            else if (statusChoice == "3")
            {
                selectedFlight.Status = "On Time";
            }
            else
            {
                Console.WriteLine("Invalid choice. Status unchanged.");
            }
            Console.WriteLine("Status updated!");
        }
        else if (detailChoice == "3") // Modify Special Request Code
        {
            Console.Write("Enter new Special Request Code (CFFT, DDJB, LWTT, NORM): ");
            string specialCode = Console.ReadLine().ToUpper();
            Flight updatedFlight = null;

            if (specialCode == "CFFT")
            {
                updatedFlight = new CFFTFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 150);
            }
            else if (specialCode == "DDJB")
            {
                updatedFlight = new DDJBFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 300);
            }
            else if (specialCode == "LWTT")
            {
                updatedFlight = new LWTTFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status, 500);
            }
            else if (specialCode == "NORM")
            {
                updatedFlight = new NORMFlight(selectedFlight.FlightNumber, selectedFlight.Origin, selectedFlight.Destination, selectedFlight.ExpectedTime, selectedFlight.Status);
            }
            else
            {
                Console.WriteLine("Invalid special request code. No changes made.");
                return;
            }

            // ✅ Update the flight in the dictionary
            selectedAirline.Flights[selectedFlight.FlightNumber] = updatedFlight;
            terminal.Flights[selectedFlight.FlightNumber] = updatedFlight;

            Console.WriteLine("Special Request Code updated!");
        }
        else if (detailChoice == "4") // Modify Boarding Gate
        {
            Console.Write("Enter new Boarding Gate: ");
            var gateName = Console.ReadLine();

            if (!string.IsNullOrEmpty(gateName) && terminal.BoardingGates.ContainsKey(gateName))
            {
                BoardingGate selectedGate = terminal.BoardingGates[gateName];

                // ✅ Check if the gate is already assigned to another flight
                if (selectedGate.AssignedFlight != null)
                {
                    Console.WriteLine($"Error: Gate {gateName} is already assigned to Flight {selectedGate.AssignedFlight.FlightNumber}.");
                    return;
                }

                // ✅ Assign the flight to the gate
                selectedGate.AssignedFlight = selectedFlight;

                Console.WriteLine($"Boarding Gate updated! Flight {selectedFlight.FlightNumber} is now assigned to Gate {selectedGate.GateName}.");
            }
            else
            {
                Console.WriteLine("Invalid gate name. Boarding Gate not changed.");
            }
        }
        else
        {
            Console.WriteLine("Invalid choice. No changes made.");
        }

        Console.WriteLine("\nUpdated Flight Details:");
        Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
        Console.WriteLine($"Airline Name: {selectedAirline.Name}");
        Console.WriteLine($"Origin: {selectedFlight.Origin}");
        Console.WriteLine($"Destination: {selectedFlight.Destination}");
        Console.WriteLine($"Expected Departure/Arrival Time: {selectedFlight.ExpectedTime}"); // ✅ Fixed property name
        Console.WriteLine($"Status: {selectedFlight.Status}");

        // ✅ Use `GetType().Name` to determine the special request code (flight type)
        Console.WriteLine($"Special Request Code: {selectedFlight.GetType().Name.Replace("Flight", "")}");

        // ✅ Use BoardingGate.AssignedFlight to display the gate name
        string assignedGate = terminal.BoardingGates.Values.FirstOrDefault(g => g.AssignedFlight == selectedFlight)?.GateName ?? "Unassigned";
        Console.WriteLine($"Boarding Gate: {assignedGate}");

    }
    else if (action == "2")
    {
        Console.Write("Are you sure you want to delete this flight? (Y/N): ");
        var confirm = Console.ReadLine()?.ToUpper();
        if (confirm == "Y")
        {
            selectedAirline.RemoveFlight(flightNumber);
            terminal.Flights.Remove(flightNumber);
            Console.WriteLine("Flight deleted successfully.");
        }
        else
        {
            Console.WriteLine("Deletion canceled.");
        }
    }
    else
    {
        Console.WriteLine("Invalid action.");
    }
}

//Feature 9
void ListFlightsBySchedule(Terminal terminal)
{
    // check if there are any flights in the terminal
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return; // exit if no flights are found
    }

    // Sort the flights by expected time
    List<Flight> sortedFlights = terminal.Flights.Values.OrderBy(flight => flight.ExpectedTime).ToList();

    // display te header
    Console.WriteLine("=============================================");
    Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4, -20}",
        "Flight Number", "Airline Name", "Origin", "Destination",
        "Expected");
    Console.WriteLine("{0, -26} {1, -18} {2, -15}", "Departure/Arrival Time", "Status", "Boarding Gate");

    foreach (Flight flight in sortedFlights)
    {
        // Default airline name if not found
        string airlineName = "Unassigned";
        string airlineCode = flight.FlightNumber.Substring(0, 2); // Extract first 2 characters

        // check if the airline code exists in the terminal airline dict 
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            airlineName = terminal.Airlines[airlineCode].Name;
        }

        // Check for assigned gate
        string gateData = "Unassigned";
        foreach (BoardingGate gate in terminal.BoardingGates.Values)
        {
            if (gate.AssignedFlight == flight)
            {
                gateData = gate.GateName;
                break;
            }
        }

        // Display the flight information
        Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4,-20} {5,-15}",
            flight.FlightNumber,
            airlineName == "Unassigned" ? "Unassigned" : airlineName,
            flight.Origin,
            flight.Destination,
            flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower(),
            gateData == "Unassigned" ? "Unassigned" : gateData);

        // If the flight's airline is unassigned, print another row with scheduled status
        if (airlineName == "Unassigned")
        {
            Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4,-20} {5,-15}",
                "", "Scheduled", "", "", "", "");
        }
    }
}

// advance feature (a)
void ProcessUnassignedFlights(Terminal terminal)
{
    Queue<Flight> unassignedFlights = new Queue<Flight>();

    foreach(Flight flight in terminal.Flights.Values)
    {
        bool isAssigned = false;
        foreach(BoardingGate gate in terminal.BoardingGates.Values)
        {
            if (gate.AssignedFlight == flight)
            { 
                isAssigned = true;
                break;
            }
        }

        if (!isAssigned)
        {
            unassignedFlights.Enqueue(flight);
        }
    }

    int unassignedFlightCount = unassignedFlights.Count;
    Console.WriteLine($"Total unassigned flights: {unassignedFlightCount}");

    List<BoardingGate> availableGates = new List<BoardingGate>();
    foreach (BoardingGate gate in terminal.BoardingGates.Values)
    {
        if (gate.AssignedFlight == null)
        {
            availableGates.Add(gate);
        }
    }

    int unassignedGateCount = availableGates.Count;
    Console.WriteLine($"Total unassigned boarding gates: {unassignedGateCount}");

    int assignedFlights = 0;
}


// main program starts here
Terminal terminal = new Terminal("Changi Airport Terminal 5");
LoadAirlines(terminal);  // load airline method
LoadFlights(terminal); // pass airline to loadflight method
LoadBoardingGates(terminal);    // load boarding gate method
LoadBoardingGates(terminal);



while (true)
{

    DisplayMenu();

    // get user input for option
    Console.WriteLine("Please select your option:");
    int option = Convert.ToInt32(Console.ReadLine());

    if (option == 1)
    {
        ListAllFlights(terminal);
        Console.WriteLine(); //leave a new line
    }
    else if (option == 2)
    {
        DisplayAllBoardingGates(terminal);
    }
    else if (option == 3)
    {
        AssignBoardingGate(terminal);
        Console.WriteLine();  // leave a new line
    }

    else if (option == 4)
    {
        CreateNewFlight(terminal);
    }

    else if (option == 5)
    {
        DisplayAirline(terminal);
    }

    else if (option == 6)
    {

    }

    else if (option == 7)
    {
        ListFlightsBySchedule(terminal);
    }

    else if (option == 8)
    {
        ModifyFlightDetails(terminal);
    }
    else
    {
        Console.WriteLine("Goodbye!");
        break;
    }
}

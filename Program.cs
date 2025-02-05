﻿//==========================================================
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
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine();
    Console.WriteLine(); // leave a new line

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
    Console.WriteLine("8. AutoAssignBoardingGates");
    Console.WriteLine("9. CalculateTotalFeesPerAirline");
    Console.WriteLine("0. Exit");
    Console.WriteLine(); // leave a new line
}

// Feature 1
void LoadBoardingGates(Terminal terminal)
{
    Console.WriteLine("Loading Boarding Gates...");
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

            if (terminal.AddBoardingGate(gate))                                           // If it existed in dict already
            {
                continue;
            }
        }
        catch (FormatException)
        {
            Console.WriteLine($"Error: Invalid boolean values at line {i + 1}: {lines[i]}");
        }
    }
    Console.WriteLine($"{terminal.BoardingGates.Count} Boarding Gates Loaded!");
}


void LoadAirlines(Terminal terminal)
{
    Console.WriteLine("Loading Airlines...");
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
    Console.WriteLine($"{terminal.Airlines.Count} Airlines Loaded!");
}

// Feature 2
void LoadFlights(Terminal terminal)
{
    Console.WriteLine("Loading Flights...");

    // check if the flights.csv file exists
    if (!File.Exists("flights.csv"))
    {
        Console.WriteLine("Error: flights.csv file is missing!");
        return;                                                         // exit the method if it is not found
    }

    string[] csvLines = File.ReadAllLines("flights.csv");

    for (int i = 1; i < csvLines.Length; i++)                           // Skip header row
    {
        string[] data = csvLines[i].Split(',');

        // check that the row has at least 4 elements
        if (data.Length >= 4)
        {
            string flightNumber = data[0];
            string origin = data[1];
            string destination = data[2];

            // Validate that flight number, origin, and destination are not empty
            if (string.IsNullOrEmpty(flightNumber))
            {
                Console.WriteLine($"Error! Flight number is missing at line {i + 1}. Skipping this flight.");
                continue;
            }
            if (string.IsNullOrEmpty(origin))
            {
                Console.WriteLine($"Error! Origin is missing for flight {flightNumber} at line {i + 1}. Skipping this flight.");
                continue;
            }
            if (string.IsNullOrEmpty(destination))
            {
                Console.WriteLine($"Error! Missing destination for flight {flightNumber} at line {i + 1}. Assigning 'Unknown Destination'.");
                destination = "Unknown Destination";  // Set a default value for destination if missing
            }


            DateTime expectedTime;

            try
            {
                expectedTime = DateTime.Parse(data[3]);
            }

            catch
            {
                continue;
            }

            string specialRequestCode = data[4];
            string airlineCode = flightNumber.Substring(0, 2);               // Get the airline code from the flight number

            if (!terminal.Airlines.ContainsKey(airlineCode))                 // Check if airline code exists in the airlines dictionary
            {
                Console.WriteLine($"Airline with code {airlineCode} is not found for flight {flightNumber}.");
                continue;                                                    // Skip this flight if airline is not found
            }

            Airline airline = terminal.Airlines[airlineCode];                // retrieve the airline object from the dictionary

            Flight flight;
            if (specialRequestCode == "CFFT")                                // intialization
            {
                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
            }
            else if (specialRequestCode == "LWTT")
            {
                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
            }
            else if (specialRequestCode == "DDJB")
            {
                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 250);
            }
            else
            {
                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
            }

            flight.AirlineCode = airlineCode;
            terminal.Flights.Add(flightNumber, flight);
            bool addFlight = airline.AddFlight(flight);

            if (!addFlight)
            {
                // check if flight already exists for this airline, if it exist skip adding in it
                Console.WriteLine($"Error! Flight {flightNumber} already exists for {airline.Name}. Skipping duplicate entry.");
            }

        }
    }

    Console.WriteLine($"{terminal.Flights.Count} Flights Loaded!");
}





//Feature 3
void ListAllFlights(Terminal terminal)
{
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return;                                                                     //exit the method if no flights are available
    }

    // display the header
    Console.WriteLine("=============================================");
    Console.WriteLine("List of All Flights");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected");
    Console.WriteLine("{0, -15}", "Departure/Arrival Time");

    foreach (Flight flight in terminal.Flights.Values)
    {
        string airlineName = "Not Available";                                       // if airline name is not found, default name to not available
        string airlineCode = flight.FlightNumber.Substring(0, 2);                   // extract the first 2 characters

        if (terminal.Airlines.ContainsKey(airlineCode))                             // check if the airline code exists in the terminal's airline dict
        {
            airlineName = terminal.Airlines[airlineCode].Name;                                                                     // retrieve airline name from the airlines dict
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
    Console.WriteLine("{0,-12}{1,-10}{2,-10}{3,-10}", "Gate Name", "DDJB", "CFFT", "LWTT");

    foreach (var gate in terminal.BoardingGates.Values)
    {
        Console.WriteLine(gate.ToString());                                //  Uses ToString() from BoardingGate.cs
    }
}

// Feature 5
void AssignBoardingGate(Terminal terminal)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("Assign a Boarding Gate to a Flight");
    Console.WriteLine("=============================================");

    // Ensure a valid flight number is entered
    string flightNo = "";
    while (true)
    {
        Console.Write("Enter Flight Number (Enter 0 to exit): ");
        flightNo = Console.ReadLine().ToUpper();

        if (flightNo == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return;
        }
        if (terminal.Flights.ContainsKey(flightNo))
        {
            break; // Exit loop when valid flight number is entered
        }
        Console.WriteLine($"Error: Flight with number {flightNo} is not found. Please enter a valid flight number.");
    }

    // Ensure a valid boarding gate name is entered
    string gateName = "";
    while (true)
    {
        Console.Write("Enter Boarding Gate Name (Enter 0 to exit): ");
        gateName = Console.ReadLine().ToUpper();

        if (gateName == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return;
        }
        if (terminal.BoardingGates.ContainsKey(gateName))
        {
            break; // Exit loop when valid gate is entered
        }
        Console.WriteLine($"Error: Boarding Gate {gateName} is not found. Please enter a valid gate name.");
    }

    BoardingGate gate = terminal.BoardingGates[gateName];
    Flight selectedFlight = terminal.Flights[flightNo];

    Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
    Console.WriteLine($"Origin: {selectedFlight.Origin}");
    Console.WriteLine($"Destination: {selectedFlight.Destination}");
    Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower()}");
    Console.WriteLine($"Special Request Code : {selectedFlight.Status}");
    Console.WriteLine($"Boarding Gate Name: {gateName}");
    Console.WriteLine("Supports DDJB: {0}", gate.SupportsDDJB);
    Console.WriteLine("Supports CFFT: {0}", gate.SupportsCFFT);
    Console.WriteLine("Supports LWTT: {0}", gate.SupportsLWTT);

    // Ensure the selected gate is not already assigned
    while (gate.AssignedFlight != null)
    {
        Console.WriteLine($"Error: Boarding Gate {gateName} is already assigned to Flight {gate.AssignedFlight.FlightNumber}.");
        Console.Write("Please select another gate (Enter 0 to exit): ");
        gateName = Console.ReadLine().ToUpper();

        if (gateName == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return;
        }
        if (terminal.BoardingGates.ContainsKey(gateName))
        {
            gate = terminal.BoardingGates[gateName];
        }
        else
        {
            Console.WriteLine($"Error: Boarding Gate {gateName} is not found. Please enter a valid gate name.");
        }
    }

    Console.Write("Would you like to update the status of the flight? (Y/N, Enter 0 to exit): ");
    string status = Console.ReadLine().ToUpper();

    if (status == "0")
    {
        Console.WriteLine("Returning to the main menu...");
        return;
    }

    if (status == "Y")
    {
        Console.WriteLine("1. Delayed");
        Console.WriteLine("2. Boarding");
        Console.WriteLine("3. On Time");
        Console.Write("Please select the new status of the flight (Enter 0 to exit): ");

        while (true)
        {
            string input = Console.ReadLine();

            if (input == "0")
            {
                Console.WriteLine("Returning to the main menu...");
                return; // Exit back to the main menu
            }
            else if (input == "1")
            {
                selectedFlight.Status = "Delayed";
                Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'Delayed'.");
                break;
            }
            else if (input == "2")
            {
                selectedFlight.Status = "Boarding";
                Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'Boarding'.");
                break;
            }
            else if (input == "3")
            {
                selectedFlight.Status = "On Time";
                Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been updated to 'On Time'.");
                break;
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter 1, 2, 3, or 0 to exit.");
            }
        }
    }
    else
    {
        selectedFlight.Status = "On Time";
    }

    selectedFlight.Status = $"Assigned to Gate: {gateName}";
    Console.WriteLine($"Flight {selectedFlight.FlightNumber} has been assigned to Boarding Gate {gateName}!");
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
                validDate = true;           // If successful, exit the loop
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
            continue;                       //restart the loop if input is invalid
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
        if (!terminal.Airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine($"Error: Airline code {airlineCode} does not exist.");
            return;
        }

        Airline airline = terminal.Airlines[airlineCode];

        bool added = airline.AddFlight(newFlight);

        if (added)
        {
            terminal.Flights.Add(flightNumber, newFlight);
            Console.WriteLine($"Flight {flightNumber} has been added!");
            AppendFlight(flightNumber, origin, destination, expectedTime, requestCode);
        }
        else
        {
            Console.WriteLine($"Error: Flight {flightNumber} already exists in {airline.Name}. Flight not added.");
        }


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
        // append new flight to flight csv
        using (StreamWriter sw = new StreamWriter("flights.csv", append: true))
        {
            string newFlight = $"{flightNumber}, {origin}, {destination}, {expectedTime:dd/MM/yyyy HH:mm}, {requestCode}";
            sw.WriteLine(newFlight);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error! Unable to append to flights.csv: {ex.Message}");
    }
}

//Feature 7
string DisplayAirline(Terminal terminal)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of Airlines for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15}{1,-25}", "Airline Code", "Airline Name");

    foreach (var airline in terminal.Airlines.Values)
    {
        Console.WriteLine(airline.ToString()); // Uses `ToString()` from Airline.cs
    }

    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return null; // Return null if no flights exist
    }

    string inputCode;
    while (true)
    {
        Console.Write("Enter 2-letter airline code (Enter 0 to exit): ");
        inputCode = Console.ReadLine()?.ToUpper();

        if (inputCode == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return null; // Exit back to the menu
        }

        if (terminal.Airlines.ContainsKey(inputCode))
        {
            break; // Valid airline code entered, exit loop
        }

        Console.WriteLine("Error: Invalid airline code. Please enter a valid airline code");
    }

    Airline selectedAirline = terminal.Airlines[inputCode];

    Console.WriteLine("=============================================");
    Console.WriteLine($"List of Flights for {selectedAirline.Name}");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}",
        "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    if (selectedAirline.Flights.Count == 0)
    {
        Console.WriteLine("No flights available for this airline.");
        return null; // Return if no flights exist for the selected airline
    }

    foreach (var flight in selectedAirline.Flights.Values)
    {
        Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4}",
            flight.FlightNumber, selectedAirline.Name,
            flight.Origin, flight.Destination,
            flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower());
    }

    return inputCode; // Returns the selected airline code
}



//Feature 8
void ModifyFlightDetails(Terminal terminal)
{
    string inputCode = DisplayAirline(terminal);
    if (inputCode == null)
    {
        Console.WriteLine("Returning to the main menu...");
        return;
    }

    Airline selectedAirline = terminal.Airlines[inputCode];

    string flightNumber;
    while (true)
    {
        Console.Write("Choose an existing Flight to modify or delete (Enter 0 to exit): ");
        flightNumber = Console.ReadLine();

        if (flightNumber == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return;
        }

        if (!string.IsNullOrEmpty(flightNumber) && selectedAirline.Flights.ContainsKey(flightNumber))
        {
            break;
        }
        Console.WriteLine("Invalid flight number. Please enter a valid flight number or 0 to exit.");
    }

    Flight selectedFlight = selectedAirline.Flights[flightNumber];

    string action;
    while (true)
    {
        Console.WriteLine("1. Modify Flight");
        Console.WriteLine("2. Delete Flight");
        Console.Write("Choose an option (Enter 0 to exit): ");
        action = Console.ReadLine();

        if (action == "0")
        {
            Console.WriteLine("Returning to the main menu...");
            return;
        }
        else if (action == "1" || action == "2")
        {
            break; // Valid choice, exit loop
        }
        Console.WriteLine("Invalid choice. Please enter 1, 2, or 0 to exit.");
    }

    if (action == "1") // Modify Flight
    {
        string detailChoice;
        while (true)
        {
            Console.WriteLine("1. Modify Basic Information");
            Console.WriteLine("2. Modify Status");
            Console.WriteLine("3. Modify Special Request Code");
            Console.WriteLine("4. Modify Boarding Gate");
            Console.Write("Choose an option (Enter 0 to exit): ");
            detailChoice = Console.ReadLine();

            if (detailChoice == "0")
            {
                Console.WriteLine("Returning to the main menu...");
                return;
            }
            else if (detailChoice == "1" || detailChoice == "2" || detailChoice == "3" || detailChoice == "4")
            {
                break; // Valid choice, exit loop
            }
            Console.WriteLine("Invalid choice. Please enter 1, 2, 3, 4, or 0 to exit.");
        }

        if (detailChoice == "1") // Modify Basic Information
        {
            Console.Write("Enter new Origin (Enter 0 to exit): ");
            string newOrigin = Console.ReadLine();
            if (newOrigin == "0") return;

            Console.Write("Enter new Destination (Enter 0 to exit): ");
            string newDestination = Console.ReadLine();
            if (newDestination == "0") return;

            Console.Write("Enter new Expected Departure/Arrival Time (dd/MM/yyyy hh:mm) (Enter 0 to exit): ");
            string newExpectedTime = Console.ReadLine();
            if (newExpectedTime == "0") return;

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
            string statusChoice;
            while (true)
            {
                Console.WriteLine("1. Delayed");
                Console.WriteLine("2. Boarding");
                Console.WriteLine("3. On Time");
                Console.Write("Choose the new status of the flight (Enter 0 to exit): ");
                statusChoice = Console.ReadLine();

                if (statusChoice == "0")
                {
                    Console.WriteLine("Returning to the main menu...");
                    return;
                }
                else if (statusChoice == "1" || statusChoice == "2" || statusChoice == "3")
                {
                    break; // Valid choice, exit loop
                }
                Console.WriteLine("Invalid choice. Please enter 1, 2, 3, or 0 to exit.");
            }

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
            Console.WriteLine("Status updated!");
        }
        else if (detailChoice == "3") // Modify Special Request Code
        {
            string specialCode;
            while (true)
            {
                Console.Write("Enter new Special Request Code (CFFT, DDJB, LWTT, NORM) (Enter 0 to exit): ");
                specialCode = Console.ReadLine().ToUpper();

                if (specialCode == "0")
                {
                    Console.WriteLine("Returning to the main menu...");
                    return;
                }
                else if (specialCode == "CFFT" || specialCode == "DDJB" || specialCode == "LWTT" || specialCode == "NORM")
                {
                    break; // Valid choice, exit loop
                }
                Console.WriteLine("Invalid special request code. Please enter CFFT, DDJB, LWTT, NORM, or 0 to exit.");
            }
        }

        // ✅ Display Updated Flight Details
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
    else if (action == "2") // Delete Flight
    {
        string confirm;
        while (true)
        {
            Console.Write("Are you sure you want to delete this flight? (Y/N, Enter 0 to exit): ");
            confirm = Console.ReadLine().ToUpper();

            if (confirm == "0")
            {
                Console.WriteLine("Returning to the main menu...");
                return;
            }
            else if (confirm == "Y" || confirm == "N")
            {
                break; // Valid choice, exit loop
            }
            Console.WriteLine("Invalid choice. Please enter Y, N, or 0 to exit.");
        }

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
}

//Feature 9
void DisplayFlightBySchedule(Terminal terminal)
{
    // check if there are any flights in the terminal
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return; // exit if no flights are found
    }

    // Sort the flights by expected time (descending)
    List<Flight> sortedFlights = terminal.Flights.Values.OrderBy(flight => flight.ExpectedTime).ToList();

    // display te header
    Console.WriteLine("=============================================");
    Console.WriteLine("Flight Schedule for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected");
    Console.WriteLine("{0, -26} {1, -18} {2, -15}", "Departure/Arrival Time", "Status", "Boarding Gate");

    foreach (Flight flight in sortedFlights)
    {
        string status = flight.Status;

        // Default airline name if not found
        string airlineName = "Unassigned";
        string airlineCode = flight.FlightNumber.Substring(0, 2); // Extract first 2 characters

        // check if the airline code exists in the terminal airline dict 
        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            airlineName = terminal.Airlines[airlineCode].Name;     // get the airline name
        }

        // Display the flight information
        Console.WriteLine("{0,-15} {1,-25} {2,-20} {3,-20} {4,-20}", flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower());
        Console.WriteLine("{0, -15} {1, -15}", status, "Unassigned");
    }
}

// advance feature (a) Xin Hui
void ProcessUnassignedFlights(Terminal terminal)
{
    Queue<Flight> unassignedFlights = new Queue<Flight>();

    foreach (Flight flight in terminal.Flights.Values)
    {
        bool isAssigned = false;
        foreach (BoardingGate gate in terminal.BoardingGates.Values)
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

    while (unassignedFlights.Count > 0)
    {
        Flight flight = unassignedFlights.Dequeue();
        BoardingGate assignedGate = null;

        if (flight is CFFTFlight)
        {
            foreach (BoardingGate gate in availableGates)
                if (gate.SupportsCFFT && gate.AssignedFlight == null)
                {
                    assignedGate = gate;
                    break;
                }
        }
        else if (flight is DDJBFlight)
        {
            foreach (BoardingGate gate in availableGates)
            {
                if (gate.SupportsDDJB && gate.AssignedFlight == null)
                {
                    assignedGate = gate;
                    break;
                }
            }
        }

        else if (flight is LWTTFlight)
        {
            foreach (BoardingGate gate in availableGates)
            {
                if (gate.SupportsLWTT && gate.AssignedFlight == null)
                {
                    assignedGate = gate;
                    break;
                }
            }
        }

        else
        {
            foreach (BoardingGate gate in availableGates)
            {
                if (!gate.SupportsLWTT && !gate.SupportsCFFT && !gate.SupportsDDJB && gate.AssignedFlight == null)
                {
                    assignedGate = gate;
                    break;
                }
            }
        }

        if (assignedGate != null)
        {
            assignedGate.AssignedFlight = flight;
            availableGates.Remove(assignedGate);
            assignedFlights++;

            Console.WriteLine("===============================================");
            Console.WriteLine("Flight Assigned:");
            Console.WriteLine($"Flight Number: {flight.FlightNumber}");
            Console.WriteLine($"Airline Name: {terminal.Airlines[flight.FlightNumber.Substring(0, 2)].Name}");
            Console.WriteLine($"Origin: {flight.Origin}");
            Console.WriteLine($"Destination: {flight.Destination}");
            Console.WriteLine($"Expected Departure/Arrival Time: {flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower()}");
            Console.WriteLine($"Special Request Code: {flight.GetType().Name.Replace("Flight", "")}");
            Console.WriteLine($"Assigned Boarding Gate: {assignedGate.GateName}");
            Console.WriteLine("===============================================");
        }
    }

    Console.WriteLine($"Total flights assigned automatically: {assignedFlights}");
    Console.WriteLine($"Total boarding gates assigned automatically: {assignedFlights}");

    double processedPercentage = (double)assignedFlights / Math.Max(1, unassignedFlightCount) * 100;
    Console.WriteLine($"Processed automatically: {processedPercentage:F2}%");
}

// Advanced feature (b) Belle
void DisplayTotalFeePerAirline(Terminal terminal)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("Total Fee Per Airline for the Day");
    Console.WriteLine("=============================================");

    double totalSubtotalFees = 0;
    double totalSubtotalDiscounts = 0;

    foreach (Airline airline in terminal.Airlines.Values)
    {
        double subtotalFees = airline.CalculateFees(); // Use method from Airline.cs
        double subtotalDiscounts = subtotalFees * 0.10; // Apply 10% discount

        Console.WriteLine(); // Extra line for spacing before each airline
        Console.WriteLine($"Airline: {airline.Name}");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine($"Subtotal Fees: ${subtotalFees}");
        Console.WriteLine($"Subtotal Discounts: -${subtotalDiscounts}");
        Console.WriteLine($"Final Fees for {airline.Name}: ${subtotalFees - subtotalDiscounts}");
        Console.WriteLine("----------------------------------------------------");
        Console.WriteLine(); // Extra line after each airline for better spacing

        totalSubtotalFees += subtotalFees;
        totalSubtotalDiscounts += subtotalDiscounts;
    }

    // Display overall totals
    Console.WriteLine("=============================================");
    Console.WriteLine("Overall Totals for the Day");
    Console.WriteLine("=============================================");
    Console.WriteLine($"Total Subtotal Fees: ${totalSubtotalFees}");
    Console.WriteLine($"Total Subtotal Discounts: -${totalSubtotalDiscounts}");
    Console.WriteLine($"Final Total Fees Terminal 5 Will Collect: ${totalSubtotalFees - totalSubtotalDiscounts}");

    double finalTotalFees = totalSubtotalFees - totalSubtotalDiscounts;
    if (finalTotalFees > 0)
    {
        Console.WriteLine($"Percentage of Discounts: {(totalSubtotalDiscounts / finalTotalFees) * 100:F2}%");
    }
    else
    {
        Console.WriteLine("No fees collected today.");
    }
}




// main program starts here
Terminal terminal = new Terminal("Changi Airport Terminal 5");
LoadAirlines(terminal);  
LoadBoardingGates(terminal);
LoadFlights(terminal);

while (true)
{
    try
    {
        DisplayMenu();

        Console.WriteLine("Please select your option:");
        string input = Console.ReadLine();

        if (!int.TryParse(input, out int option))
        {
            Console.WriteLine("Invalid input! Please enter a number between 1 and 9 or 0 to exit.");
            continue;
        }

        if (option == 1)
        {
            ListAllFlights(terminal);
        }
        else if (option == 2)
        {
            DisplayAllBoardingGates(terminal);
        }
        else if (option == 3)
        {
            AssignBoardingGate(terminal);
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
            ModifyFlightDetails(terminal);
        }
        else if (option == 7)
        {
            DisplayFlightBySchedule(terminal);
        }
        else if (option == 8)
        {
            ProcessUnassignedFlights(terminal);
        }
        else if (option == 9)
        {
            DisplayTotalFeePerAirline(terminal);
        }
        else if (option == 0)
        {
            Console.WriteLine("Goodbye!");
            break;
        }
        else
        {
            Console.WriteLine("Invalid option! Please enter a number between 1 and 9 or 0 to exit.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred: {ex.Message}. Please try again.");
    }
}

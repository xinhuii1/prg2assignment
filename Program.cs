//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

using prg2assignment;
using System.Diagnostics.CodeAnalysis;
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

// Basic feature 1) Load files (airlines and boarding gates)
// - load the airlines.csv file
// - create the Airline objects based on the data loaded
// - add the Airlines objects into an Airline Dictionary
// - load the boardinggates.csv file
// - create the Boarding Gate objects based on the data loaded
// - add the Boarding Gate objects into a Boarding Gate dictionary

Dictionary<string, Airline> airlines = new Dictionary<string, Airline>();
void LoadAirlines(Terminal terminal)
{
    if (!File.Exists("airlines.csv"))
    {
        Console.WriteLine("Error: airlines.csv file is missing!");
        return;
    }

    string[] csvLines = File.ReadAllLines("airlines.csv");

    for (int i = 1; i < csvLines.Length; i++) // Skip header
    {
        string[] data = csvLines[i].Split(',');

        if (data.Length != 2) // Ensure correct column count
        {
            Console.WriteLine($"Error: Invalid data on line {i + 1}: {csvLines[i]}");
            continue;
        }

        string name = data[0].Trim();
        string code = data[1].Trim();

        Airline newAirline = new Airline(name, code);

        // Call AddAirline method from Terminal class
        if (!terminal.AddAirline(newAirline))
        {
            Console.WriteLine($"Failed to add airline {name} with code {code}");
            Console.WriteLine($"Error: Airline code {code} already exists.");
        }
        else
        {
            Console.WriteLine($"Sucessessfully added airline {name} with code {code}");
        }
    }

    // Print the total airlines loaded
    Console.WriteLine($"Loaded {csvLines.Length - 1} airlines.");
    foreach (var airline in terminal.Airlines)
    {
        Console.WriteLine($"Airline Code: {airline.Key}, Name: {airline.Value.Name}");
    }
}


Dictionary<string, BoardingGate> boardingGates = new Dictionary<string, BoardingGate>();

void LoadBoardingGates(Terminal terminal)
{
    if (!File.Exists("boardinggates.csv"))
    {
        Console.WriteLine("Error: boardinggates.csv file is missing!");
        return;
    }


    string[] csvLines = File.ReadAllLines("boardinggates.csv");

    for (int i = 1; i < csvLines.Length; i++) // Skip header row
    {
        string[] data = csvLines[i].Split(',');

        if (data.Length != 4) // Ensure exactly 4 columns
        {
            Console.WriteLine($"Error: Invalid data on line {i + 1}: {csvLines[i]}");
            continue;

        }

        string gateName = data[0].Trim();
        bool supportsCFFT = Convert.ToBoolean(data[1].Trim());
        bool supportsDDJB = Convert.ToBoolean(data[2].Trim());
        bool supportsLWTT = Convert.ToBoolean(data[3].Trim());

        BoardingGate newGate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);

        // Call AddBoardingGate from Terminal and check for duplicates
        if (!terminal.AddBoardingGate(newGate))
        {
            Console.WriteLine($"Error: Boarding Gate {gateName} already exists in the dictionary.");
        }

        else
        {
            Console.WriteLine($"Successfully added boarding gate {gateName}");
        }
    }
}

// basic feature 2)	Load files (flights)
// - load the flights.csv file
// - create the Flight objects based on the data loaded
// - add the Flight objects into a Dictionary


// dictionary to store flight objects
Dictionary<string, Flight> flights = new Dictionary<string, Flight>();

// method to load the flight.csv flight
void LoadFlights(Terminal terminal)
{
    if (!File.Exists("flights.csv"))
    {
        Console.WriteLine("Error: flights.csv file is missing!");
        return;
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

            Airline airline = terminal.Airlines[airlineCode];

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
            Console.WriteLine($"Added flight {flightNumber} ({origin} → {destination})");
        }
    }
}




// basic feature 3)	List all flights with their basic information
// - display the Basic Information of all Flights, which are the 5 flight specifications (i.e. Flight Number, Airline Name, Origin, Destination, and Expected Departure/Arrival Time)

// method to display all the flights information
void ListAllFlights(Terminal terminal)
{
    if (terminal.Flights.Count == 0)
    {
        Console.WriteLine("No flights to display, the list is empty.");
        return;
    }

    Console.WriteLine("=============================================");
    Console.WriteLine("List of All Flights");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected Departure/Arrival Time");

    foreach (Flight flight in terminal.Flights.Values)
    {
        string airlineName = "Not Available";
        string airlineCode = flight.FlightNumber.Substring(0, 2); // extract the first 2 characters

        if (terminal.Airlines.ContainsKey(airlineCode))
        {
            airlineName = terminal.Airlines[airlineCode].Name;
        }

        Console.WriteLine("{0, -15} {1, -25} {2, -20} {3, -20} {4}", flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime.ToString("dd/MM/yyyy h:mm:ss tt").ToLower());
    }
}


// 5)	Assign a boarding gate to a flight
// - prompt the user for the Flight Number
// - display the basic information of the selected Flight, including the Special Request Code (if any)
// - prompt the user for the Boarding Gate
// - check that the selected Boarding Gate is not assigned to another Flight (Note: For Basic Features, there is no need to validate if the Special Request Codes between Flights and Boarding Gates match)
//  - if the Boarding Gate selected is already assigned to another flight, display a message that the Boarding Gate is already assigned and repeat the previous step
// - display the basic information of the selected Flight, Special Request Code (if any), and Boarding Gate entered
// - prompt the user if they would like to update the Status of the Flight, with a new Status of any of the following options: “Delayed”, “Boarding”, or “On Time” [Y] or set the Status of the Flight to the default of “On Time” and continue to the next step if [N]
// - dsplay a message to indicate a successful Boarding Gate assignment

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

    //if (!boardingGates.ContainsKey(gateName))
   

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



// main program starts here
Terminal terminal = new Terminal("Changi Airport Terminal 5");
LoadAirlines(terminal);  // load airline method
LoadFlights(terminal); // pass airline to loadflight method
LoadBoardingGates(terminal);    // load boarding gate method


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

    else if (option == 3)
    {
        AssignBoardingGate(terminal);
        Console.WriteLine();  // leave a new line
    }

    else if (option == 4)
    {
        // Call this when user selects option 2 (List Boarding Gates)
    }

}





//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

using prg2assignment;
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

//feature 1


Dictionary <string, Airline> airlines = new Dictionary<string, Airline> ();
void LoadAirlines(Dictionary<string, Airline> airlines)
{
    Console.WriteLine("Debug: Checking if airlines.csv exists...");

    if (!File.Exists("airlines.csv"))
    {
        Console.WriteLine("Error: airlines.csv file is missing!");
        return;
    }

    Console.WriteLine("Debug: Reading airlines.csv...");
    string[] csvLines = File.ReadAllLines("airlines.csv");

    Console.WriteLine($"Debug: Total lines read from airlines.csv = {csvLines.Length}");

    for (int i = 1; i < csvLines.Length; i++) // Skip header
    {
        Console.WriteLine($"Debug: Processing line {i + 1}: {csvLines[i]}");

        string[] data = csvLines[i].Split(',');

        if (data.Length != 2) // Ensure correct column count
        {
            Console.WriteLine($"Error: Invalid data on line {i + 1}: {csvLines[i]}");
            continue;
        }

        string name = data[0].Trim();
        string code = data[1].Trim();

        try
        {
            Airline newAirline = new Airline(name, code);
            airlines.Add(code, newAirline);
            Console.WriteLine($"Debug: Added airline {name} ({code})");
        }
        catch (ArgumentException)
        {
            Console.WriteLine($"Error: Airline code {code} already exists.");
        }
    }
}


void LoadBoardingGates(Terminal terminal)
{
    string[] csvLines = File.ReadAllLines("boardinggates.csv");

    for (int i = 1; i < csvLines.Length; i++) // Skip the header row
    {
        string[] data = csvLines[i].Split(',');

        if (data.Length == 4) // Ensure the row has exactly 4 columns
        {
            string gateName = data[0].Trim();           // Gate Name
            bool supportsCFFT = Convert.ToBoolean(data[1].Trim()); // Support for CFFT
            bool supportsDDJB = Convert.ToBoolean(data[2].Trim()); // Support for DDJB
            bool supportsLWTT = Convert.ToBoolean(data[3].Trim()); // Support for LWTT

            BoardingGate newGate = new BoardingGate(gateName, supportsCFFT, supportsDDJB, supportsLWTT);

            // Call AddBoardingGate and log only if the gate already exists
            if (!terminal.AddBoardingGate(newGate))
            {
                Console.WriteLine($"Boarding Gate {gateName} already exists in the dictionary.");
            }
        }
        else
        {
            Console.WriteLine($"Invalid data on line {i + 1}: {csvLines[i]}");
        }
    }
}



// basic feature 2)	Load files (flights)
// - load the flights.csv file
// - create the Flight objects based on the data loaded
// - add the Flight objects into a Dictionary


// dictionary to store flight objects
Dictionary<string, Flight> flightDict = new Dictionary<string, Flight>();

// method to load the flight.csv flight
void LoadFlights(Dictionary<string, Flight> flightDict, Dictionary<string, Airline> airlines)
{
    Console.WriteLine("Debug: Checking if flights.csv exists...");

    if (!File.Exists("flights.csv"))
    {
        Console.WriteLine("Error: flights.csv file is missing!");
        return;
    }

    Console.WriteLine("Debug: Reading flights.csv...");
    string[] csvLines = File.ReadAllLines("flights.csv");

    Console.WriteLine($"Debug: Total lines read from flights.csv = {csvLines.Length}");

    for (int i = 1; i < csvLines.Length; i++) // Skip header
    {
        Console.WriteLine($"Debug: Processing line {i + 1}: {csvLines[i]}");

        string[] data = csvLines[i].Split(',');

        if (data.Length < 5) // Ensure at least 5 columns
        {
            Console.WriteLine($"Error: Invalid data on line {i + 1}: {csvLines[i]}");
            continue;
        }

        string flightNumber = data[0].Trim();
        string origin = data[1].Trim();
        string destination = data[2].Trim();

        if (!DateTime.TryParse(data[3], out DateTime expectedTime))
        {
            Console.WriteLine($"Error: Invalid date format for flight {flightNumber} at line {i + 1}.");
            continue;
        }

        string specialRequestCode = data[4].Trim();
        string airlineCode = flightNumber.Substring(0, 2);

        if (!airlines.ContainsKey(airlineCode))
        {
            Console.WriteLine($"Error: Airline code {airlineCode} not found for flight {flightNumber} at line {i + 1}.");
            continue;
        }

        Airline airline = airlines[airlineCode];

        Flight flight;
        switch (specialRequestCode)
        {
            case "CFFT":
                flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
                break;
            case "LWTT":
                flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
                break;
            case "DDJB":
                flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 250);
                break;
            default:
                flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                break;
        }

        flight.Airline = airline;
        flightDict.Add(flightNumber, flight);
        Console.WriteLine($"Debug: Added flight {flightNumber} ({origin} → {destination})");
    }
}


// basic feature 3)	List all flights with their basic information
// - display the Basic Information of all Flights, which are the 5 flight specifications (i.e. Flight Number, Airline Name, Origin, Destination, and Expected Departure/Arrival Time)

void ListAllFlights(Dictionary<string, Flight> flightsDict)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("List of All Flights");
    Console.WriteLine("=============================================");
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20} {4, -20}", "Flight Number", "Airline Name", "Origin", "Destination", "Expected ");
    Console.WriteLine("{0, -20}", "Departure/Arrival Time");

    foreach (Flight flight in flightDict.Values)
    {
        string airlineName = flight.Airline != null ? flight.Airline.Name : "Unknown Airline";  // Display the airline name
        Console.WriteLine("{0, -15} {1, -25} {2, -20} {3, -20} {4}", flight.FlightNumber, airlineName, flight.Origin, flight.Destination, flight.ExpectedTime);
    }
}

// 5)	Assign a boarding gate to a flight
// - prompt the user for the Flight Number
// - display the basic information of the selected Flight, including the Special Request Code (if any)
// - prompt the user for the Boarding Gate
// - check that the selected Boarding Gate is not assigned to another Flight (Note: For Basic Features, there is no need to validate if the Special Request Codes between Flights and Boarding Gates match)
//   - if the Boarding Gate selected is already assigned to another flight, display a message that the Boarding Gate is already assigned and repeat the previous step
// - display the basic information of the selected Flight, Special Request Code (if any), and Boarding Gate entered
// - prompt the user if they would like to update the Status of the Flight, with a new Status of any of the following options: “Delayed”, “Boarding”, or “On Time” [Y] or set the Status of the Flight to the default of “On Time” and continue to the next step if [N]
// - dsplay a message to indicate a successful Boarding Gate assignment

void AssignBoardingGate(Dictionary<string, Flight> flightDict)
{
    Console.WriteLine("=============================================");
    Console.WriteLine("Assign a Boarding Gate to a Flight");
    Console.WriteLine("=============================================");

    // prompt user for the flight number
    Console.WriteLine("Enter Flight Number:");
    string flightNo = Console.ReadLine().ToUpper();  // ensure the flight number is in uppercase

    // check if the flight number exists
    if (!flightDict.ContainsKey(flightNo))
    {
        Console.WriteLine($"Flight with number {flightNo} is not found.");
        return; //exit if the flight is not found
    }

    // prompt user for the boarding gate name
    Console.WriteLine("Enter Boarding Gate Name: ");
    string gateName = Console.ReadLine().ToUpper();  // ensure the boarding gate is in upper case

    //if (!boardingGates.ContainsKey(gateName))
   

    Flight selectedFlight = flightDict[flightNo];

    // display flight information
    Console.WriteLine($"Flight Number: {selectedFlight.FlightNumber}");
    Console.WriteLine($"Origin: {selectedFlight.Origin})");
    Console.WriteLine($"Destination: {selectedFlight.Destination}");
    Console.WriteLine($"Expected Time: {selectedFlight.ExpectedTime}");
    Console.WriteLine($"Special Request Code : {selectedFlight.Status}");
    //gConsole.WriteLine($"Boarding Gate Name: {BoardingGate.gate}");
    Console.WriteLine("Supports DDJB: {0}", selectedFlight is DDJBFlight);
    Console.WriteLine("Supports CFFT: {0}", selectedFlight is CFFTFlight);
    Console.WriteLine("Supports LWTT: {0}", selectedFlight is LWTTFlight);

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

        Console.WriteLine("Flight {0} has been assigned to Boarding Gate {1}!", selectedFlight.FlightNumber, gateName);
    }

    else
    {
        Console.WriteLine("Flight Number {0} not found. Please try again!", flightNo);
    }



    // main program starts here
    LoadAirlines(airlines);  // load airline method
    LoadFlights(flightDict, airlines); // pass airline to loadflight method

    
}

while (true)
{

    DisplayMenu();

    // get user input for option
    Console.WriteLine("Please select your option:");
    int option = Convert.ToInt32(Console.ReadLine());

    if (option == 1)
    {
        ListAllFlights(flightDict);
        Console.WriteLine(); //leave a new line
    }

    if (option == 3)
    {
        AssignBoardingGate(flightDict);
    }
}



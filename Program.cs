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
    string[] csvLines = File.ReadAllLines("airlines.csv");

    for (int i = 1; i < csvLines.Length; i++) // Skip the header row
    {
        string[] data = csvLines[i].Split(',');

        if (data.Length == 2) // Ensure the row has exactly 2 columns
        {
            string name = data[0].Trim(); // Airline Name
            string code = data[1].Trim(); // Airline Code

            try
            {
                Airline newAirline = new Airline(name, code); // Create Airline object
                airlines.Add(code, newAirline);              // Add to dictionary
                Console.WriteLine($"Airline {name} with code {code} created and added.");
            }
            catch (ArgumentException)
            {
                Console.WriteLine($"Airline with code {code} already exists in the dictionary.");
            }
        }
        else
        {
            Console.WriteLine($"Invalid data on line {i + 1}: {csvLines[i]}");
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
void LoadFlights(Dictionary <string, Flight> flightDict, Dictionary<string, Airline> airlines)
{
    string[] csvLines = File.ReadAllLines("flights.csv");

    for ( int i = 1;  i < csvLines.Length; i++ ) // skip the header row 
    {
        string[] data = csvLines[i].Split(',');

        if ( data.Length >= 4) // 
        {
            string flightNumber = data[0];
            string origin = data[1];
            string destination = data[2];
            if (!DateTime.TryParse(data[3], out DateTime expectedTime))
            {
                Console.WriteLine($"Invalid time format for this flight number: {flightNumber}");
                continue; 
            }

            string specialRequestCode = data[4];
            string airlineCode = flightNumber.Substring(0, 2);   // extract the first 2 characters of the flight number

            if (!airlines.ContainsKey(airlineCode))
            {
                Console.WriteLine($"Airline with code {airlineCode} is not found for the flight {flightNumber}.");
                continue; // skip this flight if airline is not found
            }

            //find the corresponding airline object
            Airline airline = airlines[airlineCode];

            Flight flight;
            if (specialRequestCode == "CFFT")
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

            flight.Airline = airline; // link the airline to the flight

            flightDict.Add(flightNumber, flight);
        }
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

// main program starts here
LoadAirlines(airlines);  // load airline method
LoadFlights(flightDict, airlines); // pass airline to loadflight method

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
    }
}



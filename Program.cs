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
void LoadFlights(Dictionary <string, Flight> flightDict)
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


            if (specialRequestCode == "CFFT")
            {
                CFFTFlight flight = new CFFTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 150);
                flightDict.Add(flightNumber, flight); // add to directory (flight number as the key)
            }

            else if (specialRequestCode == "LWTT")
            {
                LWTTFlight flight = new LWTTFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 500);
                flightDict.Add(flightNumber, flight); // add to directory
            }

            else if (specialRequestCode == "DDJB")
            {
                DDJBFlight flight = new DDJBFlight(flightNumber, origin, destination, expectedTime, "Scheduled", 250);
                flightDict.Add(flightNumber, flight); // add to directory
            }

            else
            {
                NORMFlight flight = new NORMFlight(flightNumber, origin, destination, expectedTime, "Scheduled");
                flightDict.Add(flightNumber, flight); // add to directory
            }
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
    Console.WriteLine("{0,-15} {1,-25} {2,-20} {3, -20}", "Flight Number", "Origin", "Destination", "Expected ");
    Console.WriteLine("{0, -20}", "Departure/Arrival Time");

    foreach (Flight flight in flightDict.Values)
    {
        Console.WriteLine(flight.ToString());
    }
}

//FEATURE 4
void DisplayBoardingGates(Dictionary<string, BoardingGate> boardingGates)
{
    Console.WriteLine("\n=============================================");
    Console.WriteLine("List of Boarding Gates for Changi Airport Terminal 5");
    Console.WriteLine("=============================================");
    Console.WriteLine("Gate Name\tDDJB\tCFFT\tLWTT");

    foreach (var gate in boardingGates.Values)
    {
        Console.WriteLine(gate.ToString()); 
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
    if (flightDict.ContainsKey(flightNo))
    {
        Flight selectedFlight = flightDict[flightNo];

        // prompt user for the boarding gate name
        Console.WriteLine("Enter Boarding Gate Name: ");
        string gateName = Console.ReadLine().ToUpper(); // ensure the boarding gate name is in uppercase

        // check if the boarding gate is already assigned to another flight no or not
        if (selectedFlight.BoardingGate != null)
        {
            // message to display if the boarding gate is assigned already
            Console.WriteLine("Boarding Gate {0} is already assigned to Flight {1}. Please choose another gate.", selectedFlight.BoardingGate, selectedFlight.FlightNumber);
            return; // return if the gate is already assigned
        }

        // assign the boarding gate
        selectedFlight.BoardingGate = gateName;

        // display flight information
        Console.WriteLine("Flight Number: {0}", selectedFlight.FlightNumber);
        Console.WriteLine("Origin: {0}", selectedFlight.Origin);
        Console.WriteLine("Destination: {0}", selectedFlight.Destination);
        Console.WriteLine("Expected Time: {0}", selectedFlight.ExpectedTime);
        Console.WriteLine("Special Request Code : {0}", selectedFlight.Status);
        Console.WriteLine("Boarding Gate Name: {0}", selectedFlight.BoardingGate);
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
    }

}



// main program starts here
LoadFlights(flightDict);
while (true)
{
    DisplayMenu();

    // get user input for option
    Console.WriteLine("Please select your option:");
    int option = Convert.ToInt32(Console.ReadLine());

    if (option == 1)
    {
        ListAllFlights(flightDict);
    }

    if (option == 3)
    {
        AssignBoardingGate(flightDict);
    }
}



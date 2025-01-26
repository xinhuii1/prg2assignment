//==========================================================
// Student Number	: S10269252
// Student Name	: Han Xin Hui
// Partner Name	: Belle Chong Qing Xi
//==========================================================

using prg2assignment;

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
}



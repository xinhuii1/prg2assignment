
using prg2assignment;

//Feature 1

void savingsAccCollection(Dictionary<string, Airline> airlines)
{
    string[] csvLines = File.ReadAllLines("airlines.csv");
    for (int i = 1; i < csvLines.Length; i++)                                // Skip the header row
    {
        string[] line = csvLines[i].Split(',');
        string code = line[0].Trim(); 
        string name = line[1].Trim(); 

        Airline airline = new Airline(name, code);
        airlines[code] = airline;
    }
}

void LoadBoardingGateData(Dictionary<string, BoardingGate> boardingGates)
{
    string[] csvLines = File.ReadAllLines("boardinggates.csv");

    for (int i = 0; i < csvLines.Length; i++) 
    {
        string[] fields = csvLines[i].Split(',');

        string gateName = fields[0].Trim();
        bool supportsDDJB = Convert.ToBoolean(fields[1].Trim());
        bool supportsCFFT = Convert.ToBoolean(fields[2].Trim());
        bool supportsLWTT = Convert.ToBoolean(fields[3].Trim());

        BoardingGate gate = new BoardingGate(gateName, supportsDDJB, supportsCFFT, supportsLWTT);
        boardingGates[gateName] = gate;
    }
}
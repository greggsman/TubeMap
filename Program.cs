using System.Linq.Expressions;

class TubeMap { // adjacency list graph
    private Dictionary<string, List<string>> LinesAndStations = new Dictionary<string, List<string>>();
    public List<string> stations = new List<string>();
    private bool[,] adjacencyMatrix;
    public TubeMap() {
        LinesAndStations.Add("Northern line via charring cross", 
            new List<string>() {"Euston", "Warren Street", "Goodge Street", "Tottenham Court Road", "Leicester Square"});
        LinesAndStations.Add("Nothern Line via Bank",
            new List<string>() {"Euston", "King's Cross", "Angel", "Old Street", "Moorgate", "Bank"});
        LinesAndStations.Add("Victoria",
            new List<string>() {"King's Cross", "Euston", "Warren Street", "Oxford Circus", "Green Park"});
        LinesAndStations.Add("Piccadilly",
            new List<string>(){"King's Cross", "Russel Square", "Holborn", "Covent Garden", "Leicester Square", "Piccadilly Circus", "Green Park"});
        LinesAndStations.Add("Metropolitan", // for simplicity, this includes hammersmith and city and circle
            new List<string>() {"Baker Street", "Great Portland Street", "Euston Square", "King's Cross", "Farringdon", "Barbican", "Moorgate"});
        LinesAndStations.Add("Jubilee",
            new List<string>() {"St John's Wood", "Baker Street", "Bond Street", "Green Park"});
        LinesAndStations.Add("Central",
            new List<string>() {"Bond Street", "Oxford Circus", "Tottenham Court Road", "Holborn", "Chancery Lane", "St Paul's", "Bank"});
        LinesAndStations.Add("Thameslink",
            new List<string>() {"King's Cross", "Farrigdon", "City Thameslink"});
        
        foreach(KeyValuePair<string, List<string>> kvp in LinesAndStations){
            foreach(string station in kvp.Value){
                if(!stationsListContains(station)){
                    stations.Add(station);
                }
            }
        }
        adjacencyMatrix = new bool[stations.Count, stations.Count];
        foreach(KeyValuePair<string, List<string>> kvp in LinesAndStations){
            for(int i = 0; i < kvp.Value.Count - 1; i++){
                SetConnection(kvp.Value[i], kvp.Value[i + 1]);
            }
        }
    }

    public bool stationsListContains(string station){
        foreach(string str in stations){
            if (str == station){
                return true;
            }
        }
        return false;
    }
    private int FindIndexOfStation(string station){
        for(int i = 0 ; i < stations.Count; i++){
            if(stations[i] == station){
                return i;
            }
        }
        return -1;
    }
    private void SetConnection(string station1, string station2){
        try{
            int station1Index = FindIndexOfStation(station1);
            int station2Index = FindIndexOfStation(station2);
            adjacencyMatrix[station1Index, station2Index] = true;
            adjacencyMatrix[station2Index, station1Index] = true;
        }
        catch{
            Console.WriteLine("Station doesn't exist");
        }
    }
    public bool GetConnection(string station1, string station2){
        try{
            return adjacencyMatrix[FindIndexOfStation(station1), FindIndexOfStation(station2)];
        }
        catch{
            return false;
        }
    }
}
class Program{
    static void Main(string[] args){
        TubeMap tubeMap = new TubeMap();
    }
}
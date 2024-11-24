using System.Collections.Specialized;
using System.Linq.Expressions;

class TubeMap { // adjacency list graph
    private Dictionary<string, List<string>> LinesAndStations = new Dictionary<string, List<string>>();
    public List<string> stations = new List<string>();
    private string[,] adjacencyMatrix;
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
        adjacencyMatrix = new string[stations.Count, stations.Count];
        for(int i = 0; i < stations.Count; i++){ // default empty value
            for(int j = 0; j < stations.Count; j++){
                adjacencyMatrix[i, j] = "";
            }
        }
        foreach(KeyValuePair<string, List<string>> kvp in LinesAndStations){
            for(int i = 0; i < kvp.Value.Count - 1; i++){
                SetConnection(kvp.Value[i], kvp.Value[i + 1], kvp.Key);
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
    private int GetNodeIndex(string station){
        for(int i = 0 ; i < stations.Count; i++){
            if(stations[i] == station){
                return i;
            }
        }
        return -1;
    }
    private void SetConnection(string station1, string station2, string line){
        try{
            int station1Index = GetNodeIndex(station1);
            int station2Index = GetNodeIndex(station2);
            adjacencyMatrix[station1Index, station2Index] = line;
            adjacencyMatrix[station2Index, station1Index] = line;
        }
        catch{
            Console.WriteLine("Station doesn't exist");
        }
    }
    public string GetConnection(string station1, string station2){
        try{
            return adjacencyMatrix[GetNodeIndex(station1), GetNodeIndex(station2)];
        }
        catch{
            return "";
        }
    }

    private bool[] visited;
    private int[] stationCountFromStart;
    private string[] previousStation;
    public void Dijkstra(string start, string destination){
        if(GetNodeIndex(start) == -1 || GetNodeIndex(destination) == -1){
            Console.WriteLine("These nodes aren't on the graph");
            return;
        }
        // define visited, distance from start and previous vertex arrays
        visited = new bool[stations.Count];
        stationCountFromStart = new int[stations.Count];
        previousStation = new string[stations.Count];
        // populate arrays
        for(int i = 0; i < stations.Count; i++){
            visited[i] = false;
            if(stations[i] == start){
                stationCountFromStart[i] = 0;
            }
            else{
                stationCountFromStart[i] = int.MaxValue; // equivalent to infinity in this case
            }
            previousStation[i] = "";
        }
        int destinationIndex = GetNodeIndex(destination);
        string nodeToVisit = start;
        // fill the arrays with shortest paths and 
        while(!visited[destinationIndex]){
            Visit(nodeToVisit); 
            int shortestDistanceIndex = 0;
            int currentSmallestDistance = int.MaxValue;
            for(int i = 0; i < stations.Count; i++){
                if(stationCountFromStart[i] > 0){
                    if(stationCountFromStart[i] > 0){
                        if(stationCountFromStart[i] < currentSmallestDistance && !visited[i]){
                            currentSmallestDistance = stationCountFromStart[i];
                            shortestDistanceIndex = i;
                        }
                    }
                }
            }
            nodeToVisit = stations[shortestDistanceIndex];
        }
        string shortestPath = DetermineShortestPath(start, destination);
        Console.WriteLine();
        string[] shortestPathArray = shortestPath.Split(',');
        string currentLine = "";
        string previousLine = "";
        for(int i = 0; i < shortestPathArray.Length - 1; i++){
            previousLine = currentLine;
            currentLine = GetConnection(shortestPathArray[i], shortestPathArray[i + 1]);
            if(previousLine != currentLine){
                shortestPathArray[i] += ", Line: " + currentLine;
            }
            Console.WriteLine(shortestPathArray[i]);
        }
        Console.WriteLine(shortestPathArray[shortestPathArray.Length - 1]);
    }
    private void Visit(string node){ // visit one station, calculate the shortest distance from the start
        int nodeIndex = GetNodeIndex(node);
        visited[nodeIndex] = true; // mark this station as visited
        for(int i = 0; i < stations.Count; i++){ 
            string connected = GetConnection(stations[i], stations[nodeIndex]);
            if(!visited[i] && connected != ""){
                if(stationCountFromStart[nodeIndex] + 1 < stationCountFromStart[i]){
                    // if the route via the current station is less than the current root from the start, change the root to go via the current node
                    stationCountFromStart[i] = stationCountFromStart[nodeIndex] + 1;
                    previousStation[i] = node;
                }
            }
        }
    }
    private string DetermineShortestPath(string start, string destination) {
        if(start == destination){
            return destination;
        }
        string previousStationName = previousStation[GetNodeIndex(destination)];
        return DetermineShortestPath(start, previousStationName) + "," + destination;
    }
    // goes to the destination then works backwards
}
class Program{
    static void Main(string[] args){
        TubeMap tubeMap = new TubeMap();
        bool loop = true;
        while(loop){
            Console.WriteLine("What station are you leaving from?");
            string startNode = Console.ReadLine();
            Console.WriteLine("What is your destination?");
            string endNode = Console.ReadLine();
            tubeMap.Dijkstra(startNode, endNode);
            Console.WriteLine("Enter another destination? (y/n)");
            string input = Console.ReadLine();
            if(!(input.ToUpper() == "Y")){
                loop = false;
            }
        }
    }
}
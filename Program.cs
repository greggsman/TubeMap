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
    private int GetNodeIndex(string station){
        for(int i = 0 ; i < stations.Count; i++){
            if(stations[i] == station){
                return i;
            }
        }
        return -1;
    }
    private void SetConnection(string station1, string station2){
        try{
            int station1Index = GetNodeIndex(station1);
            int station2Index = GetNodeIndex(station2);
            adjacencyMatrix[station1Index, station2Index] = true;
            adjacencyMatrix[station2Index, station1Index] = true;
        }
        catch{
            Console.WriteLine("Station doesn't exist");
        }
    }
    public bool GetConnection(string station1, string station2){
        try{
            return adjacencyMatrix[GetNodeIndex(station1), GetNodeIndex(station2)];
        }
        catch{
            return false;
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
        Console.WriteLine("To get from {0} to {1}:\n{2}", start, destination, DetermineShortestPath(start, destination));
    }
    private void Visit(string node){ // visit one station, calculate the shortest distance from the start
        int nodeIndex = GetNodeIndex(node);
        visited[nodeIndex] = true; // mark this station as visited
        for(int i = 0; i < stations.Count; i++){ 
            bool connected = GetConnection(stations[i], stations[nodeIndex]);
            if(!visited[i] && connected){
                if(stationCountFromStart[nodeIndex] + 1 < stationCountFromStart[i]){
                    // if the route via the current station is less than the current root from the start, change the root to go via the current node
                    stationCountFromStart[i] = stationCountFromStart[nodeIndex] + 1;
                    previousStation[i] = node;
                }
            }
        }
    }
    private string currentLine = "";
    private string DetermineShortestPath(string start, string destination){
        string previousStationName = previousStation[GetNodeIndex(destination)];
        if(start == destination){
            return destination;
        }
        foreach(KeyValuePair<string, List<string>> kvp in LinesAndStations){
            List<string> line = kvp.Value;
            for(int i = 1; i < line.Count; i++){
                if(line[i] == destination && line[i-1] == previousStationName){
                    currentLine = kvp.Key;
                }
            }
        }
        return DetermineShortestPath(start, previousStationName) + " " + currentLine + "\n" + destination;
    }
    // goes to the destination then works backwards
}
class Program{
    static void Main(string[] args){
        TubeMap tubeMap = new TubeMap();
        Console.WriteLine("What is the start node?");
        string startNode = Console.ReadLine();
        Console.WriteLine("What is the end node?");
        string endNode = Console.ReadLine();

        tubeMap.Dijkstra(startNode, endNode);
    }
}
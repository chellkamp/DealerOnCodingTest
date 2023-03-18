using System.Runtime.CompilerServices;
using TrainsLib;

namespace WebApp.Util
{
    /// <summary>
    /// Provides a bridge between the results of TrackGraph operations and what will be displayed to the user.
    /// </summary>
    public class GraphDisplayWrapper
    {
        private static readonly string _NO_SUCH_ROUTE_MSG = "NO SUCH ROUTE";

        public TrackGraph Graph { get; private set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="graph">graph to wrap</param>
        public GraphDisplayWrapper(TrackGraph graph)
        {
            Graph = graph;
        }

        public DisplayInfo GetDistanceOfExactRoute(IList<char> route)
        {
            List<int> convertedRoute = route.Select<char, int>(s => GetStationIndex(s)).ToList();
            
            int result = Graph.GetDistanceOfExactRoute(convertedRoute);
            string displayResult = result == -1 ? _NO_SUCH_ROUTE_MSG : result.ToString();

            string description = $"Distance of route {string.Join('-', route)}";

            return new DisplayInfo(displayResult, description);
        }

        public DisplayInfo GetNumRoutesMaxStops(char source, char destination, int maxStops)
        {
            int result = Graph.GetNumRoutesMaxStops(
                GetStationIndex(source),
                GetStationIndex(destination),
                maxStops
            );

            string description = $"Number of trips starting at {source} and ending at {destination} " +
                $"with a maximum of {maxStops} " + (maxStops == 1 ? "stop" : "stops");

            return new DisplayInfo(result.ToString(), description);
        }

        public DisplayInfo GetNumRoutesLessThanDistance(char source, char destination, int maxDistance)
        {
            int result = Graph.GetNumRoutesLessThanDistance(
                GetStationIndex(source),
                GetStationIndex(destination),
                maxDistance
            );

            string description = $"Number of trips starting at {source} and ending at {destination} " +
                $"with a distance of less than {maxDistance}";

            return new DisplayInfo(result.ToString(), description);
        }

        public DisplayInfo GetNumRoutesExactStops(char source, char destination, int stops)
        {
            int result = Graph.GetNumRoutesExactStops(
                GetStationIndex(source),
                GetStationIndex(destination),
                stops
            );

            string description = $"Number of trips starting at {source} and ending at {destination} " +
                $"with exactly {stops} " + (stops == 1 ? "stop" : "stops");

            return new DisplayInfo(result.ToString(), description);
        }

        public DisplayInfo GetShortestDistance(char source, char destination)
        {
            int result = Graph.GetShortestDistance(GetStationIndex(source), GetStationIndex(destination));
            string displayResult = result == -1 ? _NO_SUCH_ROUTE_MSG : result.ToString();

            string description = $"Length of the shortest route (by total distance) from {source} to {destination}";

            return new DisplayInfo(displayResult, description);
        }

        /// <summary>
        /// Converts a station letter to an index for a graph object.
        /// </summary>
        /// <param name="c">station letter.  This must be an uppercase letter ('A'-'Z').</param>
        /// <returns>index of station in a graph</returns>
        public static int GetStationIndex(char c)
        {
            return c - 'A';
        }


        public class DisplayInfo
        {
            public string Result { get; private set; }

            public string Description { get; private set; }

            public DisplayInfo(string result, string description)
            {
                Result = result;
                Description = description;
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Formats.Asn1.AsnWriter;

namespace TrainsLib
{
    /// <summary>
    /// Contains directed linkages between towns.
    /// </summary>
    public class TrackGraph
    {

        #region Fields and Properties

        // We store the distances in a matrix of values.
        // For each value of the matrix:
        //   1) The first index, or row, is the index of the source.
        //   2) The second index, or column, is the index of the destination.
        //   3) The value is the distance from the source to the destination.
        //      A value of 0 indicates that there is no linkage.
        //
        //   For any two stations, x and y, there is at most one track from station x to station y.
        //   Assuming distance is an integer for simplicity.
        private Dictionary<int, int>[] _tracks;

        /// <summary>
        /// Gets the number of stations this graph is keeping track of.
        /// </summary>
        public int StationCount
        {
            get { return _tracks.Length; }
        }

        #endregion Fields and Properties

        #region Constructors

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="numStations">num</param>
        public TrackGraph(int numStations) {
            _tracks = new Dictionary<int, int>[numStations];
            for (int i = 0; i < numStations; i++)
            {
                _tracks[i] = new Dictionary<int, int>();
            }
        }

        #endregion Constructors

        #region Methods

        /// <summary>
        /// Gets the distance for a given valid source and destination.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <returns>value of entry, if found.  Otherwise, 0</returns>
        private int GetEntry(int source, int destination)
        {
            Dictionary<int, int> connections = _tracks[source];

            int distance = 0;
            connections.TryGetValue(destination, out distance);

            return distance;
        }

        /// <summary>
        /// Tests whether a given integer is a valid station index in this graph.
        /// </summary>
        /// <param name="index">index to test</param>
        public bool IsValidStationIndex(int index)
        {
            return 0 <= index && index < _tracks.Length;
        }

        /// <summary>
        /// Set a direct, one-way route between two stations by specifying the distance
        /// from the source to the destination.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <param name="distance">distance.  Enter 0 to clear the route</param>
        /// <exception cref="ArgumentException"></exception>
        public void SetDirectTrackDistance(int source, int destination, int distance)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            if (distance < 0)
            {
                throw new ArgumentException($"Invalid distance: {distance}");
            }

            Dictionary<int, int> connections = _tracks[source];
            if (distance > 0)
            {
                connections[destination] = distance;
            }
            else
            {
                connections.Remove(destination);
            }
        }

        /// <summary>
        /// Get the length of the direct, one-way route from one station to another, if the route exists.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <returns>
        /// If a direct, one-way route exists, the distance to travel from the source to the destination.
        /// If a direct, one-way route does not exist, 0.
        /// </returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetDirectTrackDistance(int source, int destination)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            return GetEntry(source, destination);
        }

        /// <summary>
        /// Given an ordered list of station IDs that specify an exact route, calculate the total distance.
        /// </summary>
        /// <param name="route">route</param>
        /// <returns>
        /// If the route exists, the total distance traveled along that route.  Otherwise, -1.
        /// If the list is empty, this will also be -1.
        /// </returns>
        public int GetDistanceOfExactRoute(IList<int> route)
        {
            int retVal = 0;
            IEnumerator<int> iter = route.GetEnumerator();
            bool routeExists = iter.MoveNext();
            if (routeExists)
            {
                int prev = iter.Current;

                while (routeExists && iter.MoveNext())
                {
                    int cur = iter.Current;

                    int dist = GetDirectTrackDistance(prev, cur);
                    if (dist > 0)
                    {
                        retVal += dist;
                    }
                    else
                    {
                        routeExists = false;
                    }

                    prev = cur;
                }
            }

            return routeExists ? retVal : -1;
        }

        /// <summary>
        /// Gets the number of routes from the source to the destination,
        /// with no more than the specified number of intermediate stops.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <param name="maxStops">maximum number of stops, including final station</param>
        /// <returns>Number of routes.  May be 0.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetNumRoutesMaxStops(int source, int destination, int maxStops)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            if (maxStops < 1)
            {
                throw new ArgumentException($"Max stops must be at least 1. maxStops = {maxStops}");
            }

            return GetNumRoutesDFS(
                source,
                t => t.Station == destination,
                t => t.NumStops <= maxStops
            );
        }

        /// <summary>
        /// Gets the number of routes from a source station to a destination station,
        /// with a track distance less than the one specified.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <param name="maxDistance">Maximum route length. Must be greater than 0.</param>
        /// <returns>Number of routes.  May be 0.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetNumRoutesLessThanDistance(int source, int destination, int maxDistance)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            if (maxDistance <= 0)
            {
                throw new ArgumentException($"Max distance must be greater than 0.  maxDistance = {maxDistance}");
            }

            return GetNumRoutesDFS(
                source,
                t => t.Station == destination,
                t => t.DistanceTraveled < maxDistance
            );
        }

        /// <summary>
        /// Gets the number of routes from a source to a destination that have exactly the given number of
        /// stops in between.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <param name="stops">Exact number of stops between stations.  May not be less than 1.</param>
        /// <returns>Number of routes.  May be 0.</returns>
        /// <exception cref="ArgumentException"></exception>
        public int GetNumRoutesExactStops(int source, int destination, int stops)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            if (stops < 1)
            {
                throw new ArgumentException($"Number of stops must be 1 or greater.  stops = {stops}");
            }

            return GetNumRoutesDFS(
                source,
                t => t.Station == destination && t.NumStops == stops,
                t => t.NumStops <= stops
            );
        }

        /// <summary>
        /// Gets the shortest distance from the source to the destination.
        /// </summary>
        /// <param name="source">source station index</param>
        /// <param name="destination">destination station index</param>
        /// <returns>If any routes exist, the length of the shortest route.  If no routes exist, -1.</returns>
        public int GetShortestDistance(int source, int destination)
        {
            if (!IsValidStationIndex(source))
            {
                throw new ArgumentException($"Invalid source station: {source}");
            }

            if (!IsValidStationIndex(destination))
            {
                throw new ArgumentException($"Invalid destination station: {destination}");
            }

            int retVal = -1;

            // We will store a record of edges we've visited and the smallest running distance that
            // we had when we visited each edge
            Dictionary<Tuple<int, int>, int> visitedMin = new Dictionary<Tuple<int, int>, int>();

            foreach (KeyValuePair<int, int> connection in _tracks[source])
            {
                visitedMin.Add(new Tuple<int, int>(source, connection.Key), connection.Value);
                TraversalStats traversal = new TraversalStats(connection.Key, connection.Value, 1);
                GetShortestDistance(traversal, destination, visitedMin, ref retVal);
            }

            return retVal;
        }

        private void GetShortestDistance(
            TraversalStats curTraversal,
            int destination,
            Dictionary<Tuple<int, int>, int> visitedMin,
            ref int minSoFar
        ){
            if (minSoFar != -1 && curTraversal.DistanceTraveled >= minSoFar)
            {
                // We've already traveled further than the shortest known result, so we're not going
                // to benefit by following the path any longer.
                return;
            }

            if (curTraversal.Station == destination)
            {
                if (minSoFar == -1 || curTraversal.DistanceTraveled < minSoFar)
                {
                    // We have a new minimum.  Celebrate! ...with stoic contemplation.
                    // Or, you know, just record it and move on.
                    minSoFar = curTraversal.DistanceTraveled;
                }

                return;
            }

            foreach (KeyValuePair<int, int> connection in _tracks[curTraversal.Station])
            {
                TraversalStats nextTraversal = GetNextTraversal(curTraversal, connection.Key, connection.Value);

                bool entryFound;
                int entryDistance;
                Tuple<int, int> edge = new Tuple<int, int>(curTraversal.Station, nextTraversal.Station);
                entryFound = visitedMin.TryGetValue(edge, out entryDistance);

                // We should visit the edge if it hasn't been visited yet, or if
                // we're currently on a shorter distance than we were when we last visited the edge.
                if (!entryFound || nextTraversal.DistanceTraveled < entryDistance)
                {
                    visitedMin[edge] = nextTraversal.DistanceTraveled;

                    GetShortestDistance(nextTraversal, destination, visitedMin, ref minSoFar);
                }
            }
        }

        /// <summary>
        /// Wraps common logic for public route-counting methods.
        /// Traverses the graph while a traversal condition is true.
        /// Counts routes that match a count condition.
        /// </summary>
        /// <param name="source">starting station index</param>
        /// <param name="routeCountCondition">If this traversal point meets the condition, count the route</param>
        /// <param name="traverseWhileCondition">
        /// Keep traversing as long as our stats meet this condition.
        /// Don't use t => true unless you want this method to blow the stack with
        /// neverending traversals.  That's why we're keeping this method as a private utility.
        /// </param>
        /// <returns>
        /// number of routes that meet the routeCountCondition within the bounds of the traverseWhileCondition.
        /// </returns>
        private int GetNumRoutesDFS(
            int source,
            Predicate<TraversalStats> routeCountCondition,
            Predicate<TraversalStats> traverseWhileCondition
        )
        {
            return GetNumRoutesDFS(
                new TraversalStats(source, 0, 0),
                routeCountCondition,
                traverseWhileCondition
            );
        }

        /// <summary>
        /// Actual implementation of DFS counting algorithm.  Recursive.
        /// </summary>
        /// <param name="curTraversal">represents the graph traversal up to this point</param>
        /// <param name="routeCountCondition">If this traversal point meets the condition, count the route</param>
        /// <param name="traverseWhileCondition">
        /// Keep traversing as long as our stats meet this condition.
        /// Don't use t => true unless you want this method to blow the stack with
        /// neverending traversals.  That's why we're keeping this method as a private utility.
        /// </param>
        /// <returns>
        /// number of routes that meet the routeCountCondition within the bounds of the traverseWhileCondition.
        /// </returns>
        private int GetNumRoutesDFS(
            TraversalStats curTraversal,
            Predicate<TraversalStats> routeCountCondition,
            Predicate<TraversalStats> traverseWhileCondition
        )
        {
            int retVal = 0;

            foreach (KeyValuePair<int, int> connection in _tracks[curTraversal.Station])
            {
                TraversalStats nextTraversal = GetNextTraversal(curTraversal, connection.Key, connection.Value);

                if (traverseWhileCondition(nextTraversal))
                {
                    if (routeCountCondition(nextTraversal))
                    {
                        // we meet the criteria for being counted
                        ++retVal;
                    }

                    // Add any subsequent paths that meet the condition
                    retVal += GetNumRoutesDFS(nextTraversal, routeCountCondition, traverseWhileCondition);
                }
            }

            return retVal;
        }

        /// <summary>
        /// Build new traversal information from previous information and outbound connection info.
        /// </summary>
        /// <param name="curTraversal">traversal info so far</param>
        /// <param name="station">next station</param>
        /// <param name="distance">distance to next station</param>
        /// <returns>updated traversal information</returns>
        private TraversalStats GetNextTraversal(TraversalStats curTraversal, int station, int distance)
        {
            return new TraversalStats(
                station, // next station id
                curTraversal.DistanceTraveled + distance, // total distance traveled
                curTraversal.NumStops + 1
            );
        }

        #endregion Methods

        /// <summary>
        /// Keeps track of data in a graph traversal.
        /// </summary>
        private class TraversalStats
        {
            /// <summary>
            /// Station ID
            /// </summary>
            public int Station { get; private set; }

            /// <summary>
            /// Total distance traveled to get to this station
            /// </summary>
            public int DistanceTraveled { get; private set; }

            /// <summary>
            /// Number of stations encountered on trip, not including departure
            /// </summary>
            public int NumStops { get; private set; }

            public TraversalStats(int station, int distanceTraveled, int stationCount)
            {
                Station = station;
                DistanceTraveled = distanceTraveled;
                NumStops = stationCount;
            }
        }

    }
}

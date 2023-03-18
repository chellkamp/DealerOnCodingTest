namespace TrainsLibTests
{
    [TestClass]
    public class GraphTest
    {
        /// <summary>
        /// Nominal graph population and edge retrieval.
        /// </summary>
        [TestMethod]
        public void GraphPopulation()
        {
            GraphEntry[] testEntries = CreateValidGraphEntries();
            TrackGraph g = new TrackGraph(5);

            foreach (GraphEntry entry in testEntries)
            {
                g.SetDirectTrackDistance(entry.Source, entry.Destination, entry.Distance);
            }

            foreach(GraphEntry entry in testEntries)
            {
                int distance = g.GetDirectTrackDistance(entry.Source, entry.Destination);
                Assert.AreEqual(distance, entry.Distance);
            }
        }

        /// <summary>
        /// Attempt to insert invalid entries into the graph to verify that they fail.
        /// </summary>
        [TestMethod]
        public void AttemptInvalidEntries()
        {
            // array of invalid entries
            GraphEntry[] testEntries = {

                // Invalid sources
                new GraphEntry() { Source = -1, Destination = 0, Distance = 2 },
                new GraphEntry() { Source = 5, Destination = 0, Distance = 2 },

                // Invalid destinations
                new GraphEntry() { Source = 3, Destination = -1, Distance = 1 },
                new GraphEntry() { Source = 3, Destination = 5, Distance = 1 },

                // Invalid distance
                new GraphEntry() { Source = 3, Destination = 4, Distance = -5 },

                // Source == Destination
                new GraphEntry() { Source = 3, Destination = 3, Distance = 2 }
        };

            TrackGraph g = new TrackGraph(5);

            // Loop through cases
            foreach (GraphEntry entry in testEntries)
            {
                Assert.ThrowsException<ArgumentException>(
                    () => g.SetDirectTrackDistance(entry.Source, entry.Destination, entry.Distance)
                );
            }
        }

        [TestMethod]
        public void ExactRouteDistanceSuccess()
        {
            TrackGraph g = CreateValidGraph();

            // Test inputs and outputs
            TestPath[] testPaths = {

                // Route exists
                new TestPath() { Distance = 9, Route = new List<int>() { 0, 1, 2 } },
                new TestPath() { Distance = 5, Route = new List<int>() { 0, 3 } },
                new TestPath() { Distance = 13, Route = new List<int>() { 0, 3, 2 }},
                new TestPath() { Distance = 22, Route = new List<int>() { 0, 4, 1, 2, 3} },

                // No such route
                new TestPath() { Distance = -1, Route = new List<int>() { 0, 4, 3 } },
                new TestPath() { Distance = -1, Route = new List<int>() { 2, 0 } },
                new TestPath() { Distance = -1, Route= new List<int>() { 4, 1, 2, 0 } }

            };

            foreach (TestPath path in testPaths)
            {
                int result = g.GetDistanceOfExactRoute(path.Route);
                Assert.AreEqual(path.Distance, result);
            }
        }

        /// <summary>
        /// Tests bad arguments in calls to TrackGraph.GetNumRoutesMaxStops().
        /// </summary>
        [TestMethod]
        public void MaxStopsFail()
        {
            TrackGraph g = CreateValidGraph();

            NumStopsArgs[] testArgs = {

                // Invalid source
                new NumStopsArgs() { Source = -1, Destination = 0, NumStops = 1 },
                new NumStopsArgs() { Source = 6, Destination = 0, NumStops = 1 },

                // Invalid destination
                new NumStopsArgs() { Source = 3, Destination = -1, NumStops = 1 },
                new NumStopsArgs() { Source = 3, Destination = 6, NumStops = 1 },

                // Invalid number of stops
                new NumStopsArgs() { Source = 3, Destination = 0, NumStops = 0 },
            };

            foreach (NumStopsArgs argEntry in testArgs)
            {
                Assert.ThrowsException<ArgumentException>(
                    () => g.GetNumRoutesMaxStops(argEntry.Source, argEntry.Destination, argEntry.NumStops)
                );
            }
        }

        /// <summary>
        /// Tests valid arguments to TrackGraph.GetNumRouteMaxStops().
        /// </summary>
        [TestMethod]
        public void MaxStopsSuccess()
        {
            TrackGraph g = CreateValidGraph();

            NumStopsArgs[] testArgs = {
                new NumStopsArgs() { Source = 2, Destination = 2, NumStops = 3, Expected = 2 },
                new NumStopsArgs() { Source = 3, Destination = 0, NumStops = 6, Expected = 0 },
                new NumStopsArgs() { Source = 2, Destination = 4, NumStops = 6, Expected = 12 }
            };

            foreach (NumStopsArgs argEntry in testArgs)
            {
                int result = g.GetNumRoutesMaxStops(argEntry.Source, argEntry.Destination, argEntry.NumStops);
                Assert.AreEqual(argEntry.Expected, result);
            }
        }

        /// <summary>
        /// Tests bad arguments in calls to TrackGraph.GetNumRoutesExactStops().
        /// </summary>
        [TestMethod]
        public void ExactStopsFail()
        {
            TrackGraph g = CreateValidGraph();

            NumStopsArgs[] testArgs = {

                // Invalid source
                new NumStopsArgs() { Source = -1, Destination = 0, NumStops = 1 },
                new NumStopsArgs() { Source = 6, Destination = 0, NumStops = 1 },

                // Invalid destination
                new NumStopsArgs() { Source = 3, Destination = -1, NumStops = 1 },
                new NumStopsArgs() { Source = 3, Destination = 6, NumStops = 1 },

                // Invalid number of stops
                new NumStopsArgs() { Source = 3, Destination = 0, NumStops = 0 }
            };

            foreach (NumStopsArgs argEntry in testArgs)
            {
                Assert.ThrowsException<ArgumentException>(
                    () => g.GetNumRoutesExactStops(argEntry.Source, argEntry.Destination, argEntry.NumStops)
                );
            }
        }

        /// <summary>
        /// Tests valid arguments in calls to TrackGraph.GetNumRoutesExactStops().
        /// </summary>
        [TestMethod]
        public void ExactStopsSuccess()
        {
            TrackGraph g = CreateValidGraph();

            NumStopsArgs[] testArgs = {
                new NumStopsArgs() { Source = 0, Destination = 2, NumStops = 4, Expected = 3 },
                new NumStopsArgs() { Source = 3, Destination = 0, NumStops = 2, Expected = 0 },
                new NumStopsArgs() { Source = 4, Destination = 4, NumStops = 4, Expected = 1 },
                new NumStopsArgs() { Source = 4, Destination = 4, NumStops = 6, Expected = 2 },
            };

            foreach (NumStopsArgs argEntry in testArgs)
            {
                int result = g.GetNumRoutesExactStops(argEntry.Source, argEntry.Destination, argEntry.NumStops);
                Assert.AreEqual(argEntry.Expected, result);
            }
        }

        /// <summary>
        /// Tests argument failures for TrackGraph.GetShortestDistance().
        /// </summary>
        [TestMethod]
        public void ShortestDistanceFail()
        {
            TrackGraph g = CreateValidGraph();
            MinDistanceArgs[] testArgs = {

                // Invalid source
                new MinDistanceArgs() { Source = -1, Destination = 2 },
                new MinDistanceArgs() { Source = 6, Destination = 2 },

                // Invalid destination
                new MinDistanceArgs() { Source = 2, Destination = -1 },
                new MinDistanceArgs() { Source = 2, Destination = 6 }
            };

            foreach (MinDistanceArgs argEntry in testArgs)
            {
                Assert.ThrowsException<ArgumentException>(
                    () => g.GetShortestDistance(argEntry.Source, argEntry.Destination)
                );
            }
        }

        /// <summary>
        /// Tests valid arguments for TrackGraph.GetShortestDistance().
        /// </summary>
        [TestMethod]
        public void ShortestDistanceSuccess()
        {
            TrackGraph g = CreateValidGraph();

            MinDistanceArgs[] testArgs = {

                // at least one path exists
                new MinDistanceArgs() { Source = 0, Destination = 2, Expected = 9 },
                new MinDistanceArgs() { Source = 1, Destination = 1, Expected = 9 },

                // no path
                new MinDistanceArgs() { Source = 0, Destination = 0, Expected = -1 }
            };

            foreach (MinDistanceArgs argEntry in testArgs)
            {
                int result = g.GetShortestDistance(argEntry.Source, argEntry.Destination);
                Assert.AreEqual(argEntry.Expected, result);
            }
        }


        private GraphEntry[] CreateValidGraphEntries()
        {
            return new GraphEntry[] {
                new GraphEntry() { Source = 0, Destination = 1, Distance = 5 },
                new GraphEntry() { Source = 1, Destination = 2, Distance = 4 },
                new GraphEntry() { Source = 2, Destination = 3, Distance = 8 },
                new GraphEntry() { Source = 3, Destination = 2, Distance = 8 },
                new GraphEntry() { Source = 3, Destination = 4, Distance = 6 },
                new GraphEntry() { Source = 0, Destination = 3, Distance = 5 },
                new GraphEntry() { Source = 2, Destination = 4, Distance = 2 },
                new GraphEntry() { Source = 4, Destination = 1, Distance = 3 },
                new GraphEntry() { Source = 0, Destination = 4, Distance = 7 }
            };
        }

        private TrackGraph CreateValidGraph()
        {
            GraphEntry[] testEntries = CreateValidGraphEntries();
            TrackGraph g = new TrackGraph(5);

            foreach (GraphEntry entry in testEntries)
            {
                g.SetDirectTrackDistance(entry.Source, entry.Destination, entry.Distance);
            }

            return g;
        }

        /// <summary>
        /// Utility class for keeping track of test inputs
        /// </summary>
        class GraphEntry
        {
            public int Source;
            public int Destination;
            public int Distance;
        }

        /// <summary>
        /// Specifies an exact route and the expected result of TrackGraph.GetDistanceOfExactRoute().
        /// </summary>
        class TestPath
        {
            public int Distance;
            public IList<int> Route = new List<int>();
        }

        class NumStopsArgs
        {
            public int Source;
            public int Destination;
            public int NumStops;
            public int Expected;
        }

        class MinDistanceArgs
        {
            public int Source;
            public int Destination;
            public int Expected;
        }
    }
}

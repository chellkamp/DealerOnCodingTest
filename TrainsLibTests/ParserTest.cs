namespace TrainsLibTests
{
    [TestClass]
    public class ParserTest
    {
        /// <summary>
        /// Tests for a list of valid values
        /// </summary>
        [TestMethod]
        public void ValidInputExpectedValues()
        {
            InputUnit[] inputUnits = {
                new InputUnit() { Source = 'A', Destination = 'B', Distance = 5 },
                new InputUnit() { Source = 'B', Destination = 'C', Distance = 4 },
                new InputUnit() { Source = 'C', Destination = 'D', Distance = 8 },
                new InputUnit() { Source = 'D', Destination = 'C', Distance = 8 },
                new InputUnit() { Source = 'd', Destination = 'E', Distance = 6 },
                new InputUnit() { Source = 'A', Destination = 'D', Distance = 5 },
                new InputUnit() { Source = 'C', Destination = 'E', Distance = 2 },
                new InputUnit() { Source = 'E', Destination = 'b', Distance = 3 },
                new InputUnit() { Source = 'A', Destination = 'E', Distance = 7 },
            };

            string input = String.Join(
                ", ",
                inputUnits.Select<InputUnit, string>(i => i.ToString()).ToArray()
            );

            List<InputParser.DistanceInfo> results = InputParser.Parse(input);

            Assert.IsNotNull(results);
            Assert.AreEqual(inputUnits.Length, results.Count);

            for (int i = 0; i < inputUnits.Length; i++)
            {
                InputUnit curTestInput = inputUnits[i];
                InputParser.DistanceInfo curResult = results[i];

                Assert.IsNotNull(curResult);
                Assert.AreEqual(Char.ToUpper(curTestInput.Source), curResult.Source);
                Assert.AreEqual(Char.ToUpper(curTestInput.Destination), curResult.Destination);
                Assert.AreEqual(curTestInput.Distance, curResult.Distance);
            }
        }

        /// <summary>
        /// Tests for a single valid value
        /// </summary>
        [TestMethod]
        public void ValidInputSingleValue()
        {
            InputUnit testInput = new InputUnit() { Source = 'C', Destination = 'A', Distance = 35 };
            List<InputParser.DistanceInfo> results = InputParser.Parse(testInput.ToString());

            Assert.IsNotNull(results);
            Assert.AreEqual(1, results.Count);

            Assert.AreEqual(Char.ToUpper(testInput.Source), results[0].Source);
            Assert.AreEqual(Char.ToUpper(testInput.Destination), results[0].Destination);
            Assert.AreEqual(testInput.Distance, results[0].Distance);
        }

        /// <summary>
        /// Tests for handling extra whitespace.
        /// </summary>
        [TestMethod]
        public void ValidInputExtraWhitespace()
        {
            string testInput = "\t\r\nAB5 , BC4\n , CD8, DC8, DE6,\r\nAD5, CE2, EB3, AE7   ";

            List<InputParser.DistanceInfo> results = InputParser.Parse(testInput);

            Assert.IsNotNull(results);
            Assert.AreEqual(9, results.Count);
        }

        /// <summary>
        /// Tests that we correctly handle 0 entries.
        /// </summary>
        [TestMethod]
        public void NoInput()
        {
            string testInput = "";
            List<InputParser.DistanceInfo> results = InputParser.Parse(testInput);

            Assert.IsNotNull(results);
            Assert.AreEqual(0, results.Count);
        }

        [TestMethod]
        public void MalformedInput()
        {
            string testInput = "AB5 , C4";
            InvalidDataException e = Assert.ThrowsException<InvalidDataException>(
                () => InputParser.Parse(testInput)
            );
        }

        /// <summary>
        /// Specify that a town identifier is outside the range we currently expect (A-E)
        /// </summary>
        [TestMethod]
        public void IdentifierOutsideRange()
        {
            string testInput = "AF5";
            InvalidDataException e = Assert.ThrowsException<InvalidDataException>(
                () => InputParser.Parse(testInput)
            );
        }

        class InputUnit
        {
            public char Source;
            public char Destination;
            public int Distance;

            public override string ToString()
            {
                return $"{Source}{Destination}{Distance}";
            }
        }
    }
}
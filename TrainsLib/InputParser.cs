using System.Text.RegularExpressions;

namespace TrainsLib
{
    public static class InputParser
    {
        private static readonly string _edgeFormat = @"[A-E][A-E]\d+";

        private static readonly Regex _edgeRegex = new Regex(
            _edgeFormat,
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );

        // matches 0 or more edges
        private static readonly Regex _validInputRegex = new Regex(
            $@"^\s*({_edgeFormat}(\s*,\s*{_edgeFormat})*)?\s*$",
            RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.IgnoreCase
        );

        public static List<DistanceInfo> Parse(string input)
        {
            if (!_validInputRegex.IsMatch(input))
            {
                throw new InvalidDataException("Input incorrectly formatted");
            }

            List<DistanceInfo> retVal = new List<DistanceInfo>();

            int startPos = 0;

            Match? m = null;
            while ((m = _edgeRegex.Match(input, startPos)).Success) {
                string val = m.Value.ToUpper();
                char source = val[0];
                char destination = val[1];
                int distance = Int32.Parse(val.Substring(2));

                if (distance <= 0)
                {
                    throw new InvalidDataException("Track length cannot be 0");
                }

                retVal.Add(new DistanceInfo(source, destination, distance));

                startPos = m.Index + m.Length;
            }

            return retVal;
        }

        /// <summary>
        /// Parsed unit of input.
        /// </summary>
        public class DistanceInfo
        {
            /// <summary>
            /// Source station
            /// </summary>
            public char Source { get; private set; }

            /// <summary>
            /// Destination station
            /// </summary>
            public char Destination { get; private set;}

            public int Distance { get; private set; }

            /// <summary>
            /// Constructor
            /// </summary>
            /// <param name="source">source station</param>
            /// <param name="destination">destination station</param>
            public DistanceInfo(char source, char destination, int distance)
            {
                Source = source;
                Destination = destination;
                Distance = distance;
            }
        }
    }
}
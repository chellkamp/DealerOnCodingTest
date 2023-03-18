using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Runtime.ConstrainedExecution;
using TrainsLib;
using WebApp.Models;
using WebApp.Util;
using static System.Formats.Asn1.AsnWriter;

namespace WebApp.Controllers
{
    /// <summary>
    /// Controller handling request data.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class WorkController : ControllerBase
    {
        // We're hardcoding 5 stations for now because that satisfies the parameters of the problem description
        // but everything underneath should be able to handle more for future scalability.
        private readonly int _NUM_NODES = 5;

        // POST api/<WorkController>
        [HttpPost]
        public object Post([FromBody]WorkRequest request)
        {
            List<WorkResult> retVal = new List<WorkResult>();

            try
            {
                List<InputParser.DistanceInfo> parsedEdges = InputParser.Parse(request.Data);

                TrackGraph g = new TrackGraph(_NUM_NODES);

                foreach (InputParser.DistanceInfo edge in parsedEdges)
                {
                    g.SetDirectTrackDistance(
                        GraphDisplayWrapper.GetStationIndex(edge.Source),
                        GraphDisplayWrapper.GetStationIndex(edge.Destination),
                        edge.Distance
                    );
                }

                GraphDisplayWrapper gWrapper = new GraphDisplayWrapper(g);

                //  1) The distance of the route A-B-C
                GraphDisplayWrapper.DisplayInfo di = gWrapper.GetDistanceOfExactRoute(
                        new List<char> { 'A', 'B', 'C' }
                );
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  2) The distance of the route A-D
                di = gWrapper.GetDistanceOfExactRoute(new List<char> { 'A', 'D' });
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  3) The distance of the route A - D - C
                di = gWrapper.GetDistanceOfExactRoute(new List<char> { 'A', 'D', 'C' });
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  4) The distance of the route A - E - B - C - D
                di = gWrapper.GetDistanceOfExactRoute(new List<char> { 'A', 'E', 'B', 'C', 'D' });
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  5) The distance of the route A - E - D
                di = gWrapper.GetDistanceOfExactRoute(new List<char> { 'A', 'E', 'D' });
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  6) The number of trips starting at C and ending at C with a maximum of 3 stops
                di = gWrapper.GetNumRoutesMaxStops('C', 'C', 3);
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  7) The number of trips starting at A and ending at C with exactly 4 stops
                di = gWrapper.GetNumRoutesExactStops('A', 'C', 4);
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  8) The length of the shortest route (in terms of distance to travel) from A to C
                di = gWrapper.GetShortestDistance('A', 'C');
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                //  9) The length of the shortest route (in terms of distance to travel) from B to B
                di = gWrapper.GetShortestDistance('B', 'B');
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                // 10) The number of different routes from C to C with a distance of less than 30
                di = gWrapper.GetNumRoutesLessThanDistance('C', 'C', 30);
                retVal.Add(new WorkResult() { Value = di.Result, Description = di.Description });

                for (int i = 0; i < retVal.Count; ++i)
                {
                    retVal[i].Name = $"Output #{i + 1}";
                }
            }
            catch (Exception ex)
            {
                return new { Error = ex.Message };
            }

            
            return retVal;
        }

    }
}

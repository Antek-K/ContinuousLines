using ContinuousLines.DataStructures;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace ContinuousLines.Resolvers
{
    /// <summary>
    /// The class to find next continues lines between chosen points, assuming that lines cannot cross each other
    /// </summary>
    /// The idea of algorithm was mine, but I got inspiration from article https://en.wikipedia.org/wiki/Maze-solving_algorithm (mainly from demos)
    /// And I used my implementation of Dijkstra algorithm
    public class PathResolver
    {
        // already used values, useful to get potential new values
        private readonly SortedSet<double> usedXValues = new();
        private readonly SortedSet<double> usedYValues = new();

        // existing edges, to make sure new edges don't cross them
        private readonly Dictionary<double, List<SortedEdgeEnds>> existingEdgesHorizontal = new();
        private readonly Dictionary<double, List<SortedEdgeEnds>> existingEdgesVertical = new();


        public PathResolver(double areaHeight, double areaWidth)
        {
            usedXValues.Add(0);
            usedXValues.Add(areaWidth);
            usedYValues.Add(0);
            usedYValues.Add(areaHeight);
        }

        public PointCollection ResolveNextPath(Point startPoint, Point endPoint)
        {
            // To simplify problem, I asumed that every line in the problem is polyline and every it's edge is horizontal or vertical, not oblique
            // Moreover, node coordinates can have only discreet values, from list potentialXValues and potentialYValues
            var potentialXValues = GetPotentialValues(usedXValues.ToList(), startPoint.X, endPoint.X, out int startNodeXCoordinate, out int endNodeXCoordinate);
            var potentialYValues = GetPotentialValues(usedYValues.ToList(), startPoint.Y, endPoint.Y, out int startNodeYCoordinate, out int endNodeYCoordinate);

            // coordinates by indexes
            var startNodeCoordinates = new NodeCoordinates(startNodeXCoordinate, startNodeYCoordinate);
            var endNodeCoordinates = new NodeCoordinates(endNodeXCoordinate, endNodeYCoordinate);

            var availableEdgesHorizontal = GetAvailableEdgesHorizontal(potentialXValues, potentialYValues, existingEdgesVertical); // if true, horizontal edge is availble for solution
            var availableEdgesVertical = GetAvailableEdgesVertical(potentialXValues, potentialYValues, existingEdgesHorizontal); // if true, vertical edge is availble for solution

            var edgesCostsHorizontal = GetEdgesCosts(potentialXValues); // cost of next horizontal edges in row, apply for every row
            var edgesCostsVertical = GetEdgesCosts(potentialYValues); // cost of next vertical edges in column, apply for every column

            // Graph prepared for Dijkstra's algorithm, operating on indexes
            var graph = new Graph(availableEdgesHorizontal, edgesCostsHorizontal, availableEdgesVertical, edgesCostsVertical);


            var dijkstraaAlgorithm = new DijkstrasAlgorithm(graph);
            var path = dijkstraaAlgorithm.ComputePath(startNodeCoordinates, endNodeCoordinates);

            var points = path.Select(n => new Point(potentialXValues[n.X], potentialYValues[n.Y])).ToList(); // nodes to point

            AddValuesToUsedValues(points);
            AddEdgesToExistingEdges(points);

            return new PointCollection(points);
        }

        public void AddValuesToUsedValues(List<Point> points)
        {
            usedXValues.UnionWith(points.Select(p => p.X));
            usedYValues.UnionWith(points.Select(p => p.Y));
        }

        public void AddEdgesToExistingEdges(List<Point> points)
        {
            for (int i = 0; i < points.Count - 1; i++)
            {
                if (points[i].X == points[i + 1].X)
                {
                    if (!existingEdgesVertical.ContainsKey(points[i].X))
                    {
                        existingEdgesVertical[points[i].X] = new();
                    }
                    existingEdgesVertical[points[i].X].Add(new(points[i].Y, points[i + 1].Y));
                }
                else if (points[i].Y == points[i + 1].Y)
                {
                    if (!existingEdgesHorizontal.ContainsKey(points[i].Y))
                    {
                        existingEdgesHorizontal[points[i].Y] = new();
                    }
                    existingEdgesHorizontal[points[i].Y].Add(new(points[i].X, points[i + 1].X));
                }
            }
        }
        
        // Get one (or two, in special condition) value from every range between used values
        private static List<double> GetPotentialValues(List<double> usedXValuesList, double startPointCoordinate, double endPointCoordinate, out int startNodeCoordinate, out int endNodeCoordinate)
        {
            var potentialValuesList = new List<double>();
            startNodeCoordinate = 0;
            endNodeCoordinate = 0;

            for (int i = 0; i < usedXValuesList.Count - 1; i++)
            {
                bool valueAdded = false;
                if (startPointCoordinate >= usedXValuesList[i] && startPointCoordinate < usedXValuesList[i + 1]) // if start point is in range, use it's coordinate
                {
                    startNodeCoordinate = potentialValuesList.Count;
                    potentialValuesList.Add(startPointCoordinate);
                    valueAdded = true;
                }
                if (endPointCoordinate >= usedXValuesList[i] && endPointCoordinate < usedXValuesList[i + 1]) // if end point is in range, use it's coordinate
                {
                    endNodeCoordinate = potentialValuesList.Count;
                    potentialValuesList.Add(endPointCoordinate);
                    if (valueAdded && endPointCoordinate < startPointCoordinate)
                    {
                        var tmp = endNodeCoordinate;
                        endNodeCoordinate = startNodeCoordinate;
                        startNodeCoordinate = tmp;
                    }
                    valueAdded = true;
                }
                if (!valueAdded) // if start point nor end point not used, take average
                {
                    potentialValuesList.Add((usedXValuesList[i] + usedXValuesList[i + 1]) / 2);
                }
            }

            potentialValuesList.Sort();
            return potentialValuesList;
        }

        private static bool[,] GetAvailableEdgesHorizontal(List<double> potentialXValues, List<double> potentialYValues, Dictionary<double, List<SortedEdgeEnds>> existingEdgesVertical)
        {
            bool[,] availableEdges = PrepareEmptyAvailabeEdgesArray(potentialXValues.Count - 1, potentialYValues.Count);

            foreach (var existingEdgesForX in existingEdgesVertical)
            {
                int i = GetIndexOfLastPotentialValueLessThanValue(potentialXValues, existingEdgesForX.Key);

                foreach (var edge in existingEdgesForX.Value)
                {
                    for (int j = 0; j < potentialYValues.Count; j++)
                    {
                        var potentialYValue = potentialYValues[j];
                        availableEdges[i, j] = availableEdges[i, j] && !(potentialYValue > edge.Lower && potentialYValue < edge.Greater); // if cross with existing edge, change to false
                    }
                }
            }
            return availableEdges;
        }

        private static bool[,] GetAvailableEdgesVertical(List<double> potentialXValues, List<double> potentialYValues, Dictionary<double, List<SortedEdgeEnds>> existingEdgesHorizontal)
        {
            bool[,] availableEdges = PrepareEmptyAvailabeEdgesArray(potentialXValues.Count, potentialYValues.Count - 1);

            foreach (var existingEdgesForY in existingEdgesHorizontal)
            {
                int i = GetIndexOfLastPotentialValueLessThanValue(potentialYValues, existingEdgesForY.Key);

                foreach (var edge in existingEdgesForY.Value)
                {
                    for (int j = 0; j < potentialXValues.Count; j++)
                    {
                        var potentialXValue = potentialXValues[j];
                        availableEdges[j, i] = availableEdges[j, i] && !(potentialXValue > edge.Lower && potentialXValue < edge.Greater); // if cross with existing edge, change to false
                    }
                }
            }
            return availableEdges;
        }

        private static int GetIndexOfLastPotentialValueLessThanValue(List<double> potentialValues, double value)
        {
            var index = potentialValues.BinarySearch(value);
            index = (index < 0 ? ~index : index) - 1;
            return index;
        }

        private static bool[,] PrepareEmptyAvailabeEdgesArray(int sizeX, int sizeY)
        {
            var emptyAvailableEdges = new bool[sizeX, sizeY];

            for (int i = 0; i < emptyAvailableEdges.GetLength(0); i++)
            {
                for (int j = 0; j < emptyAvailableEdges.GetLength(1); j++)
                {
                    emptyAvailableEdges[i, j] = true;
                }
            }

            return emptyAvailableEdges;
        }

        private static List<double> GetEdgesCosts(List<double> potentialValuesList)
        {
            var edgesCosts = new List<double>(potentialValuesList.Count - 1);
            for (int i = 0; i < potentialValuesList.Count - 1; i++)
            {
                edgesCosts.Add(potentialValuesList[i + 1] - potentialValuesList[i]);
            }
            return edgesCosts;
        }
    }
}

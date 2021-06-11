using System.Collections.Generic;
using System.Linq;

namespace ContinuousLines.DataStructures
{
    /// <summary>
    /// The class to store node's properties, it's Cost, Coordinates and PrevoiusNodeCoordinates.
    /// Additionaly, it provide method for node comparison.
    /// </summary>
    class Graph
    {
        private readonly bool[,] availableEdgesHorizontal; // if true, horizontal edge is availble for solution
        private readonly List<double> edgesCostsHorizontal; // cost of next horizontal edges in row, apply for every row
        private readonly int nodeHorizontalCount; // number of nodes in row
        private readonly bool[,] availableEdgesVertical; // if true, vertical edge is availble for solution
        private readonly List<double> edgesCostsVertical; // cost of next vertical edges in column, apply for every column
        private readonly int nodeVerticalCount; // number of nodes in column

        private readonly Node[,] nodes; // nodes to be used in solution
        private readonly SortedSet<Node> processedNodes = new(); // nodes for that computation has started, but not finished yet
        private readonly HashSet<Node> notComputedNodes = new(); // nodes for that computation has not finished yet

        public Graph(bool[,] availableEdgesHorizontal, List<double> edgesCostsHorizontal, bool[,] availableEdgesVertical, List<double> edgesCostsVertical)
        {
            this.availableEdgesHorizontal = availableEdgesHorizontal;
            this.edgesCostsHorizontal = edgesCostsHorizontal;
            nodeHorizontalCount = edgesCostsHorizontal.Count + 1;
            this.availableEdgesVertical = availableEdgesVertical;
            this.edgesCostsVertical = edgesCostsVertical;
            nodeVerticalCount = edgesCostsVertical.Count + 1;

            nodes = new Node[nodeHorizontalCount, nodeVerticalCount];

            for (int i = 0; i < nodeHorizontalCount; i++)
            {
                for (int j = 0; j < nodeVerticalCount; j++)
                {
                    var coordinates = new NodeCoordinates(i, j);
                    var node = new Node(coordinates);
                    nodes[i, j] = node;
                    notComputedNodes.Add(node);
                }
            }
        }

        public Node GetNode(NodeCoordinates coordinates) => nodes[coordinates.X, coordinates.Y];

        public double GetEdgeCostToLeft(NodeCoordinates coordinates) => edgesCostsHorizontal[coordinates.X - 1];
        public double GetEdgeCostToUp(NodeCoordinates coordinates) => edgesCostsVertical[coordinates.Y - 1];
        public double GetEdgeCostToRight(NodeCoordinates coordinates) => edgesCostsHorizontal[coordinates.X];
        public double GetEdgeCostToDown(NodeCoordinates coordinates) => edgesCostsVertical[coordinates.Y];

        // Gets node from left and determinates if edge to it is available
        public bool IsEdgeToLeftAvailable(NodeCoordinates prevoiusNodeCoordinates, out NodeCoordinates nextNodeCoordinates)
        {
            nextNodeCoordinates = prevoiusNodeCoordinates.GetNodeCoordinatesFromLeft();
            return IsCoordinatesInRange(nextNodeCoordinates) && availableEdgesHorizontal[nextNodeCoordinates.X, nextNodeCoordinates.Y];
        }

        // Gets node from up and determinates if edge to it is available
        public bool IsEdgeToUpAvailable(NodeCoordinates prevoiusNodeCoordinates, out NodeCoordinates nextNodeCoordinates)
        {
            nextNodeCoordinates = prevoiusNodeCoordinates.GetNodeCoordinatesFromUp();
            return IsCoordinatesInRange(nextNodeCoordinates) && availableEdgesVertical[nextNodeCoordinates.X, nextNodeCoordinates.Y];
        }

        // Gets node from right and determinates if edge to it is available
        public bool IsEdgeToRightAvailable(NodeCoordinates prevoiusNodeCoordinates, out NodeCoordinates nextNodeCoordinates)
        {
            nextNodeCoordinates = prevoiusNodeCoordinates.GetNodeCoordinatesFromRight();
            return IsCoordinatesInRange(nextNodeCoordinates) && availableEdgesHorizontal[prevoiusNodeCoordinates.X, prevoiusNodeCoordinates.Y];
        }

        // Gets node from down and determinates if edge to it is available
        public bool IsEdgeToDownAvailable(NodeCoordinates prevoiusNodeCoordinates, out NodeCoordinates nextNodeCoordinates)
        {
            nextNodeCoordinates = prevoiusNodeCoordinates.GetNodeCoordinatesFromDown();
            return IsCoordinatesInRange(nextNodeCoordinates) && availableEdgesVertical[prevoiusNodeCoordinates.X, prevoiusNodeCoordinates.Y];
        }

        public void MarkNodeAsProcessed(Node node) => processedNodes.Add(node);
        public void MarkNodeAsNotProcessed(Node node) => processedNodes.Remove(node);
        public void MarkNodeAsComputed(Node node)
        {
            processedNodes.Remove(node);
            notComputedNodes.Remove(node);
        }

        public bool IsNodeNotComputed(Node node) => notComputedNodes.Contains(node);

        public bool TryGetCheapestProcessedNode(out Node cheapestProcessedNode)
        {
            var anyProcessedNodes = processedNodes.Any();
            cheapestProcessedNode = null;
            if (anyProcessedNodes)
            {
                cheapestProcessedNode = processedNodes.First();
            }
            return anyProcessedNodes;
        }

        // Verifies if node exists with such coordinates
        private bool IsCoordinatesInRange(NodeCoordinates nodeCoordinates)
            => nodeCoordinates.X >= 0 && nodeCoordinates.X < nodeHorizontalCount && nodeCoordinates.Y >= 0 && nodeCoordinates.Y < nodeVerticalCount;
    }
}

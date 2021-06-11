using ContinuousLines.DataStructures;
using System.Collections.Generic;


namespace ContinuousLines.Resolvers
{
    /// <summary>
    /// Implementation of the Dijkstra's algorithm.
    /// It is algorithm for finding shortest path on graph
    /// </summary>
    /// It is implemented based on pseudo code and demos from https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
    class DijkstrasAlgorithm
    {
        // The graph which the path is found on 
        private readonly Graph graph;

        public DijkstrasAlgorithm(Graph graph)
        {
            this.graph = graph;
        }

        public List<NodeCoordinates> ComputePath(NodeCoordinates startNodeCoordinates, NodeCoordinates endNodeCoordinates)
        {
            var startNode = graph.GetNode(startNodeCoordinates);
            startNode.Cost = 0; // No move is needed to get to start node 
            graph.MarkNodeAsProcessed(startNode);

            while (graph.TryGetCheapestProcessedNode(out Node node)) // The cheapest processed node has already the lowest possible cost
            {
                if (node.Coordinates.Equals(endNodeCoordinates)) // If end node reached, get path and return it
                {
                    return GetPath(startNodeCoordinates, endNodeCoordinates);
                }
                graph.MarkNodeAsComputed(node);
                ComputeNeighbors(node);
            }

            return new(); // If end node cannot be reached, return empty list of node coordinates
        }

        // Restores path, by pass from end node to start node throughout previous nodes
        private List<NodeCoordinates> GetPath(NodeCoordinates startNodeCoordinates, NodeCoordinates endNodeCoordinates)
        {
            List<NodeCoordinates> path = new();

            var nodeCoordinates = endNodeCoordinates;
            path.Add(nodeCoordinates);
            while (!nodeCoordinates.Equals(startNodeCoordinates)) // while start node not reached
            {
                var node = graph.GetNode(nodeCoordinates);
                nodeCoordinates = node.PrevoiusNodeCoordinates;
                path.Add(nodeCoordinates);
            }

            return path;
        }

        // Updates cost of every available neighbor
        private void ComputeNeighbors(Node node)
        {
            NodeCoordinates prevoiusNodeCoordinates = node.Coordinates;
            double previousNodeCost = node.Cost;

            if (graph.IsEdgeToLeftAvailable(prevoiusNodeCoordinates, out NodeCoordinates nextNodeCoordinates))
            {
                UpdateNode(prevoiusNodeCoordinates, nextNodeCoordinates, previousNodeCost + graph.GetEdgeCostToLeft(prevoiusNodeCoordinates));
            }
            if (graph.IsEdgeToUpAvailable(prevoiusNodeCoordinates, out nextNodeCoordinates))
            {
                UpdateNode(prevoiusNodeCoordinates, nextNodeCoordinates, previousNodeCost + graph.GetEdgeCostToUp(prevoiusNodeCoordinates));
            }
            if (graph.IsEdgeToRightAvailable(prevoiusNodeCoordinates, out nextNodeCoordinates))
            {
                UpdateNode(prevoiusNodeCoordinates, nextNodeCoordinates, previousNodeCost + graph.GetEdgeCostToRight(prevoiusNodeCoordinates));
            }
            if (graph.IsEdgeToDownAvailable(prevoiusNodeCoordinates, out nextNodeCoordinates))
            {
                UpdateNode(prevoiusNodeCoordinates, nextNodeCoordinates, previousNodeCost + graph.GetEdgeCostToDown(prevoiusNodeCoordinates));
            }
        }


        // Updates cost of node, if it not already computed and has greater cost than new path.
        private void UpdateNode(NodeCoordinates prevoiusNodeCoordinates, NodeCoordinates nextNodeCoordinates, double cost)
        {
            var node = graph.GetNode(nextNodeCoordinates);
            if (graph.IsNodeNotComputed(node) && node.Cost > cost)
            {
                graph.MarkNodeAsNotProcessed(node);//Remove existing node from SortedSet, to update order after change by Add method
                node.Cost = cost;
                node.PrevoiusNodeCoordinates = prevoiusNodeCoordinates;
                graph.MarkNodeAsProcessed(node);
            }
        }
    }
}

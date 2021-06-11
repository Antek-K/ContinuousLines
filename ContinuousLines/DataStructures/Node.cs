using System;

namespace ContinuousLines.DataStructures
{
    /// <summary>
    /// The class to store node's properties, it's Cost, Coordinates and PrevoiusNodeCoordinates.
    /// Additionaly, it provide method for node comparison.
    /// </summary>
    class Node : IComparable<Node>
    {
        public Node(NodeCoordinates coordinates)
        {
            Coordinates = coordinates;
        }

        // Cost to reach the node.
        public double Cost { get; set; } = double.MaxValue;

        // Coordinates of the node
        public NodeCoordinates Coordinates { get; set; }

        // Coordinates of the previous node on the path
        public NodeCoordinates PrevoiusNodeCoordinates { get; set; }

        public int CompareTo(Node other)
        {
            if(ReferenceEquals(this, other))
            {
                return 0;
            }
            var result = Cost.CompareTo(other.Cost);
            if(result != 0)
            {
                return result;
            }

            return 2 * Coordinates.X.CompareTo(other.Coordinates.X) + Coordinates.Y.CompareTo(other.Coordinates.Y);
        }

        public override bool Equals(object obj)  => obj is Node node && CompareTo(node) == 0;

        public override int GetHashCode() => HashCode.Combine(Cost, Coordinates);

        public static bool operator <(Node left, Node right) => left.CompareTo(right) < 0;

        public static bool operator >(Node left, Node right) => left.CompareTo(right) > 0;

        public static bool operator <=(Node left, Node right) => left.CompareTo(right) <= 0;

        public static bool operator >=(Node left, Node right) => left.CompareTo(right) >= 0;

        public static bool operator ==(Node left, Node right) => left.CompareTo(right) == 0;

        public static bool operator !=(Node left, Node right) => left.CompareTo(right) != 0;
    }
}

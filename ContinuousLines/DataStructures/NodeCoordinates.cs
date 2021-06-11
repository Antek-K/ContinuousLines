using System;

namespace ContinuousLines.DataStructures
{
    /// <summary>
    /// The class to store coordianates of node.
    /// Additionally it allows to get neighbor's coordinates.
    /// </summary>
    class NodeCoordinates
    {
        public NodeCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }

        public NodeCoordinates GetNodeCoordinatesFromLeft() => new(X - 1, Y);
        public NodeCoordinates GetNodeCoordinatesFromUp() => new(X, Y - 1);
        public NodeCoordinates GetNodeCoordinatesFromRight() => new(X + 1, Y);
        public NodeCoordinates GetNodeCoordinatesFromDown() => new(X, Y + 1);

        public override bool Equals(object obj) 
            => obj is NodeCoordinates coordinates &&
                X == coordinates.X &&
                Y == coordinates.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);
    }
}

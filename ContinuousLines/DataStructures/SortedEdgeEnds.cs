namespace ContinuousLines.DataStructures
{
    /// <summary>
    /// The class to store coordianates of two ends of edge in one dimension in sorted order.
    /// </summary>
    class SortedEdgeEnds
    {
        public SortedEdgeEnds(double end1, double end2)
        {
            if(end1 < end2)
            {
                Lower = end1;
                Greater = end2;
            }
            else
            {
                Lower = end2;
                Greater = end1;
            }
        }

        public double Lower { get; }
        public double Greater { get; }
    }
}

using System.Numerics;


namespace GeometryToolkit.Util
{
    public class WindingUtil
    {
        /// <summary>
        /// Returns the points in counter-clockwise order.
        /// </summary>
        /// <param name="points">
        /// The points to order.
        /// </param>
        /// <returns>
        /// The new list of points in counter-clockwise order.
        /// </returns>
        public static List<Vector2> CounterClockWise(IReadOnlyList<Vector2> points)
        {
            Vector2 midPoint = points.Aggregate(new Vector2(0, 0), (acc, p) => acc + p) / points.Count;
            return points.OrderBy(p => Math.Atan2(p.Y - midPoint.Y, p.X - midPoint.X)).ToList();
        }
    }
}

using GeometryToolkit.Util;
using System.Numerics;

namespace GeometryToolkit.UV;

/// <summary>
/// The texture coordinate component.
/// </summary>
public enum TexCoordComponent
{
    U,
    V
}

/// <summary>
/// Tool to help with UV calculations.
/// </summary>
public class UVHelper
{
    /// <summary>
    /// Determines the whole number UV coordinates where the edges cross integer boundaries outside of the [0, 1] range.
    /// </summary>
    /// <param name="left">The left edge.</param>
    /// <param name="right">The right edge.</param>
    /// <param name="result">
    /// The list to populate with the coordinates where the edges cross integer boundaries outside of the [0, 1] range.
    /// If no edges are found, the list will remain empty.
    /// If edges are found, the list will contain the coordinates in ascending order, where each coordinate is a whole number.
    /// </param>
    /// <returns>
    /// <c>true</c> if any edges were found and added to the result list; otherwise, <c>false</c>.
    /// </returns>
    public static bool FindCoordsOutsideOfNormalUVRange(float left, float right, List<float> result)
    {
        float min = Math.Min(left, right);
        float max = Math.Max(left, right);

        // If both are within the 0 to 1 range, then no need to tessellate.
        if (min >= 0 && max <= 1)
        {
            return false;
        }

        // Ceil to next integer as we don't cate about values below min if both are positive.
        // For example for min of 2.3 we want to start at 3.
        if (min > 0 && max > 1)
        {
            min = (float)Math.Ceiling(min);
        }

        // 0 for min is OK therefore we can start at 1.
        if (min == 0)
        {
            min = 1;
        }

        // Floor or Ceil max based on if it's negative or positive.
        max = max < 0 ? (float)Math.Floor(max) : (float)Math.Ceiling(max);

        bool added = false;
        for (float i = min; i < max; i++)
        {
            if (i >= 0)
            {
                result.Add((float)Math.Floor(i));
                added = true;
            }
            else
            {
                if (i < -1) // To avoid adding -0 as an edge.
                {
                    result.Add((float)Math.Ceiling(i));
                    added = true;
                }
            }
        }

        return added;
    }

    /// <summary>
    /// Find the positions where to clip a line segment based on clipping positions.
    /// </summary>
    /// <param name="clipPositions">The positions where to clip.</param>
    /// <param name="aPos">The a in line segment.</param>
    /// <param name="aUV">
    /// The UV coordinate of the a position. This is used to map the clip position to the line segment.
    /// </param>
    /// <param name="bPos">The b in line segment.</param>
    /// <param name="bUV">
    /// The UV coordinate of the b position. This is used to map the clip position to the line segment.
    /// </param>
    /// <param name="result">
    /// The list to populate with the positions where to clip the line segment.
    /// </param>
    public static void FindCLipPositions(
        List<float> clipPositions,
        Vector3 aPos, float aUV,
        Vector3 bPos, float bUV,
        List<Vector3> result
        )
    {
        foreach (float edge in clipPositions)
        {
            float t = MathUtil.Map(edge, aUV, bUV, 0, 1);
            Vector3 p = MathUtil.Lerp(aPos, bPos, t);
            result.Add(p);
        }
    }
}

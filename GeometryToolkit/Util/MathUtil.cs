using System.Numerics;

namespace GeometryToolkit.Util;


/// <summary>
/// Toolbox for common math operations.
/// </summary>
public class MathUtil
{
    /// <summary>
    /// Maps a value from one range to another.
    /// </summary>
    /// <param name="value">The value to map.</param>
    /// <param name="inMin">The input range minimum.</param>
    /// <param name="inMax">The input range maximum.</param>
    /// <param name="outMin">The output range minimum.</param>
    /// <param name="outMax">The output range maximum.</param>
    /// <returns>
    /// The mapped value.
    /// </returns>
    public static float Map(float value, float inMin, float inMax, float outMin, float outMax)
    {
        return (value - inMin) * (outMax - outMin) / (inMax - inMin) + outMin;
    }


    /// <summary>
    /// Linearly interpolates between two numbers.
    /// </summary>
    /// <param name="a">The a.</param>
    /// <param name="b">The b.</param>
    /// <param name="t">
    /// The interpolation factor. Should be between 0 and 1.
    /// </param>
    /// <returns>
    /// The interpolated number.
    /// </returns>
    public static float Lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    /// <summary>
    /// Linearly interpolates between two vectors.
    /// </summary>
    /// <param name="a">The a vector.</param>
    /// <param name="b">The b vector.</param>
    /// <param name="t">
    /// The interpolation factor. Should be between 0 and 1.
    /// </param>
    /// <returns>
    /// The interpolated vector.
    /// </returns>
    public static Vector3 Lerp(Vector3 a, Vector3 b, float t)
    {
        return a + (b - a) * t;
    }
}

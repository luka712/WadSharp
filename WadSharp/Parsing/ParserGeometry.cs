

namespace WadSharp.Parsing;

/// <summary>
/// The sector geometry returned by the parser.
/// </summary>
public class ParserGeometry
{
    /// <summary>
    /// The vertices of the mesh.
    /// </summary>
    public float[] Positions { get; set; } = Array.Empty<float>();

    /// <summary>
    /// The texture coordinates of the mesh.
    /// </summary>
    public float[] TextureCoordinates { get; set; } = Array.Empty<float>();

    /// <summary>
    /// The colors of the mesh.
    /// </summary>
    public float[] Colors { get; set; } = Array.Empty<float>();

    /// <summary>
    /// The indices of the mesh.
    /// </summary>
    public uint[] Indices { get; set; } = Array.Empty<uint>();

    /// <summary>
    /// Merge two geometries into one.
    /// </summary>
    /// <param name="a">The a geometry.</param>
    /// <param name="b">The b </param>
    /// <returns></returns>
    public static ParserGeometry? MergeGeometry(ParserGeometry? a, ParserGeometry? b)
    {
        // If both are null, return null
        if (a == null && b == null)
        {
            return null;
        }
        else if (a is null)
        {
            return b!;
        }
        else if (b is null)
        {
            return a;
        }

        // If both exists, then we can merge.
        List<uint> indices = a.Indices.ToList();
        for (int i = 0; i < b.Indices.Length; i++)
        {
            indices.Add(b.Indices[i] + (uint)a.Positions.Length / 3);
        }

        return new ParserGeometry
        {
            Positions = a.Positions.Concat(b.Positions).ToArray(),
            TextureCoordinates = a.TextureCoordinates.Concat(b.TextureCoordinates).ToArray(),
            Colors = a.Colors.Concat(b.Colors).ToArray(),
            Indices = indices.ToArray()
        };
    }
}

using GeometryToolkit.Vertex;

namespace WadSharp.Parsing;

/// <summary>
/// The sector geometry returned by the parser.
/// </summary>
public class ParserGeometry
{
    /// <summary>
    /// The indices of the mesh.
    /// </summary>
    public uint[] Indices { get; set; } = Array.Empty<uint>();

    /// <summary>
    /// The vertex of the mesh.
    /// </summary>
    public VertexPositionColorTexture[] Vertices = Array.Empty<VertexPositionColorTexture>();

    /// <summary>
    /// Merge two geometries into one.
    /// </summary>
    /// <param name="a">The a geometry.</param>
    /// <param name="b">The b geometry.</param>
    /// <returns>
    /// The merged geometry.
    /// </returns>
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
            indices.Add(b.Indices[i] + (uint)a.Vertices.Length);
        }

        return new ParserGeometry
        {
            Vertices = a.Vertices.Concat(b.Vertices).ToArray(),
            Indices = indices.ToArray()
        };
    }
}

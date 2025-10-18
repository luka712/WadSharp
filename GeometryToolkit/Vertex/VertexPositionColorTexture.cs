using System.Numerics;

namespace GeometryToolkit.Vertex;

/// <summary>
/// The vertex format with position, color, and texture coordinates.
/// </summary>
public struct VertexPositionColorTexture
{
    /// <summary>
    /// The constructor to create a vertex with position, color, and texture coordinates.
    /// </summary>
    /// <param name="position">The position.</param>
    /// <param name="color">The color.</param>
    /// <param name="texCoords">The texture coordinates.</param>
    public VertexPositionColorTexture(Vector3 position, Vector4 color, Vector2 texCoords)
    {
        Position = position;
        Color = color;
        UV = texCoords;
    }

    /// <summary>
    /// The position of the vertex.
    /// </summary>
    public Vector3 Position;

    /// <summary>
    /// The color of the vertex.
    /// </summary>
    public Vector4 Color;

    /// <summary>
    /// Texture coordinates.
    /// </summary>
    public Vector2 UV;
}

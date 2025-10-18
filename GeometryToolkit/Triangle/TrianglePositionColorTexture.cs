using GeometryToolkit.Vertex;
using System.Numerics;

namespace GeometryToolkit.Triangle;

/// <summary>
/// The triangle format with position, color, and texture coordinates.
/// </summary>
public struct TrianglePositionColorTexture
{
    /// <summary>
    /// The first vertex of the triangle.
    /// </summary>
    public VertexPositionColorTexture A { get; set; }

    /// <summary>
    /// The second vertex of the triangle.
    /// </summary>
    public VertexPositionColorTexture B { get; set; }

    /// <summary>
    /// The third vertex of the triangle.
    /// </summary>
    public VertexPositionColorTexture C { get; set; }

    /// <summary>
    /// Gets the normal vector of the triangle.
    /// </summary>
    public Vector3 Normal
    {
        get
        {
            Vector3 ab = B.Position - A.Position;
            Vector3 ac = C.Position - A.Position;
            return Vector3.Normalize(Vector3.Cross(ab, ac));
        }
    }

    /// <summary>
    /// Gets the axis with the largest absolute component in the triangle's normal vector.
    /// </summary>
    /// <returns>
    /// The axis with the largest absolute component (1,0,0 for X, 0,1,0 for Y, or 0,0,1 for Z).
    /// </returns>
    public Vector3 AxisWithLargestAbsComponent()
    {
        Vector3 normal = Normal;
        Vector3 absNormal = new(MathF.Abs(normal.X), MathF.Abs(normal.Y), MathF.Abs(normal.Z));
        if (absNormal.X > absNormal.Y)
        {
            if (absNormal.X > absNormal.Z)
            {
                return new Vector3(1, 0, 0); // X is largest
            }
            else
            {
                return new Vector3(0, 0, 1); // Z is largest
            }
        }
        else
        {
            if (absNormal.Y > absNormal.Z)
            {
                return new Vector3(0, 1, 0); // Y is largest
            }
            else
            {
                return new Vector3(0, 0, 1); // Z is largest
            }
        }
    }

    public IReadOnlyList<Vector2> ProjectTo2D()
    {
        Vector3 axis = AxisWithLargestAbsComponent();
        if (axis.X == 1)
        {
            // Project to YZ plane
            return new List<Vector2>
            {
                new (A.Position.Y, A.Position.Z),
                new (B.Position.Y, B.Position.Z),
                new (C.Position.Y, C.Position.Z)
            };
        }
        else if (axis.Y == 1)
        {
            // Project to XZ plane
            return new List<Vector2>
            {
                new (A.Position.X, A.Position.Z),
                new (B.Position.X, B.Position.Z),
                new (C.Position.X, C.Position.Z)
            };
        }
        else
        {
            // Project to XY plane
            return new List<Vector2>
            {
                new (A.Position.X, A.Position.Y),
                new (B.Position.X, B.Position.Y),
                new (C.Position.X, C.Position.Y)
            };
        }
    }

    /// <summary>
    /// Compares two triangles based on their vertex positions.
    /// </summary>
    /// <param name="t1">Triangle a.</param>
    /// <param name="t2">Triangle b.</param>
    /// <param name="epsilon">Allowed difference between vertex components.</param>
    /// <returns>
    /// <c>true</c> if the triangles are equal based on vertex positions; otherwise, <c>false</c>.
    /// </returns>
    public static bool ArePositionEqual(TrianglePositionColorTexture t1, TrianglePositionColorTexture t2, Double epsilon = 0.0001)
    {
        return Vector3.Distance(t1.A.Position, t2.A.Position) < epsilon &&
               Vector3.Distance(t1.B.Position, t2.B.Position) < epsilon &&
               Vector3.Distance(t1.C.Position, t2.C.Position) < epsilon;
    }
}

using GeometryToolkit.Vertex;
using MadWorldNL.EarCut.Logic;
using MIConvexHull;
using System.Numerics;
using static GeometryToolkit.Triangle.TriangleHelper;

namespace GeometryToolkit.Util
{
    /// <summary>
    /// Toolbox for common mesh operations.
    /// </summary>
    public class MeshUtil
    {
        private const float TOLERANCE = 1e-10f;

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh by projecting them onto the XY, XZ, or YZ plane as appropriate.
        /// </summary>
        /// <param name="positions">
        /// The list of 3D positions to triangulate.
        /// </param>
        /// <param name="projectToVec2">
        /// The function to project a 3D point onto a 2D plane.
        /// </param>
        /// <param name="verticesResult">
        /// The list to be filled with the triangulated vertices.
        /// </param>
        /// <param name="projectToVec3">
        /// The function to project a 2D point back into 3D space.
        /// </param>
        private static void TriangulateProjectedOntoPlane(
            IReadOnlyList<Vector3> positions,
            Func<Vector3, Vector2> projectToVec2,
            List<Vector3> verticesResult,
            Func<Vector2, Vector3> projectToVec3
            )
        {
            IReadOnlyList<Vector2> projected = positions
                  .Select(projectToVec2)
                  .ToList();
            List<Vector2> result = new List<Vector2>();
            Triangulate2D(projected, result);
            foreach (var v in result)
            {
                verticesResult.Add(projectToVec3.Invoke(v));
            }
        }

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh by projecting them onto the XY, XZ, or YZ plane as appropriate.
        /// </summary>
        /// <param name="positions">
        /// The list of 3D positions to triangulate.
        /// </param>
        /// <param name="projectToVec2">
        /// The function to project a 3D point onto a 2D plane.
        /// </param>
        /// <param name="indicesResult">
        /// The list to be filled with the indices of the triangulated vertices.
        /// </param>
        private static void TriangulateProjectedOntoPlane(
            IReadOnlyList<Vector3> positions,
            Func<Vector3, Vector2> projectToVec2,
            List<uint> indicesResult
            )
        {
            IReadOnlyList<Vector2> projected = positions
                  .Select(projectToVec2)
                  .ToList();

            Triangulate2D(projected, indicesResult);
        }

        /// <summary>
        /// Triangulates a simple 2D polygon.
        /// </summary>
        /// <param name="positions">
        /// The list of 2D positions defining the polygon to triangulate.
        /// </param>
        /// <param name="verticesResult">
        /// The list to be filled with the triangulated vertices.
        /// </param>
        public static void Triangulate2D(IReadOnlyList<Vector2> positions, List<Vector2> verticesResult)
        {
            positions = WindingUtil.CounterClockWise(positions);

            double[] points = positions
                .Select(p => new double[] { p.X, p.Y })
                .SelectMany(a => a)
                .ToArray();

            List<int> indices = EarCut.Tessellate(points);
            for (int i = 0; i < indices.Count; i++)
            {
                int idx = indices[i] * 2;
                verticesResult.Add(new Vector2((float)points[idx], (float)points[idx + 1]));
            }
        }

        /// <summary>
        /// Fills the provided lists with the triangulated vertices and indices.
        /// </summary>
        /// <param name="positions">
        /// The list of 2D positions defining the polygon to triangulate.
        /// <param name="indicesResult">
        /// The list to be filled with the indices of the triangulated vertices.
        /// </param>
        public static void Triangulate2D(IReadOnlyList<Vector2> positions, List<uint> indicesResult)
        {
            positions = WindingUtil.CounterClockWise(positions);

            double[] points = positions
                .Select(p => new double[] { p.X, p.Y })
                .SelectMany(a => a)
                .ToArray();

            indicesResult.AddRange(EarCut.Tessellate(points).Select(x => (uint)x));
        }

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh using a convex hull approach.
        /// </summary>
        /// <param name="inputVertices">
        /// The list of 3D positions to triangulate.
        /// </param>
        /// <param name="verticesResult">
        /// The list to be filled with the triangulated vertices.
        /// </param>
        public static void Triangulate(IReadOnlyList<Vector3> inputVertices, List<Vector3> verticesResult)
        {
            // If all points are coplanar on any primary plane, we can handle that case separately.

            // Check if all points are coplanar on XY, XZ, or YZ plane.
            if (inputVertices.All(p => Math.Abs(p.X) < TOLERANCE))
            {
                // Project onto YZ plane
                TriangulateProjectedOntoPlane(
                    inputVertices,
                    p => new Vector2(p.Y, p.Z),
                    verticesResult,
                    p => new Vector3(0, p.X, p.Y)
                    );
                return;
            }
            else if (inputVertices.All(p => Math.Abs(p.Y) < TOLERANCE))
            {
                // Project onto XZ plane
                TriangulateProjectedOntoPlane(
                    inputVertices,
                    p => new Vector2(p.X, p.Z),
                    verticesResult,
                    p => new Vector3(p.X, 0, p.Y)
                    );
                return;
            }
            else if (inputVertices.All(p => Math.Abs(p.Z) < TOLERANCE))
            {
                // Project onto XY plane
                TriangulateProjectedOntoPlane(
                    inputVertices,
                    p => new Vector2(p.X, p.Y),
                    verticesResult,
                    p => new Vector3(p.X, p.Y, 0)
                    );
                return;
            }

            // For general 3D points, use MIConvexHull to compute the convex hull and extract triangles.
            try
            {
                List<Vertex3D> points = inputVertices.Select(p => new Vertex3D(p.X, p.Y, p.Z)).ToList();
                var creationResult = ConvexHull.Create(points, TOLERANCE);
                var mesh = creationResult.Result;
                foreach (var face in mesh.Faces)
                {
                    foreach (var v in face.Vertices)
                    {
                        verticesResult.Add(new Vector3((float)v.Position[0], (float)v.Position[1], (float)v.Position[2]));
                    }
                }
            }
            catch (ConvexHullGenerationException ex)
            {
                Console.WriteLine($"Convex hull generation error: {ex.ErrorMessage}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during triangulation: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh using a convex hull approach.
        /// </summary>
        /// <param name="positions">The list of 3D positions to triangulate.</param>
        /// <param name="outputIndices">The list to be filled with the output indices defining the triangulated mesh connectivity.</param>
        public static void Triangulate(
            IReadOnlyList<Vector3> positions, List<uint> outputIndices)
        {
            // If all points are coplanar on any primary plane, we can handle that case separately.

            // Check if all points are coplanar on XY, XZ, or YZ plane.
            Single maxX = positions.Max(p => p.X);
            Single maxY = positions.Max(p => p.Y);
            Single maxZ = positions.Max(p => p.Z);
            if (positions.All(p => Math.Abs(maxX - p.X) < TOLERANCE))
            {
                // Project onto YZ plane
                TriangulateProjectedOntoPlane(
                    positions,
                    p => new Vector2(p.Y, p.Z),
                    outputIndices);
                return;
            }
            else if (positions.All(p => Math.Abs(maxY - p.Y) < TOLERANCE))
            {
                // Project onto XZ plane
                TriangulateProjectedOntoPlane(
                    positions,
                    p => new Vector2(p.X, p.Z),
                    outputIndices);
                return;
            }
            else if (positions.All(p => Math.Abs(maxZ - p.Z) < TOLERANCE))
            {
                // Project onto XY plane
                TriangulateProjectedOntoPlane(
                    positions,
                    p => new Vector2(p.X, p.Y),
                    outputIndices);
                return;
            }

            // For general 3D points, use MIConvexHull to compute the convex hull and extract triangles.
            try
            {
                List<Vertex3D> points = positions.Select(p => new Vertex3D(p.X, p.Y, p.Z)).ToList();
                var creationResult = ConvexHull.Create(points, TOLERANCE);
                var mesh = creationResult.Result;
                foreach (var face in mesh.Faces)
                {
                    foreach (var v in face.Vertices)
                    {
                        outputIndices.Add((uint)points.IndexOf(v));
                    }
                }
            }
            catch (ConvexHullGenerationException ex)
            {
                Console.WriteLine($"Convex hull generation error: {ex.ErrorMessage}");
                throw;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during triangulation: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh using a convex hull approach.
        /// </summary>
        /// <param name="positions">
        /// The list of 3D positions to triangulate.
        /// </param>
        /// <param name="vertices">
        /// The list to be filled with the triangulated vertices.
        /// </param>
        public static void Triangulate(
            IReadOnlyList<VertexPositionColorTexture> positions,
            List<VertexPositionColorTexture> vertices)
        {
            List<Vector3> posOnly = positions
                .Select(p => p.Position)
                .ToList();

            List<Vector3> triangulated = new();
            Triangulate(posOnly, triangulated);
            foreach (Vector3 v in triangulated)
            {
                // Find the original vertex to copy color and texture from.
                VertexPositionColorTexture? original = positions.FirstOrDefault(p =>
                    Math.Abs(p.Position.X - v.X) < TOLERANCE &&
                    Math.Abs(p.Position.Y - v.Y) < TOLERANCE &&
                    Math.Abs(p.Position.Z - v.Z) < TOLERANCE);

                if (!original.HasValue)
                {
                    string msg = $"Could not find original vertex for position {v.ToString()}.";
                    Console.WriteLine(msg);
                    throw new InvalidOperationException(msg);
                }

                vertices.Add(new VertexPositionColorTexture()
                {
                    Position = v,
                    Color = original.Value.Color,
                    UV = original.Value.UV
                });
            }
        }

        /// <summary>
        /// Converts a set of 3D points into a triangulated mesh using a convex hull approach.
        /// </summary>
        /// <param name="vertices">
        /// The list of 3D vertices  to triangulate.
        /// </param>
        /// <param name="vertices">
        /// The list to be filled with the triangulated vertices.
        /// </param>
        public static void Triangulate(
            IReadOnlyList<VertexPositionColorTexture> vertices,
            List<uint> outputIndices
            )
        {
            List<Vector3> posOnly = vertices
                .Select(p => p.Position)
                .ToList();

            List<Vector3> triangulated = new();
            Triangulate(posOnly, outputIndices);
        }
    }
}

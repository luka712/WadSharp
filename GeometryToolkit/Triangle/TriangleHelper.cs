using GeometryToolkit.Util;
using MadWorldNL.EarCut.Logic;
using MIConvexHull;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GeometryToolkit.Triangle
{
    public class TriangleHelper
    {
        private const float Tolerance = 1e-10f;

        // Custom vertex class implementing IVertex (for 3D coordinates)
        public class Vertex3D : IVertex
        {
            /// <inheritdoc/>
            public double[] Position { get; }

            public Vertex3D(double x, double y, double z)
            {
                Position = [ x, y, z ];
            }

            /// <inheritdoc/>
            public override bool Equals(object obj) => obj is Vertex3D v && Position.SequenceEqual(v.Position);

            /// <inheritdoc/>
            public override int GetHashCode() => Position.GetHashCode();
        }

        // Custom cell class for tetrahedra (implements default triangulation cell)
        public class Cell3D : TriangulationCell<Vertex3D, Cell3D>
        {
            // No additional properties needed for basic use
        }

        /// <summary>
        /// Offsets points that are all coplanar on the XY plane by a small random Z value to avoid issues with 3D Delaunay triangulation.
        /// </summary>
        /// <param name="positions">
        /// The list of 3D positions to check and potentially offset.
        /// </param>
        /// <returns>
        /// The original list if not all points are coplanar on the XY plane; otherwise, a new list with slight Z offsets applied.
        /// </returns>
        private static bool IsFlat(IReadOnlyList<Vector3> positions)
        {
            return positions.All(p => Math.Abs(p.Z) < Tolerance);
        }

        public static float SignedArea(IReadOnlyList<Vector2> vertices)
        {
            float area = 0;
            int n = vertices.Count;
            for (int i = 0; i < n; i++)
            {
                Vector2 current = vertices[i];
                Vector2 next = vertices[(i + 1) % n];
                area += (current.X * next.Y - next.X * current.Y);
            }
            return area * 0.5f;
        }



      
    }

}

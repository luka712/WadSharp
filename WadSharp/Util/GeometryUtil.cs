using Clipper2Lib;
using GeometryToolkit.Triangle;
using GeometryToolkit.UV;
using SharpGLTF.Schema2;
using System;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace WadSharp.Util
{
  

    public class GeometryUtil
    {
        public enum TriangleEdge
        {
            AB,
            BC,
            CA
        }


        public class ClipInfo
        {
            /// <summary>
            /// The texture coordinate axis to clip on.
            /// </summary>
            public TexCoordComponent Axis { get; set; }

            public TriangleEdge Edge;

            public List<Vector3> Positions = new();
        }

        /// <summary>
        /// Finds where to clip the triangle based on its UV coordinates.
        /// </summary>
        /// <param name="triangle"></param>
        /// <returns></returns>

        public static ClipInfo? FindWhereToClip(TrianglePositionColorTexture triangle)
        {
            var a = triangle.A;
            var b = triangle.B;
            var c = triangle.C;

            // Find uv points where triangle should be split.
            List<float> coordsOutsideDesiredRange = new();

            // Split where more edges are found.
            if (UVHelper.FindCoordsOutsideOfNormalUVRange(a.UV.X, b.UV.X, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, a.Position, a.UV.X, b.Position, b.UV.X, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.U,
                    Edge = TriangleEdge.AB,
                    Positions = positions
                };
            }
            else if (UVHelper.FindCoordsOutsideOfNormalUVRange(a.UV.Y, b.UV.Y, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, a.Position, a.UV.Y, b.Position, b.UV.Y, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.V,
                    Edge = TriangleEdge.AB,
                    Positions = positions
                };
            }
            else if (UVHelper.FindCoordsOutsideOfNormalUVRange(b.UV.X, c.UV.X, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, b.Position, b.UV.X, c.Position, c.UV.X, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.U,
                    Edge = TriangleEdge.BC,
                    Positions = positions
                };
            }
            else if (UVHelper.FindCoordsOutsideOfNormalUVRange(b.UV.Y, c.UV.Y, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, b.Position, b.UV.Y, c.Position, c.UV.Y, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.V,
                    Edge = TriangleEdge.BC,
                    Positions = positions
                };
            }
            else if (UVHelper.FindCoordsOutsideOfNormalUVRange(c.UV.X, a.UV.X, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, c.Position, c.UV.X, a.Position, a.UV.X, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.U,
                    Edge = TriangleEdge.CA,
                    Positions = positions
                };
            }
            else if (UVHelper.FindCoordsOutsideOfNormalUVRange(c.UV.Y, a.UV.Y, coordsOutsideDesiredRange))
            {
                List<Vector3> positions = new();
                UVHelper.FindCLipPositions(coordsOutsideDesiredRange, c.Position, c.UV.Y, a.Position, a.UV.Y, positions);
                return new ClipInfo
                {
                    Axis = TexCoordComponent.V,
                    Edge = TriangleEdge.CA,
                    Positions = positions
                };
            }


            return null;
        }
    }
}

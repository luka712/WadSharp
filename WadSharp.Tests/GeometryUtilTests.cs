using GeometryToolkit.Triangle;
using GeometryToolkit.Util;
using GeometryToolkit.UV;
using GeometryToolkit.Vertex;
using System.Numerics;
using WadSharp.Util;

namespace WadSharp.Tests
{
    public class GeometryUtilTests
    {
        [SetUp]
        public void Setup()
        {
        }

        private void AssertCloseTo(Double expected, Double actual, Double tolerance = 0.0001)
        {
            Assert.That(actual, Is.InRange(expected - tolerance, expected + tolerance));
        }

        /// <summary>
        /// Test finding UV edges.
        /// </summary>
        [Test]
        public void Test_Find_UV_Edges_A()
        {
            List<float> result = new();
            // Test case -2.3 to 3.2.
            // Result should be -2, -1, 0, 1, 2, 3.
            UVHelper.FindCoordsOutsideOfNormalUVRange(-2.3f, 3.2f, result);

            Assert.That(result.Count, Is.EqualTo(6));
            AssertCloseTo(-2, result[0]);
            AssertCloseTo(-1, result[1]);
            AssertCloseTo(0, result[2]);
            AssertCloseTo(1, result[3]);
            AssertCloseTo(2, result[4]);
            AssertCloseTo(3, result[5]);
        }

        /// <summary>
        /// Test finding UV edges.
        /// </summary>
        [Test]
        public void Test_Find_UV_Edges_B()
        {
            List<float> result = new();
            // Test case -10.3 to -4.2.
            // Result should be -10, -9, -8, -7, -6, -5.
            UVHelper.FindCoordsOutsideOfNormalUVRange(-10.3f, -4.2f, result);

            Assert.That(result.Count, Is.EqualTo(6));
            AssertCloseTo(-10, result[0]);
            AssertCloseTo(-9, result[1]);
            AssertCloseTo(-8, result[2]);
            AssertCloseTo(-7, result[3]);
            AssertCloseTo(-6, result[4]);
            AssertCloseTo(-5, result[5]);
        }

        /// <summary>
        /// Test finding UV edges.
        /// </summary>
        [Test]
        public void Test_Find_UV_Edges_C()
        {
            List<float> result = new();
            // Test case 2.3 to 7.4.
            // Result should be 3,4,5,6,7
            UVHelper.FindCoordsOutsideOfNormalUVRange(2.3f, 7.4f, result);

            Assert.That(result.Count, Is.EqualTo(5));
            AssertCloseTo(3, result[0]);
            AssertCloseTo(4, result[1]);
            AssertCloseTo(5, result[2]);
            AssertCloseTo(6, result[3]);
            AssertCloseTo(7, result[4]);
        }

        /// <summary>
        /// Test for triangle defined by A(0,0,0) B(2,0,0) C(0,1,0). 
        /// Clipping should occur at position P(1,0,0) where U is (1,0).
        /// </summary>
        [Test]
        public void Test_Find_Where_To_Clip_A()
        {
            var clipInfo = GeometryUtil.FindWhereToClip(
                new TrianglePositionColorTexture()
                {
                    A = new(new(0, 0, 0), new(1, 1, 1, 1), new(0f, 0)),
                    B = new(new(2, 0, 0), new(1, 1, 1, 1), new(2, 0)),
                    C = new(new(0, 1, 0), new(1, 1, 1, 1), new(0, 1))
                }
            );

            Assert.NotNull(clipInfo);
            AssertCloseTo(1f, clipInfo!.Positions[0].X);
            AssertCloseTo(0f, clipInfo!.Positions[0].Y);
            AssertCloseTo(0f, clipInfo!.Positions[0].Z);
        }

        /// <summary>
        /// Test for triangle defined by A(0,-1.5,0) B(0,1.5,0) C(1,0,0). 
        /// Clipping should occur at positions:
        /// - P(0,-1, 0) where V coords is -1
        /// - P(0, 0, 0) where V coords is 0
        /// - P(0, 1, 0) where V coords is 1
        /// </summary>
        [Test]
        public void Test_Find_Where_To_Clip_B()
        {
            var clipInfo = GeometryUtil.FindWhereToClip(
                new TrianglePositionColorTexture()
                {
                    A = new(new(0, -1.5f, 0), new(1, 1, 1, 1), new(0f, -1.5f)),
                    B = new(new(0, 1.5f, 0), new(1, 1, 1, 1), new(0, 1.5f)),
                    C = new(new(1, 0, 0), new(1, 1, 1, 1), new(1, 0))
                }
            );

            Assert.NotNull(clipInfo);
            AssertCloseTo(0f, clipInfo!.Positions[0].X);
            AssertCloseTo(-1f, clipInfo!.Positions[0].Y);
            AssertCloseTo(0f, clipInfo!.Positions[0].Z);
            AssertCloseTo(0f, clipInfo!.Positions[1].X);
            AssertCloseTo(0f, clipInfo!.Positions[1].Y);
            AssertCloseTo(0f, clipInfo!.Positions[1].Z);
            AssertCloseTo(0f, clipInfo!.Positions[2].X);
            AssertCloseTo(1f, clipInfo!.Positions[2].Y);
            AssertCloseTo(0f, clipInfo!.Positions[2].Z);
        }

        /// <summary>
        /// Given a quad, test triangulation.
        /// </summary>
        [Test]
        public void Test_Triangulate2DQuadVerticesResult()
        {
            List<Vector2> quad = new()
            {
               new(0, 0),
               new(1, 0),
               new(0, 1),
               new(1, 1),
            };

            List<Vector2> points = new();
            MeshUtil.Triangulate2D(quad, points);

            Assert.That(points.Count, Is.EqualTo(6));
            AssertCloseTo(1, points[0].X);
            AssertCloseTo(1, points[0].Y);
            AssertCloseTo(0, points[1].X);
            AssertCloseTo(1, points[1].Y);
            AssertCloseTo(0, points[2].X);
            AssertCloseTo(0, points[2].Y);
            AssertCloseTo(0, points[3].X);
            AssertCloseTo(0, points[3].Y);
            AssertCloseTo(1, points[4].X);
            AssertCloseTo(0, points[4].Y);
            AssertCloseTo(1, points[5].X);
            AssertCloseTo(1, points[5].Y);
        }

        /// <summary>
        /// Given a quad, test triangulation.
        /// </summary>
        [Test]
        public void Test_Triangulate2DQuadIndicesResult()
        {
            List<Vector2> quad = new()
            {
               new(0, 0),
               new(1, 0),
               new(0, 1),
               new(1, 1),
            };

            List<uint> indices = new();
            MeshUtil.Triangulate2D(quad, indices);

            Assert.That(indices.Count, Is.EqualTo(6));
            Assert.That(indices[0], Is.EqualTo(2));
            Assert.That(indices[1], Is.EqualTo(3));
            Assert.That(indices[2], Is.EqualTo(0));
            Assert.That(indices[3], Is.EqualTo(0));
            Assert.That(indices[4], Is.EqualTo(1));
            Assert.That(indices[5], Is.EqualTo(2));
        }

        /// <summary>
        /// Given a quad, test triangulation.
        /// </summary>
        [Test]
        public void Test_Triangulate3DQuadVerticesResult()
        {
            List<Vector3> cube = new()
            {
                new (0, 0, 0),
                new (1, 0, 0),
                new (0, 1, 0),
                new (1, 1, 0),

                new (0, 0, 1),
                new (0, 1, 0),
                new (1, 0, 1),
                new (1, 1, 1),
            };

            List<Vector3> points = new();
            MeshUtil.Triangulate(cube, points);

            Assert.That(points.Count, Is.EqualTo(30));
        }

        /// <summary>
        /// Given a quad with <see cref="VertexPositionColorTexture"/>, test triangulation.
        /// </summary>
        [Test]
        public void Test_Triangulate3DQuadVertexPositionColorTextureResult()
        {
            List<VertexPositionColorTexture> cube = new()
            {
                new VertexPositionColorTexture(){ Position = new (0, 0, 0) },
                new VertexPositionColorTexture(){ Position = new (1, 0, 0) },
                new VertexPositionColorTexture(){ Position = new (0, 1, 0) },
                new VertexPositionColorTexture(){ Position = new (1, 1, 0) },

                new VertexPositionColorTexture{ Position = new (0, 0, 1) },
                new VertexPositionColorTexture{ Position = new (0, 1, 0) },
                new VertexPositionColorTexture{ Position = new (1, 0, 1) },
                new VertexPositionColorTexture{ Position = new (1, 1, 1) }
            };

            List<VertexPositionColorTexture> points = new();
            MeshUtil.Triangulate(cube, points);

            Assert.That(points.Count, Is.EqualTo(30));
        }
    }
}
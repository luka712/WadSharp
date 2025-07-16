namespace WadSharp.Tests
{
    public class Tests
    {
        private const string TestWadPath = "Content/freedoom-0.13.0/freedoom1_gl.wad";

        [SetUp]
        public void Setup()
        {
        }

        /// <summary>
        /// Just see if the header is correct.
        /// </summary>
        [Test]
        public async Task AssertHeaderIsCorrect()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);

            Assert.That(new string(wad.Identification), Is.EqualTo("IWAD"));
        }

        /// <summary>
        /// Assert the directories are correct.
        /// </summary>
        [Test]
        public async Task AssertDirectoryIsCorrect()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);

            Assert.NotNull(wad.Directories);
            Assert.True(wad.Directories.Exists(x => new string(x.Name) == "VERTEXES"));
        }

        /// <summary>
        /// Assert the VERTEXES are correct.
        /// </summary>
        [Test]
        public async Task AssertVertexesAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(wad);
            Assert.True(level.Vertices.Count > 0);
        }

        /// <summary>
        /// Assert the LINEDEFS are correct.
        /// </summary>
        [Test]
        public async Task AssertLineDefsAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.LineDefs.Count > 0);
        }

        /// <summary>
        /// Assert the SUB_SECTORS are correct.
        /// </summary>
        [Test]
        public async Task AssertSubSectorsAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.SubSectors.Count > 0);
        }

        /// <summary>
        /// Assert the NODES are correct.
        /// </summary>
        [Test]
        public async Task AssertNodesAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.Nodes.Count > 0);
        }

        /// <summary>
        /// Assert the Segs are correct.
        /// </summary>
        [Test]
        public async Task AssertSegsAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.Segs.Count > 0);
        }

        /// <summary>
        /// Assert the GL_VERTICES are correct.
        /// </summary>
        [Test]
        public async Task AssertGLVerticesAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.GLVertices.Count > 0);
        }

        /// <summary>
        /// Assert the Segs are correct.
        /// </summary>
        [Test]
        public async Task AssertGLSegsAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.Segs.Count > 0);
        }

        /// <summary>
        /// Assert the GLNodes are correct.
        /// </summary>
        [Test]
        public async Task AssertGLNodesAreLoaded()
        {
            WADLoader loader = new();

            WAD wad = await loader.LoadAsync(TestWadPath);
            WADLevel level = wad.LoadLevel("E1M1");

            Assert.NotNull(level);
            Assert.True(level.GLNodes.Count > 0);
        }
    }
}
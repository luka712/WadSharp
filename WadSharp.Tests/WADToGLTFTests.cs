
using WadSharp;
using WadSharp.GLTF;

namespace AssetToolkit.Test;

/// <summary>
/// The test for <see cref="WADToGLTF"/>.
/// </summary>
public class WADToGLTFTests
{
    private const string TestIWadPath = "Content/freedoom-0.13.0/freedoom1_gl.wad";

    private const string TestPWadPath = "Content/freedoom-0.13.0/freedoom1_pwad_gl.wad";

    /// <summary>
    /// Just see if the header is correct.
    /// </summary>
    [Test]
    public async Task AssertWADIsConvertedToGLTF()
    {
        WADLoader loader = new();

        WAD wad = await loader.LoadAsync(TestIWadPath);
        WADLevel level = wad.LoadLevel("E1M1");

        WadToGltf wadToGltf = new();
        wadToGltf.ToGLTF(level, "Content/freedoom");

        Assert.True(File.Exists("Content/freedoom.gltf"));
    }

    /// <summary>
    /// Just see if the header is correct.
    /// </summary>
    [Test]
    public async Task AssertPWADIsConvertedToGLTF()
    {
        WADLoader loader = new();

        WAD wad = await loader.LoadAsync(TestIWadPath, TestPWadPath);
        WADLevel level = wad.LoadLevel("MAP01");

        WadToGltf wadToGltf = new();
        wadToGltf.ToGLTF(level, "Content/pwad_freedoom");

        Assert.True(File.Exists("Content/pwad_freedoom.gltf"));
    }
}
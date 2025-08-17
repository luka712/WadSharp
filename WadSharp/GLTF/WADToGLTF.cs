using AssetToolkit.Image;
using SharpGLTF.Schema2;
using System.Numerics;
using WadSharp.GLTF.Data;
using WadSharp.Parsing;

namespace WadSharp.GLTF;

/// <summary>
/// Service to convert a WAD level to GLTF.
/// </summary>
public class WadToGltf
{
    private readonly ImageService imgService = new();

    /// <summary>
    /// Convert a array to a <see cref="Vector4"/> starting at the given index.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="startIndex">The start index, or index of X.</param>
    /// <returns>The <see cref="Vector4"/>.</returns>
    private static Vector4 ToVector4(float[] array, int startIndex)
        => new Vector4(array[startIndex + 0],
                       array[startIndex + 1],
                       array[startIndex + 2],
                       array[startIndex + 3]
                       );

    /// <summary>
    /// Convert a array to a <see cref="Vector3"/> starting at the given index.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="startIndex">The start index, or index of X.</param>
    /// <returns>The <see cref="Vector3"/>.</returns>
    private static Vector3 ToVector3(float[] array, int startIndex)
        => new Vector3(array[startIndex + 0],
                       array[startIndex + 1],
                       array[startIndex + 2]);

    /// <summary>
    /// Convert a array to a <see cref="Vector2"/> starting at the given index.
    /// </summary>
    /// <param name="array">The array.</param>
    /// <param name="startIndex">The start index, or index of X.</param>
    /// <returns>The <see cref="Vector2"/>.</returns>
    private static Vector2 ToVector2(float[] array, int startIndex)
        => new Vector2(array[startIndex + 0],
                       array[startIndex + 1]);

    /// <summary>
    /// Convert a position array to an array of <see cref="Vector3"/>s.
    /// </summary>
    /// <param name="array">The positions <see cref="float"/> array.</param>
    /// <returns>The <see cref="Vector3"/> array.</returns>
    private static Vector3[] ToVector3Array(float[] array)
    {
        Vector3[] vectorArray = new Vector3[array.Length / 3];
        for (int i = 0; i < vectorArray.Length; i++)
        {
            vectorArray[i] = ToVector3(array, i * 3);
        }
        return vectorArray;
    }

    /// <summary>
    /// Convert a position array to an array of <see cref="Vector4"/>s.
    /// </summary>
    /// <param name="array">The positions <see cref="float"/> array.</param>
    /// <returns>The <see cref="Vector4"/> array.</returns>
    private static Vector4[] ToVector4Array(float[] array)
    {
        Vector4[] vectorArray = new Vector4[array.Length / 4];
        for (int i = 0; i < vectorArray.Length; i++)
        {
            vectorArray[i] = ToVector4(array, i * 4);
        }
        return vectorArray;
    }

    /// <summary>
    /// Convert a position array to an array of <see cref="Vector3"/>s.
    /// </summary>
    /// <param name="positions">The positions <see cref="float"/> array.</param>
    /// <returns>The <see cref="Vector3"/> array.</returns>
    private static Vector2[] ToVector2Array(float[] positions)
    {
        Vector2[] vectorArray = new Vector2[positions.Length / 2];
        for (int i = 0; i < vectorArray.Length; i++)
        {
            vectorArray[i] = ToVector2(positions, i * 2);
        }
        return vectorArray;
    }

    private WADToGLTFNodeData CreateNode(
        ModelRoot model,
        ParserSector sector,
        List<ParserImage> images,
        string image,
        float[] posVertices,
        float[] textureCoordsVertices,
        float[] colorsVertices,
        uint[] indices)
    {
        ParserImage parserImage = images.First(x => x.Name == image);
        bool hasTransparency = parserImage.HasTransparency;
        AddImageToModel(parserImage, model);

        Material material = model.CreateMaterial(image)
            .WithDoubleSide(hasTransparency) // If we have transparency, we need double sided.
            .WithUnlit()
            .WithChannelTexture("BaseColor", 0, model.LogicalImages.First(x => x.Name == image));

        if (hasTransparency)
        {
            material.Alpha = AlphaMode.BLEND;
        }

        // Create mesh
        Vector3[] positions = ToVector3Array(posVertices);
        Vector2[] texCoords = ToVector2Array(textureCoordsVertices);
        Vector4[] colors = ToVector4Array(colorsVertices);
        Mesh mesh = model.CreateMesh($"Floor {sector.Id}");
        int[] indicesInt = indices.Select(x => (int)x).ToArray();
        mesh.CreatePrimitive()
            .WithVertexAccessor("POSITION", positions)
            .WithVertexAccessor("TEXCOORD_0", texCoords)
            .WithVertexAccessor("COLOR_0", colors)
            .WithMaterial(material)
            .WithIndicesAccessor(PrimitiveType.TRIANGLES, indicesInt);

        return new (mesh, material, hasTransparency, sector);
    }


    /// <summary>
    /// Adds images to the model.
    /// </summary>
    /// <param name="images">Wad images.</param>
    /// <param name="model">The gltf root model.</param>
    private void AddImageToModel(
        ParserImage image,
        ModelRoot model)
    {
        if (!model.LogicalImages.Any(x => x.Name == image.Name))
        {
            imgService.SavePngImage(image.Name, image.Data, image.Width, image.Height);
            model.CreateImage(image.Name).Content = new SharpGLTF.Memory.MemoryImage($"{image.Name}.png");
        }
    }

    private List<ParserImage> GetImages(Parser parser, WADLevel level)
    {
        List<ParserImage> wallTextures = parser.ParsePatches(level.Patches, level.PlayPals[0]);
        List<ParserImage> flatTextures = parser.ParseFlats(level.Flats, level.PlayPals[0]);
        List<ParserImage> textures = parser.ParseTextures(level.Texture2s,
            level.PNames,
            level.Patches,
            level.PlayPals[0]);
        List<ParserImage> textures2 = parser.ParseTextures(level.Texture1s,
            level.PNames,
            level.Patches,
            level.PlayPals[0]);

        return wallTextures
            .Concat(flatTextures)
            .Concat(textures)
            .Concat(textures2)
            .ToList();
    }

    /// <summary>
    /// Converts a WAD level to GLTF format and saves it to the specified path.
    /// </summary>
    /// <param name="level">The <see cref="WADLevel"/>.</param>
    /// <param name="destinationPath"></param>
    public void ToGLTF(WADLevel level, string destinationPath)
    {
        if(destinationPath.EndsWith(".gltf", StringComparison.OrdinalIgnoreCase))
        {
            destinationPath = destinationPath[..^5]; // Remove .gltf extension if present
        }

        Parser parser = new();

        List<ParserSector> sectors = parser.ParseSectors(level);
        List<ParserImage> images = GetImages(parser, level);

        Dictionary<string, Material> materials = new();

        ModelRoot model = ModelRoot.CreateModel();
        Scene scene = model.UseScene(0);

        List<WADToGLTFNodeData> nodeDataList = new();

        foreach (ParserSector sector in sectors)
        {
            if (sector.FloorGeometry is not null)
            {
                WADToGLTFNodeData nodeData = CreateNode(
                    model,
                    sector,
                    images,
                    sector.FloorImage ?? "",
                    sector.FloorGeometry.Positions,
                    sector.FloorGeometry.TextureCoordinates,
                    sector.FloorGeometry.Colors,
                    sector.FloorGeometry.Indices);
                nodeDataList.Add(nodeData);
            }

            if (sector.CeilingGeometry is not null)
            {
                WADToGLTFNodeData nodeData = CreateNode(
                    model,
                    sector,
                    images,
                    sector.CeilingImage ?? "",
                    sector.CeilingGeometry.Positions,
                    sector.CeilingGeometry.TextureCoordinates,
                    sector.CeilingGeometry.Colors,
                    sector.CeilingGeometry.Indices);
                nodeDataList.Add(nodeData);
            }

            foreach (ParserSectorWall wall in sector.Walls)
            {
                WADToGLTFNodeData nodeData = CreateNode(
                    model,
                    sector,
                    images,
                    wall.Texture ?? "",
                    wall.Geometry.Positions,
                    wall.Geometry.TextureCoordinates,
                    wall.Geometry.Colors,
                    wall.Geometry.Indices);
                nodeDataList.Add(nodeData);
            }
        }

        foreach(WADToGLTFNodeData nodeData in nodeDataList.OrderBy(x => x.hasTransparency))
        {
            scene.CreateNode($"Sector_{nodeData.wadSector.Id}")
                .WithMesh(nodeData.mesh);
        }

        model.SaveGLTF($"{destinationPath}.gltf", new WriteSettings());
    }
}

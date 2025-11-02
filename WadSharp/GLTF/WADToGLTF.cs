using AssetToolkit.Image;
using GeometryToolkit.Util;
using GeometryToolkit.UV;
using GeometryToolkit.Vertex;
using SharpGLTF.Schema2;
using System.Numerics;
using WadSharp.GLTF.Data;
using WadSharp.Parsing;
using WadSharp.Util;

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

    private WADToGLTFNodeData CreateNode(
        ModelRoot model,
        ParserSector sector,
        SheetCollection sheetCollection,
        string imageName,
        VertexPositionColorTexture[] vertices,
        uint[] indices)
    {
        SheetSprite sprite = sheetCollection.GetSprite(imageName);
        ParserImage parserImage = sprite.Image!;
        bool hasTransparency = parserImage.HasTransparency;
        AddImageToModel(sprite, model);

        Material material = model.CreateMaterial(imageName)
            .WithDoubleSide(hasTransparency) // If we have transparency, we need double sided.
            .WithUnlit()
            .WithChannelTexture("BaseColor", 0, model.LogicalImages.First(x => x.Name == sprite.Sheet.Name));

        if (hasTransparency)
        {
            material.Alpha = AlphaMode.BLEND;
        }

 
        Vector3[] positions = vertices.Select(v => v.Position).ToArray();
        Vector2[] texCoords = vertices.Select(v => v.UV).ToArray(); // MapTextureCoordsToSheet(sprite, vertices.Select(x => x.UV).ToArray());
        Vector4[] colors = vertices.Select(v => v.Color).ToArray();

        float xMin = texCoords.Min(tc => tc.X);
        float xMax = texCoords.Max(tc => tc.X);
        List<float> result = new();

        List<uint> newIndices = new List<uint>();
        MeshUtil.Triangulate(vertices, newIndices);

        // Create mesh
    

        Mesh mesh = model.CreateMesh($"Floor {sector.Id}");
        int[] indicesInt = newIndices.Select(x => (int)x).ToArray();
        mesh.CreatePrimitive()
            .WithVertexAccessor("POSITION", positions)
            .WithVertexAccessor("TEXCOORD_0", texCoords)
            .WithVertexAccessor("COLOR_0", colors)
            .WithMaterial(material)
            .WithIndicesAccessor(PrimitiveType.TRIANGLES, indicesInt);

        return new(mesh, material, hasTransparency, sector);
    }


    /// <summary>
    /// Take sheet sprite and adds the sheet image of the sprite to the model if it does not already exist.
    /// </summary>
    /// <param name="sheetSprite">The sprite in a sheet.</param>
    /// <param name="model">The gltf root model.</param>
    private void AddImageToModel(
        SheetSprite sheetSprite,
        ModelRoot model)
    {
        Sheet sheet = sheetSprite.Sheet;
        if (!model.LogicalImages.Any(x => x.Name == sheet.Name))
        {
            imgService.SavePngImage(sheet.Name, sheet.Data, sheet.Width, sheet.Height);
            model.CreateImage(sheet.Name).Content = new SharpGLTF.Memory.MemoryImage($"{sheet.Name}.png");
        }
    }

    /// <summary>
    /// Creates a sheet collection of images from given parser and level.
    /// </summary>
    /// <param name="parser">The <see cref="Parser"/>.</param>
    /// <param name="level">The <see cref="WADLevel"/>.</param>
    /// <returns>
    /// The <see cref="SheetCollection"/>.
    /// </returns>
    private SheetCollection CreateSheetCollection(Parser parser, WADLevel level)
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

        List<ParserImage> images = wallTextures
            .Concat(flatTextures)
            .Concat(textures)
            .Concat(textures2)
            .ToList();

        return SheetCollection.Create(images);
    }

    /// <summary>
    /// Map the texture coordinates for single image from DOOM to the texture coordinates of the sprite in the sheet.
    /// Effectively remaps the 0-1 UV coordinates to the correct coordinates in the sprite sheet.
    /// </summary>
    /// <param name="sprite">The <see cref="SheetSprite"/> which is sprite in a sheet.</param>
    /// <param name="textureCoordinates">
    /// The texture coordinates to map, in the range of 0-1 for the original image.
    /// </param>
    /// <returns>
    /// The new texture coordinates mapped to the sprite sheet.
    /// </returns>
    private float[] MapTextureCoordsToSheet(SheetSprite sprite, float[] textureCoordinates)
    {
        float[] newTexCoords = new float[textureCoordinates.Length];

        float minU = float.PositiveInfinity;
        float maxU = float.NegativeInfinity;
        float minV = float.PositiveInfinity;
        float maxV = float.NegativeInfinity;

        for (int i = 0; i < textureCoordinates.Length / 2; i++)
        {
            float u = textureCoordinates[i * 2 + 0];
            float v = textureCoordinates[i * 2 + 1];

            if (u < minU) minU = u;
            if (u > maxU) maxU = u;
            if (v < minV) minV = v;
            if (v > maxV) maxV = v;
        }

        for (int i = 0; i < textureCoordinates.Length / 2; i++)
        {
            float u = textureCoordinates[i * 2 + 0];
            float v = textureCoordinates[i * 2 + 1];

            newTexCoords[i * 2 + 0] = MathUtil.Map(u, minU, maxU, sprite.U0, sprite.U1);
            newTexCoords[i * 2 + 1] = MathUtil.Map(v, minV, maxV, sprite.V0, sprite.V1);
        }
        return newTexCoords;
    }

    /// <summary>
    /// Map the texture coordinates for single image from DOOM to the texture coordinates of the sprite in the sheet.
    /// Effectively remaps the 0-1 UV coordinates to the correct coordinates in the sprite sheet.
    /// </summary>
    /// <param name="sprite">The <see cref="SheetSprite"/> which is sprite in a sheet.</param>
    /// <param name="textureCoordinates">
    /// The texture coordinates to map, in the range of 0-1 for the original image.
    /// </param>
    /// <returns>
    /// The new texture coordinates mapped to the sprite sheet.
    /// </returns>
    private Vector2[] MapTextureCoordsToSheet(SheetSprite sprite, Vector2[] textureCoordinates)
    {
        Vector2[] newTexCoords = new Vector2[textureCoordinates.Length];

        float minU = float.PositiveInfinity;
        float maxU = float.NegativeInfinity;
        float minV = float.PositiveInfinity;
        float maxV = float.NegativeInfinity;

        for (int i = 0; i < textureCoordinates.Length; i++)
        {
            float u = textureCoordinates[i].X;
            float v = textureCoordinates[i].Y;

            if (u < minU) minU = u;
            if (u > maxU) maxU = u;
            if (v < minV) minV = v;
            if (v > maxV) maxV = v;
        }

        for (int i = 0; i < textureCoordinates.Length; i++)
        {
            float u = textureCoordinates[i].X;
            float v = textureCoordinates[i].Y;

            u = MathUtil.Map(u, minU, maxU, sprite.U0, sprite.U1);
            v = MathUtil.Map(v, minV, maxV, sprite.V0, sprite.V1);
            newTexCoords[i] = new Vector2(u, v);
        }
        return newTexCoords;
    }

    /// <summary>
    /// Converts a WAD level to GLTF format and saves it to the specified path.
    /// </summary>
    /// <param name="level">The <see cref="WADLevel"/>.</param>
    /// <param name="destinationPath"></param>
    public void ToGLTF(WADLevel level, string destinationPath)
    {
        if (destinationPath.EndsWith(".gltf", StringComparison.OrdinalIgnoreCase))
        {
            destinationPath = destinationPath[..^5]; // Remove .gltf extension if present
        }

        Parser parser = new();

        List<ParserSector> sectors = parser.ParseSectors(level);
        SheetCollection sheetCollection = CreateSheetCollection(parser, level);

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
                    sheetCollection,
                    sector.FloorImage ?? "",
                    sector.FloorGeometry.Vertices,
                    sector.FloorGeometry.Indices);
                nodeDataList.Add(nodeData);
            }

            if (sector.CeilingGeometry is not null)
            {
                WADToGLTFNodeData nodeData = CreateNode(
                    model,
                    sector,
                    sheetCollection,
                    sector.CeilingImage ?? "",
                    sector.CeilingGeometry.Vertices,
                    sector.CeilingGeometry.Indices);
                nodeDataList.Add(nodeData);
            }

            foreach (ParserSectorWall wall in sector.Walls)
            {
                try
                {
                    WADToGLTFNodeData nodeData = CreateNode(
                        model,
                        sector,
                        sheetCollection,
                        wall.Texture ?? "",
                        wall.Geometry.Vertices,
                        wall.Geometry.Indices);
                    nodeDataList.Add(nodeData);
                }
                catch(Exception ex)
                {
                    Console.WriteLine($"Failed to create wall for sector {sector.Id} with texture {wall.Texture}: {ex.Message}");
                }
            }
        }

        foreach (WADToGLTFNodeData nodeData in nodeDataList.OrderBy(x => x.hasTransparency))
        {
            scene.CreateNode($"Sector_{nodeData.wadSector.Id}")
                .WithMesh(nodeData.mesh);
        }

        model.SaveGLTF($"{destinationPath}.gltf", new WriteSettings());
    }
}

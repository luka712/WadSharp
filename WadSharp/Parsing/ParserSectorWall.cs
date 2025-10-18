using GeometryToolkit.Vertex;
using System.Collections.Generic;
using System.Numerics;
using WadSharp.Parts;

namespace WadSharp.Parsing;


/// <summary>
/// The wall of a sector.
/// </summary>
public class ParserSectorWall
{
    /// <summary>
    /// The geometry of the wall.
    /// </summary>
    public ParserGeometry Geometry { get; set; } = null!;

    /// <summary>
    /// The name of a texture for a given wall.
    /// </summary>
    public string Texture { get; set; } = string.Empty;

    /// <summary>
    /// Creates vertices from positions, texture coordinates and colors.
    /// </summary>
    /// <param name="positions">Positions.</param>
    /// <param name="textureCoords">Texture coordinates.</param>
    /// <param name="colors">Colors.</param>
    /// <returns>
    /// The created vertices.
    /// </returns>
    private static VertexPositionColorTexture[] CreateVertices(
        IReadOnlyList<float> positions,
        IReadOnlyList<float> textureCoords,
        IReadOnlyList<float> colors)
    {
        int count = positions.Count / 3;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[count];
        for (int i = 0, p = 0, t = 0, c = 0; i < count; i++)
        {
            vertices[i] = new VertexPositionColorTexture()
            {
                Position = new Vector3(
                    positions[p++],
                    positions[p++],
                    positions[p++]),
                UV = new Vector2(
                    textureCoords[t++],
                    textureCoords[t++]),
                Color = new Vector4(
                    colors[c++],
                    colors[c++],
                    colors[c++],
                    colors[c++]),
            };
        }
        return vertices;
    }

    public static ParserSectorWall? LoadFrontSidedWall(
        WADLineDef frontLineDef,
        WADSector sector,
        List<WADSideDef> allSideDefs,
        List<WADVertex> allLevelVertices,
        List<ParserImage> allTextures)
    {
        ParserGeometry geometry = new();
        List<float> positions = new();
        List<float> colors = new();
        List<float> textureCoords = new();
        List<uint> indices = [0, 1, 2, 2, 3, 0];

        // Find required data. Vertices that make a line.
        WADVertex start = allLevelVertices[frontLineDef.StartVertexId];
        WADVertex end = allLevelVertices[frontLineDef.EndVertexId];

        // Side definition that makes the front side of the line.
        WADSideDef frontSideDef = allSideDefs[frontLineDef.FrontSideDef];

        // Texture that makes the front side of the line.
        ParserImage? middleTexture = allTextures.FirstOrDefault(x => x.Name == frontSideDef.MiddleTextureName);
        if (middleTexture is null)
        {
            Console.WriteLine($"Missing texture {frontSideDef.MiddleTextureName}");
            return null;
        }

        // Find information about the side def.
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float lineLength = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        float height = Math.Abs(sector.CeilingHeight - sector.FloorHeight);

        float l = sector.LightLevel / 255.0f;
        Vector4 color = new(l, l, l, 1);

        // Find the texture coordinates.
        float u0 = (float)frontSideDef.XOffset / middleTexture.Width;
        float u1 = u0 + lineLength / middleTexture.Width;
        float v0 = (float)frontSideDef.YOffset / middleTexture.Height;
        if (frontLineDef.HasFlag(WADLineDefFlag.DontPegBottom))
        {
            v0 -= height / middleTexture.Height;
        }

        float v1 = v0 + height / middleTexture.Height;

        // Quad data
        AddVertex(positions, colors, textureCoords,
            new(end.X, sector.FloorHeight, -end.Y),
            new(u1, v1),
            color); // Bottom right
        AddVertex(positions, colors, textureCoords,
            new(end.X, sector.CeilingHeight, -end.Y),
            new(u1, v0),
            color); // Top right
        AddVertex(positions, colors, textureCoords,
            new(start.X, sector.CeilingHeight, -start.Y),
            new(u0, v0),
            color); // Top left
        AddVertex(positions, colors, textureCoords,
            new(start.X, sector.FloorHeight, -start.Y),
            new(u0, v1),
            color); // Bottom left

        int count = positions.Count / 3;
        VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[count];
        for (int i = 0, p = 0, t = 0, c = 0; i < count; i++)
        {
            vertices[i] = new VertexPositionColorTexture()
            {
                Position = new Vector3(
                    positions[p++],
                    positions[p++],
                    positions[p++]),
                UV = new Vector2(
                    textureCoords[t++],
                    textureCoords[t++]),
                Color = new Vector4(
                    colors[c++],
                    colors[c++],
                    colors[c++],
                    colors[c++]),
            };
        }
        geometry.Vertices = CreateVertices(positions, textureCoords, colors);
        geometry.Indices = indices.ToArray();

        return new()
        {
            Geometry = geometry,
            Texture = middleTexture.Name,
        };
    }

    /// <summary>
    /// Loads the front sided bottom wall.
    /// </summary>
    /// <param name="frontLineDef">The front line definition.</param>
    /// <param name="backLineDef">The back line definition.</param>
    /// <param name="sectors">All the game sectors.</param>
    /// <param name="allSideDefs">All the game side definitions.</param>
    /// <param name="allLevelVertices">All the level vertices.</param>
    /// <param name="allTextures">All textures of the game.</param>
    /// <returns></returns>
    public static ParserSectorWall? LoadFrontSidedBottomWall(
        WADLineDef frontLineDef,
        WADLineDef backLineDef,
        List<WADSector> sectors,
        List<WADSideDef> allSideDefs,
        List<WADVertex> allLevelVertices,
        List<ParserImage> allTextures)
    {
        ParserGeometry geometry = new();
        List<float> positions = new();
        List<float> colors = new();
        List<float> textureCoords = new();
        List<uint> indices = [0, 1, 2, 2, 3, 0];

        // Find required data. Vertices that make a line.
        WADVertex start = allLevelVertices[frontLineDef.StartVertexId];
        WADVertex end = allLevelVertices[frontLineDef.EndVertexId];

        // Side definition that makes the front side of the line.
        WADSideDef frontSideDef = allSideDefs[frontLineDef.FrontSideDef];
        WADSideDef backSideDef = allSideDefs[backLineDef.BackSideDef];

        // Find the sector that makes the back side of the line.
        WADSector frontSideSector = sectors[frontSideDef.SectorId];
        WADSector backSideSector = sectors[backSideDef.SectorId];

        // Texture that makes the front side of the line.
        ParserImage? lowerTexture = allTextures.FirstOrDefault(x => x.Name == frontSideDef.LowerTextureName);
        if (lowerTexture is null)
        {
            Console.WriteLine($"Missing texture {frontSideDef.LowerTextureName}");
            return null;
        }

        // Find information about the side def.
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float lineLength = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        float height = Math.Abs(backSideSector.FloorHeight - frontSideSector.FloorHeight);

        float l = frontSideSector.LightLevel / 255.0f;
        Vector4 color = new(l, l, l, 1);

        // Find the texture coordinates.
        // TODO: THIS MIGHT NOT BE OK
        float u0 = (float)frontSideDef.XOffset / lowerTexture.Width;
        float u1 = (u0 + lineLength) / lowerTexture.Width;
        float v0 = (float)frontSideDef.YOffset / lowerTexture.Height;
        float v1 = (v0 + height) / lowerTexture.Height;

        if (frontLineDef.HasFlag(WADLineDefFlag.DontPegBottom))
        {
            // If the "lower unpegged" flag is set on the linedef, the lower texture will be drawn as if it began at the higher ceiling and continued downwards.
            // So if the higher ceiling is at 96 and the higher floor is at 64, a lower unpegged texture will start 32 pixels from
            // its top edge and draw downwards from the higher floor.
            float higherCeiling = Math.Max(backSideSector.CeilingHeight, frontSideSector.CeilingHeight);
            float higherFloor = Math.Max(backSideSector.FloorHeight, frontSideSector.FloorHeight);

            height = frontSideSector.CeilingHeight - frontSideSector.FloorHeight;

            v0 = (higherCeiling - higherFloor + frontSideDef.YOffset) / lowerTexture.Height;
            v1 = (v0 + height) / lowerTexture.Height;
        }

        // Quad data
        AddVertex(positions, colors, textureCoords,
            new(end.X, frontSideSector.FloorHeight, -end.Y),
            new(u1, v1),
            color); // Bottom right

        AddVertex(positions, colors, textureCoords,
            new(end.X, backSideSector.FloorHeight, -end.Y),
            new(u1, v0),
            color); // Top right

        AddVertex(positions, colors, textureCoords,
            new(start.X, backSideSector.FloorHeight, -start.Y),
            new(u0, v0),
            color); // Top left

        AddVertex(positions, colors, textureCoords,
            new(start.X, frontSideSector.FloorHeight, -start.Y),
            new(u0, v1),
            color); // Bottom left

        geometry.Vertices = CreateVertices(positions, textureCoords, colors);
        geometry.Indices = indices.ToArray();


        return new()
        {
            Geometry = geometry,
            Texture = lowerTexture.Name,
        };
    }

    /// <summary>
    /// Loads the front sided top wall.
    /// </summary>
    /// <param name="frontLineDef">The front line definition.</param>
    /// <param name="backLineDef">The back line definition.</param>
    /// <param name="sectors">All the game sectors.</param>
    /// <param name="allSideDefs">All the game side definitions.</param>
    /// <param name="allLevelVertices">All the level vertices.</param>
    /// <param name="allTextures">All textures of the game.</param>
    /// <returns></returns>
    public static ParserSectorWall? LoadFrontSidedTopWall(
        WADLineDef frontLineDef,
        WADLineDef backLineDef,
        List<WADSector> sectors,
        List<WADSideDef> allSideDefs,
        List<WADVertex> allLevelVertices,
        List<ParserImage> allTextures)
    {
        ParserGeometry geometry = new();
        List<float> positions = new();
        List<float> colors = new();
        List<float> textureCoords = new();
        List<uint> indices = [0, 1, 2, 2, 3, 0];

        // Find required data. Vertices that make a line.
        WADVertex start = allLevelVertices[frontLineDef.StartVertexId];
        WADVertex end = allLevelVertices[frontLineDef.EndVertexId];

        // Side definition that makes the front side of the line.
        WADSideDef frontSideDef = allSideDefs[frontLineDef.FrontSideDef];
        WADSideDef backSideDef = allSideDefs[backLineDef.BackSideDef];

        // Find sector that makes the back side of the line.
        WADSector frontSideSector = sectors[frontSideDef.SectorId];
        WADSector backSideSector = sectors[backSideDef.SectorId];

        ParserImage? upperTexture = allTextures.FirstOrDefault(x => x.Name == frontSideDef.UpperTextureName);
        if (upperTexture is null)
        {
            Console.WriteLine($"Missing texture {frontSideDef.UpperTextureName}");
            return null;
        }

        // Find information about the side def.
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float width = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        float height = Math.Abs(backSideSector.CeilingHeight - frontSideSector.CeilingHeight);

        float l = frontSideSector.LightLevel / 255.0f;
        Vector4 color = new(l, l, l, 1);

        // Find the texture coordinates.
        float u0 = (float)frontSideDef.XOffset / upperTexture.Width;
        float u1 = u0 + width / upperTexture.Width;
        float v0 = (float)frontSideDef.YOffset / upperTexture.Height;
        if (frontLineDef.HasFlag(WADLineDefFlag.DontPegTop))
        {
            v0 -= height / upperTexture.Height;
        }

        float v1 = v0 - height / upperTexture.Height;

        // Quad data
        AddVertex(positions, colors, textureCoords,
            new(start.X, backSideSector.CeilingHeight, -start.Y),
            new(u0, v0),
            color); // Top left
        AddVertex(positions, colors, textureCoords,
            new(end.X, backSideSector.CeilingHeight, -end.Y),
            new(u1, v0),
            color); // Top right
        AddVertex(positions, colors, textureCoords,
            new(end.X, frontSideSector.CeilingHeight, -end.Y),
            new(u1, v1),
            color); // Bottom right
        AddVertex(positions, colors, textureCoords,
            new(start.X, frontSideSector.CeilingHeight, -start.Y),
            new(u0, v1),
            color); // Bottom left

        geometry.Vertices = CreateVertices(positions, textureCoords, colors);
        geometry.Indices = indices.ToArray();

        return new()
        {
            Geometry = geometry,
            Texture = upperTexture.Name,
        };
    }


    public static ParserSectorWall? LoadBackSidedWall(
        WADLineDef rightLineDef,
        WADSector sector,
        List<WADSideDef> allSideDefs,
        List<WADVertex> allLevelVertices,
        List<ParserImage> allTextures)
    {
        ParserGeometry geometry = new();
        List<float> positions = new();
        List<float> colors = new();
        List<float> textureCoords = new();
        List<uint> indices = [0, 1, 2, 2, 3, 0];

        // Find required data. Vertices that make a line.
        WADVertex start = allLevelVertices[rightLineDef.StartVertexId];
        WADVertex end = allLevelVertices[rightLineDef.EndVertexId];

        // Side definition that makes the front side of the line.
        WADSideDef backSideDef = allSideDefs[rightLineDef.BackSideDef];

        // Texture that makes the front side of the line.
        ParserImage? middleTexture = allTextures.FirstOrDefault(x => x.Name == backSideDef.MiddleTextureName);
        if (middleTexture is null)
        {
            Console.WriteLine($"Missing texture {backSideDef.MiddleTextureName}");
            return null;
        }

        // Find information about the side def.
        float dx = end.X - start.X;
        float dy = end.Y - start.Y;
        float lineLength = (float)Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));
        float height = Math.Abs(sector.CeilingHeight - sector.FloorHeight);

        float l = sector.LightLevel / 255.0f;
        Vector4 color = new(l, l, l, 1);

        // Find the texture coordinates.
        float u0 = (float)backSideDef.XOffset / middleTexture.Width;
        float u1 = u0 + lineLength / middleTexture.Width;
        float v0 = (float)backSideDef.YOffset / middleTexture.Height;
        if ((WADLineDefFlag.DontPegBottom & (WADLineDefFlag)rightLineDef.Flags) == WADLineDefFlag.DontPegBottom)
        {
            v0 -= height / middleTexture.Height;
        }

        float v1 = v0 + height / middleTexture.Height;


        // Quad data
        AddVertex(positions, colors, textureCoords,
            new(start.X, sector.CeilingHeight, -start.Y),
            new(u0, v0),
            color); // Top left
        AddVertex(positions, colors, textureCoords,
            new(end.X, sector.CeilingHeight, -end.Y),
            new(u1, v0),
            color); // Top right
        AddVertex(positions, colors, textureCoords,
            new(end.X, sector.FloorHeight, -end.Y),
            new(u1, v1),
            color); // Bottom right
        AddVertex(positions, colors, textureCoords,
            new(start.X, sector.FloorHeight, -start.Y),
            new(u0, v1),
            color); // Bottom left

        geometry.Vertices = CreateVertices(positions, textureCoords, colors);
        geometry.Indices = indices.ToArray();

        return new()
        {
            Geometry = geometry,
            Texture = middleTexture.Name,
        };
    }

    /// <summary>
    /// Add vertices to the appropriate lists.
    /// </summary>
    /// <param name="positions">The positions list.</param>
    /// <param name="colors">The colors list.</param>
    /// <param name="textureCoords">The texture coords list.</param>
    /// <param name="vertex">The vertex.</param>
    /// <param name="texCoords">The texture coords.</param>
    /// <param name="color">The color to add.</param>
    private static void AddVertex(
        List<float> positions, List<float> colors, List<float> textureCoords,
        Vector3 vertex, Vector2 texCoords, Vector4 color)
    {
        // Pos
        positions.Add(vertex.X); // X coordinate.
        positions.Add(vertex.Y); // Height.
        positions.Add(vertex.Z); // Z coordinate.

        // Color
        colors.Add(color.X);
        colors.Add(color.Y);
        colors.Add(color.Z);
        colors.Add(color.W);

        // Tex coords
        textureCoords.Add(texCoords.X);
        textureCoords.Add(texCoords.Y);
    }
}
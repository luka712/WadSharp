using System.Numerics;
using WadSharp.Parts;

namespace WadSharp.Parsing;

/// <summary>
/// The sector information for a WAD file.
/// </summary>
public class ParserSector
{
    /// <summary>
    /// The ID of the sector.
    /// </summary>
    public int Id { get; set; } = -1;

    /// <summary>
    /// The geometry of the floor.
    /// </summary>
    public ParserGeometry? FloorGeometry { get; set; }

    /// <summary>
    /// The floor image.
    /// </summary>
    public string? FloorImage { get; set; }

    /// <summary>
    /// The geometry of the ceiling.
    /// </summary>
    public ParserGeometry? CeilingGeometry { get; set; }

    /// <summary>
    /// The ceiling image.
    /// </summary>
    public string? CeilingImage { get; set; }

    /// <summary>
    /// The tag number of the sector.
    /// </summary>
    public uint Tag { get; set; } = 0;

    /// <summary>
    /// The bounding box minimum value.
    /// </summary>
    public Vector3 BoundingBoxMin { get; set; }

    /// <summary>
    /// The bounding box maximum value.
    /// </summary>
    public Vector3 BoundingBoxMax { get; set; }

    /// <summary>
    /// The walls of a sector.
    /// </summary>
    public List<ParserSectorWall> Walls { get; set; } = new();

    /// <summary>
    /// Find the sector for the given segment.
    /// </summary>
    /// <param name="sectors">The list of all sectors.</param>
    /// <param name="sideDefs">The list of all side definitions.</param>
    /// <param name="lineDefs">The list of all line definitions.</param>
    /// <param name="glSegment">The segment to check.</param>
    /// <param name="sectorIdx">
    /// The sector ID. If line def is two-sided line def and gl segment is back segment, this will be the back side sector ID.
    /// Otherwise, this will be the front side sector ID.
    /// </param>
    /// <returns>
    /// The sector that segment belongs to.
    /// If the segment is a back side segment, this will be the back side sector.
    /// If the segment is a front side segment, this will be the front side sector.
    /// </returns>
    private static WADSector? FindSector(
        List<WADSector> sectors,
        List<WADSideDef> sideDefs,
        List<WADLineDef> lineDefs,
        WADGLSeg glSegment,
        out int sectorIdx)
    {
        sectorIdx = -1;

        // Mini segs are never drawn.
        if (glSegment.IsMiniSeg)
        {
            return null;
        }

        // Segments contains the line definition ID.
        // In turn line def contains the sidedef ID.
        // The sidedef ID contains the sector ID.
        WADLineDef lineDef = lineDefs[glSegment.LineDefId];

        // If the segment is a back side, we need to check if it is two-sided.
        // If it is, we need to get the back side sector.
        if (lineDef.HasFlag(WADLineDefFlag.TwoSided) && !glSegment.IsFrontSide)
        {
            sectorIdx = sideDefs[lineDef.BackSideDef].SectorId;
        }
        else
        {
            // If it is not two-sided, we need to get the front side sector.
            sectorIdx = sideDefs[lineDef.FrontSideDef].SectorId;
        }

        return sectors[sectorIdx];
    }

    /// <summary>
    /// Parse the WAD file and return a list of sectors.
    /// </summary>
    /// <param name="allGameSectors">The all the game sectors.</param>
    /// <param name="allGameSubSectors">The all the game subsectors.</param>
    /// <param name="allSideDefs">The all the game side defs.</param>
    /// <param name="allLineDefs">The all the game line defs.</param>
    /// <param name="allGLSegments">The all the game gl segments.</param>
    /// <param name="allGLVertices">The all the game gl vertices.</param>
    /// <param name="allVertices">The all the game vertices.</param>
    /// <param name="allImages">The game images or textures.</param>
    /// <returns>List of parsed sectors.</returns>
    public static List<ParserSector> Parse(
        List<WADSector> allGameSectors,
        List<WADGLSubSector> allGameSubSectors,
        List<WADSideDef> allSideDefs,
        List<WADLineDef> allLineDefs,
        List<WADGLSeg> allGLSegments,
        List<WADGLVertex> allGLVertices,
        List<WADVertex> allVertices,
        List<ParserImage> allImages
    )
    {
        List<ParserSector> sectors = new();

        // We need to parse information bottom up, so we need to go through all the subsectors first.

        // A subsector is a convex polygon defined by the sequence of segments listed in GL_SEGS,
        // starting at first_seg and continuing for num_segs entries.
        for (int i = 0; i < allGameSubSectors.Count; i++)
        {
            // Definition can have two sectors, front, back or only front or back.
            WADSector? currentSector = null;
            WADGLSubSector subSector = allGameSubSectors[i];

            Vector3 boundingBoxMin = new(float.MaxValue, float.MaxValue, float.MaxValue);
            Vector3 boundingBoxMax = new(float.MinValue, float.MinValue, float.MinValue);

            // Cannot be triangulated.
            if (subSector.SegCount < 3)
            {
                Console.WriteLine("Subsector cannot be triangulated.");
                continue;
            }

            // Go through all segments in the subsector.
            Vector3[]? sectorVertices = new Vector3[subSector.SegCount];
            int sectorId = -1;
            for (int j = 0; j < subSector.SegCount; j++)
            {
                // GL segment is a segment that is part of the subsector. It refers to the GL vertex.
                WADGLSeg segment = allGLSegments[(int)subSector.FirstSegIdx + j];
                if (currentSector is null)
                {
                    currentSector = FindSector(
                        allGameSectors, allSideDefs, allLineDefs, segment,
                        out sectorId);

                    if (currentSector is null)
                    {
                        continue;
                    }
                }

                Vector3 vector;
                if (segment.IsStartVertexIdGLVertex)
                {
                    WADGLVertex vertex = allGLVertices[segment.GLStartVertexId];
                    vector = new Vector3(vertex.X, 0, -vertex.Y);
                }
                else
                {
                    WADVertex vertex = allVertices[segment.StartVertexId];
                    vector = new Vector3(vertex.X, 0, -vertex.Y);
                }

                // Find bounding box values.
                boundingBoxMin = new Vector3(
                    Math.Min(boundingBoxMin.X, vector.X),
                    Math.Min(boundingBoxMin.Y, currentSector.FloorHeight),
                    Math.Min(boundingBoxMin.Z, vector.Z)
                );

                boundingBoxMax = new Vector3(
                    Math.Max(boundingBoxMax.X, vector.X),
                    Math.Max(boundingBoxMax.Y, currentSector.CeilingHeight),
                    Math.Max(boundingBoxMax.Z, vector.Z)
                );


                sectorVertices[j] = vector;
            }

            List<(WADLineDef, WADLineDef?)> lineDefs =
                FindSectorLineDefs(sectorId, allLineDefs, allSideDefs);

            float lightLevel = currentSector!.LightLevel / 255.0f;
            Vector4 color = new(lightLevel, lightLevel, lightLevel, 1);

            // Create the mesh.
            ParserSector sectorObj = new()
            {
                FloorGeometry = CreateFloorCeilingGeometry(sectorVertices, currentSector!.FloorHeight, color, true),
                FloorImage = currentSector.FloorTexture
            };
            if (currentSector.CeilingTexture?.Contains("SKY") != true)
            {
                sectorObj.CeilingGeometry =
                    CreateFloorCeilingGeometry(sectorVertices, currentSector.CeilingHeight, color, false);
                sectorObj.CeilingImage = currentSector.CeilingTexture;
            }

            sectorObj.Tag = (uint)currentSector.Tag;
            sectorObj.BoundingBoxMin = boundingBoxMin;
            sectorObj.BoundingBoxMax = boundingBoxMax;
            sectorObj.Id = sectorId;


            // Create the walls.
            foreach ((WADLineDef, WADLineDef?) lineDef in lineDefs)
            {
                if (SkipSectorWall(lineDef.Item1, lineDef.Item2, allGameSectors, allSideDefs))
                {
                    continue;
                }

                ParserSectorWall? wall = ParserSectorWall.LoadFrontSidedWall(
                    lineDef.Item1,
                    currentSector,
                    allSideDefs,
                    allVertices,
                    allImages
                );
                if (wall != null)
                {
                    sectorObj.Walls.Add(wall);
                }

                if (lineDef.Item2 != null)
                {
                    wall = ParserSectorWall.LoadBackSidedWall(
                        lineDef.Item2,
                        currentSector,
                        allSideDefs,
                        allVertices,
                        allImages
                    );
                    if (wall != null)
                    {
                        sectorObj.Walls.Add(wall);
                    }

                    wall = ParserSectorWall.LoadFrontSidedBottomWall(
                        lineDef.Item1,
                        lineDef.Item2,
                        allGameSectors,
                        allSideDefs,
                        allVertices,
                        allImages
                    );
                    if (wall != null)
                    {
                        sectorObj.Walls.Add(wall);
                    }

                    wall = ParserSectorWall.LoadFrontSidedTopWall(
                        lineDef.Item1,
                        lineDef.Item2,
                        allGameSectors,
                        allSideDefs,
                        allVertices,
                        allImages
                    );
                    if (wall != null)
                    {
                        sectorObj.Walls.Add(wall);
                    }
                }
            }

            sectors.Add(sectorObj);
        }

        return MergeSectors(sectors);
    }

    /// <summary>
    /// Should we skip the sector?
    /// </summary>
    /// <param name="frontLineDef">The front line definition.</param>
    /// <param name="backLineDef">The back line definition.</param>
    /// <param name="sectors">All the game sectors</param>
    /// <param name="sideDefs">All the game side definitions.</param>
    /// <returns>True if sector wall should be skipped.</returns>
    /// <remarks>
    /// It should be skipped if:
    /// <list>
    /// <item>1. It is a two-sided line definition.</item>
    /// <item>2. The back line definition is not null.</item>
    /// <item>3. The front and back sector share the same ceiling texture.</item>
    /// </list>
    /// </remarks>
    private static bool SkipSectorWall(WADLineDef frontLineDef, WADLineDef? backLineDef, List<WADSector> sectors, List<WADSideDef> sideDefs)
    {
        // It is one-sided, nothing to skip.
        if (backLineDef is null)
        {
            return false;
        }

        ushort frontSectorIdx = sideDefs[frontLineDef.FrontSideDef].SectorId;
        ushort backSectorIdx = sideDefs[backLineDef.BackSideDef].SectorId;

        // If both sectors share same ceiling texture and ceiling texture is sky, skip it.
        WADSector front = sectors[frontSectorIdx];
        WADSector back = sectors[backSectorIdx];

        // We skip if 
        // 1. If both sectors share same ceiling texture
        // 2. If the ceiling texture is sky
        // 3. If the floor height of two sectors is same.
        if (front.CeilingTexture == back.CeilingTexture
            && front.CeilingTexture?.Contains("SKY") == true
            && front.FloorHeight == back.FloorHeight)
        {
            return true;
        }

        return false;
    }

    private static List<ParserSector> MergeSectors(List<ParserSector> sectors)
    {
        Dictionary<int, List<ParserSector>> mergedSectors = new();
        for (int i = 0; i < sectors.Count; i++)
        {
            ParserSector sector = sectors[i];
            if (!mergedSectors.ContainsKey(sector.Id))
            {
                mergedSectors[sector.Id] = new List<ParserSector>();
            }

            mergedSectors[sector.Id].Add(sector);
        }

        List<ParserSector> merged = new();
        foreach (KeyValuePair<int, List<ParserSector>> kvp in mergedSectors)
        {
            ParserSector mergedSector = kvp.Value[0];
            for (int i = 1; i < kvp.Value.Count; i++)
            {
                mergedSector = MergeSectors(mergedSector, kvp.Value[i]);
            }

            merged.Add(mergedSector);
        }

        return merged;
    }

    private static ParserSector MergeSectors(ParserSector a, ParserSector b)
    {
        // Merge the two sectors.
        if (a.Id != b.Id)
        {
            throw new InvalidCastException("Cannot merge sectors with different IDs.");
        }

        ParserSector merged = new();
        merged.Id = a.Id;
        merged.FloorGeometry = ParserGeometry.MergeGeometry(a.FloorGeometry!, b.FloorGeometry!);
        merged.FloorImage = a.FloorImage;
        merged.CeilingGeometry = ParserGeometry.MergeGeometry(a.CeilingGeometry!, b.CeilingGeometry!);
        merged.CeilingImage = a.CeilingImage;
        merged.Tag = a.Tag;
        merged.BoundingBoxMin = Vector3.Min(a.BoundingBoxMin, b.BoundingBoxMin);
        merged.BoundingBoxMax = Vector3.Max(a.BoundingBoxMax, b.BoundingBoxMax);
        merged.Walls = a.Walls.Concat(b.Walls).ToList();

        return merged;
    }

    /// <summary>
    /// Create geometry for ceiling or floor.
    /// </summary>
    /// <param name="vertices">The vertices of geometry.</param>
    /// <param name="height">The height of floor or ceiling geometry.</param>
    /// <param name="color">Color of the geometry.</param>
    /// <param name="isFloor">Set <c>true</c> if it is floor geometry, <c>false</c> if it is ceiling geometry.</param>
    /// <returns>The sector geometry.</returns>
    private static ParserGeometry CreateFloorCeilingGeometry(Vector3[] vertices, float height, Vector4 color, bool isFloor)
    {
        uint[] indices = new uint[3 * (vertices.Length - 2)];

        for (int i = 0, j = 1; i < indices.Length; i += 3, j++)
        {
            if (isFloor)
            {
                indices[i] = (uint)(j + 1);
                indices[i + 1] = (uint)j;
                indices[i + 2] = 0;
            }
            else
            {
                indices[i] = 0;
                indices[i + 1] = (uint)j;
                indices[i + 2] = (uint)(j + 1);
            }
        }

        // Create the mesh.
        ParserGeometry geometry = new();
        geometry.Positions = new float[vertices.Length * 3];
        geometry.TextureCoordinates = new float[vertices.Length * 2];
        geometry.Colors = new float[vertices.Length * 4];
        geometry.Indices = indices;

        for (int i = 0, p = 0, c = 0, t = 0; i < vertices.Length; i++)
        {
            Vector3 vertex = vertices[i];

            geometry.Positions[p++] = vertex.X;
            geometry.Positions[p++] = height;
            geometry.Positions[p++] = vertex.Z;

            geometry.Colors[c++] = color.X;
            geometry.Colors[c++] = color.Y;
            geometry.Colors[c++] = color.Z;
            geometry.Colors[c++] = color.W;

            geometry.TextureCoordinates[t++] = vertex.X / 64f;
            geometry.TextureCoordinates[t++] = vertex.Z / 64f;
        }

        return geometry;
    }

    /// <summary>
    /// Find all line definitions that make a sector.
    /// </summary>
    /// <param name="sectorId">The <see cref="WADSector"/> id.</param>
    /// <param name="allLineDefs">All line definitions.</param>
    /// <param name="allSideDefs">All side definitions.</param>
    /// <returns>
    /// The tuple of line definitions that belong to a sector.
    /// First item is the front side definition.
    /// Second, if present, is the back side definition. If not present, it is <c>null</c>.
    /// </returns>
    private static List<(WADLineDef, WADLineDef?)> FindSectorLineDefs(
        int sectorId,
        List<WADLineDef> allLineDefs,
        List<WADSideDef> allSideDefs)
    {
        List<(WADLineDef, WADLineDef?)> lineDefs = new();

        foreach (WADLineDef lineDef in allLineDefs)
        {
            WADLineDef front = null!;
            WADLineDef back = null!;

            WADSideDef frontSideDef = allSideDefs[lineDef.FrontSideDef];
            if (frontSideDef.SectorId != sectorId)
            {
                continue;
            }

            front = lineDef;

            if (lineDef.HasFlag(WADLineDefFlag.TwoSided))
            {
                // NOTE: Back side does not have to belong to the same sector. In fact, it will belong to a different sector.
                WADSideDef backSideDef = allSideDefs[lineDef.BackSideDef];
                back = lineDef;
            }

            lineDefs.Add(new(front, back));
        }

        return lineDefs;
    }
}
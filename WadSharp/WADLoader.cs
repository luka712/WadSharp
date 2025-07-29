using Microsoft.Extensions.Logging;
using WadSharp.Parts;

using static WadSharp.Parts.WADGLVersion;

namespace WadSharp;

/// <summary>
/// The Doom WAD loader.
/// </summary>
/// <param name="logger">The logger.</param>
public class WADLoader(ILogger? logger = null)
{
    private const string THINGS = "THINGS";
    private const string VERTEXES = "VERTEXES";
    private const string LINEDEFS = "LINEDEFS";
    private const string SIDEDEFS = "SIDEDEFS";
    private const string SEGS = "SEGS";
    private const string SSECTORS = "SSECTORS";
    private const string NODES = "NODES";
    private const string SECTORS = "SECTORS";

    /// <summary>
    /// The PLAYPAL lump is a palette used in the DOOM engine.
    /// </summary>
    private const string PLAYPAL = "PLAYPAL";

    /// <summary>
    /// The F_START and F_END lumps are used to define the start and end of a flat texture in the DOOM engine.
    /// </summary>
    private const string F_START = "F_START";

    /// <summary>
    /// The F_START and F_END lumps are used to define the start and end of a flat texture in the DOOM engine.
    /// </summary>
    private const string F_END = "F_END";

    /// <summary>
    /// The P_START and P_END lumps are used to define the start and end of a patches (wall textures ) in the DOOM engine.
    /// Patches are stored as a list which is read from PNAMES lump.
    /// </summary>
    private const string P_START = "P_START";

    /// <summary>
    /// The P_START and P_END lumps are used to define the start and end of a patches (wall textures ) in the DOOM engine.
    /// Patches are stored as a list which is read from PNAMES lump.
    /// </summary>
    private const string P_END = "P_END";

    /// <summary>
    /// Patches are used to define the textures used in the DOOM engine.
    /// </summary>
    private const string PNAMES = "PNAMES";

    private const string TEXTURE1 = "TEXTURE1";
    private const string TEXTURE2 = "TEXTURE2";

    private const string GL_VERT = "GL_VERT";
    private const string GL_SEGS = "GL_SEGS";
    private const string GL_SSECT = "GL_SSECT";
    private const string GL_NODES = "GL_NODES";

    private const string GL_LUMP_VERSION_2 = "gNd2";
    private const string GL_LUMP_VERSION_3 = "gNd3";
    private const string GL_LUMP_VERSION_4 = "gNd4";
    private const string GL_LUMP_VERSION_5 = "gNd5";

    /// <summary>
    /// Load the DOOM WAD file.
    /// </summary>
    /// <param name="iwadFilePath">The file path to the .wad file which represents <b>iwad</b>.</param>
    /// <param name="pwadFilePaths">Optional file paths to the .wad files which represent <b>pwad</b>.</param>
    /// <returns>The <see cref="WAD"/>.</returns>
    /// <exception cref="FileNotFoundException">If file is not found.</exception>
    public WAD Load(string iwadFilePath, params string[] pwadFilePaths)
    {
        if (!File.Exists(iwadFilePath))
        {
            string msg = $"The file does not exist: {iwadFilePath}";
            logger?.LogError(msg);
            throw new FileNotFoundException(msg, iwadFilePath);
        }

        byte[] bytes = File.ReadAllBytes(iwadFilePath);
        WAD iwad = ReadWAD(bytes, iwadFilePath);

        foreach(string pwadFilePath in pwadFilePaths)
        {
            if (!File.Exists(pwadFilePath))
            {
                string msg = $"The file does not exist: {pwadFilePath}";
                logger?.LogError(msg);
                throw new FileNotFoundException(msg, pwadFilePath);
            }

            byte[] pwadBytes = File.ReadAllBytes(pwadFilePath);
            WAD pwad = ReadWAD(pwadBytes, pwadFilePath);

            // Merge the PWAD into the IWAD.
            iwad.Directories.AddRange(pwad.Directories);
        }

        return iwad;
    }

    /// <summary>
    /// Load the DOOM WAD file.
    /// </summary>
    /// <param name="iwadFilePath">The file path to the .wad file which represents <b>iwad</b>.</param>
    /// <param name="pwadFilePaths">Optional file paths to the .wad files which represent <b>pwad</b>.</param>
    /// <returns>The <see cref="WAD"/>.</returns>
    /// <exception cref="FileNotFoundException">If file is not found.</exception>
    public async Task<WAD> LoadAsync(string iwadFilePath, params string[] pwadFilePaths)
    {
        if (!File.Exists(iwadFilePath))
        {
            string msg = $"The file does not exist: {iwadFilePath}";
            logger?.LogError(msg);
            throw new FileNotFoundException(msg, iwadFilePath);
        }

        byte[] bytes = await File.ReadAllBytesAsync(iwadFilePath);
        WAD iwad = ReadWAD(bytes, iwadFilePath);

        foreach (string pwadFilePath in pwadFilePaths)
        {
            if (!File.Exists(pwadFilePath))
            {
                string msg = $"The file does not exist: {pwadFilePath}";
                logger?.LogError(msg);
                throw new FileNotFoundException(msg, pwadFilePath);
            }

            byte[] pwadBytes = await File.ReadAllBytesAsync(pwadFilePath);
            WAD pwad = ReadWAD(pwadBytes, pwadFilePath);

            // Merge the PWAD into the IWAD.
            iwad.Directories.AddRange(pwad.Directories);
        }

        return iwad;
    }

    private WAD ReadWAD(byte[] bytes, string filePath)
    {
        if (bytes.Length < 12)
        {
            string msg = $"The file is too small to be a valid WAD file: {filePath}";
            logger?.LogError(msg);
            throw new InvalidDataException(msg);
        }

        WAD wad = new();
        using (BinaryReader reader = new BinaryReader(new MemoryStream(bytes)))
        {
            char[] identification = reader.ReadChars(4);
            string identificationStr = new string(identification);
            if (identificationStr != "IWAD" && identificationStr != "PWAD")
            {
                string msg = $"The file is not a valid WAD file: {filePath}";
                logger?.LogError(msg);
                throw new InvalidDataException(msg);
            }

            wad.Identification = identification;
            wad.NumLumps = reader.ReadUInt32();
            wad.InfoTableOffset = reader.ReadUInt32();

            // Read the directory.
            wad.Directories = ReadDirectories(reader, wad);
        }

        return wad;
    }

    private List<WADLump> ReadDirectories(BinaryReader reader, WAD wad)
    {
        List<WADLump> directories = new();
        WADGLVersion glLumpsVersion = WADGLVersion.gNd1;

        long currentPos = wad.InfoTableOffset;
        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        // Flats are contained between F_START and F_END lumps.
        // There are floor and ceiling flats. We use them to create floor and ceiling textures.
        bool isReadingFlats = false;

        // Patches are contained between P_START and P_END lumps.
        // They are used to create wall textures.
        bool isReadingPatches = false;

        long patchHeaderStart = 0;

        List<string> patchNames = new();

        for (int i = 0; i < wad.NumLumps; i++)
        {
            if (currentPos >= reader.BaseStream.Length)
            {
                break;
            }

            WADLump lump = new();
            lump.FilePos = reader.ReadUInt32();
            lump.Size = reader.ReadUInt32();
            lump.Name = reader.ReadChars(8);
            directories.Add(lump);

            // Replace null characters with empty string. Doom WAD uses null characters to pad the name to 8 characters.
            string nameStr = new string(lump.Name).Replace("\0", "");
            Console.WriteLine(nameStr);

            if (nameStr == THINGS)
            {
                lump.Things = ReadThings(reader, lump);
            }
            else if (nameStr == VERTEXES)
            {
                lump.Vertices = ReadVertexes(reader, lump);
            }
            else if (nameStr == LINEDEFS)
            {
                lump.LineDefs = ReadLineDefs(reader, lump);
            }
            else if (nameStr == SIDEDEFS)
            {
                lump.SideDefs = ReadSideDefs(reader, lump);
            }
            else if (nameStr == SSECTORS)
            {
                lump.SubSectors = ReadSubSectors(reader, lump);
            }
            else if (nameStr == NODES)
            {
                lump.Nodes = ReadNodes(reader, lump);
            }
            else if (nameStr == SEGS)
            {
                lump.Segs = ReadSegs(reader, lump);
            }
            else if (nameStr == SECTORS)
            {
                lump.Sectors = ReadSectors(reader, lump);
            }
            else if (nameStr == PNAMES)
            {
                lump.PNames = LoadPatches(reader, lump, out patchHeaderStart);
                patchNames = lump.PNames.PatchNames;
            }
            else if (nameStr == TEXTURE1)
            {
                lump.Texture1 = ReadTexture(reader, lump);
            }
            else if (nameStr == TEXTURE2)
            {
                lump.Texture2 = ReadTexture(reader, lump);
            }
            else if (nameStr == PLAYPAL)
            {
                lump.PlayPal = ReadPlayPal(reader, lump);
            }
            // Start of FLATS
            else if (nameStr == F_START)
            {
                isReadingFlats = true;
            }
            else if (isReadingFlats)
            {
                lump.Flat = ReadFlat(reader, lump);
            }
            // End of flats.
            else if (nameStr == F_END)
            {
                isReadingFlats = false;
            }
            else if (nameStr == P_START)
            {
                isReadingPatches = true;
            }
            else if (isReadingPatches)
            {
                string patchName = new string(lump.Name).Replace("\0", "");
                if (patchNames.Contains(patchName))
                {
                    lump.PictureFormat = ReadPictureFormat(reader, lump, patchName);
                }
            }
            else if (isReadingFlats)
            {
                isReadingPatches = false;
            }
            else if (nameStr == GL_VERT)
            {
                lump.GLVertices = ReadGLVertices(reader, lump, out WADGLVersion version);
                glLumpsVersion = version;
                lump.GLVerticesVersion = glLumpsVersion;
            }
            else if (nameStr == GL_SEGS)
            {
                lump.GLSegs = ReadGLSegs(reader, lump, glLumpsVersion);
            }
            else if (nameStr == GL_SSECT)
            {
                lump.GLSubSectors = ReadGLSubSectors(reader, lump, glLumpsVersion);
            }
            else if (nameStr == GL_NODES)
            {
                lump.GLNodes = ReadGLNodes(reader, lump);
            }
        }

        return directories;
    }

    private List<WADThing> ReadThings(BinaryReader reader, WADLump lump)
    {
        List<WADThing> things = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 10)
        {
            WADThing thing = new();
            thing.X = reader.ReadInt16();
            thing.Y = reader.ReadInt16();
            thing.Angle = reader.ReadInt16();
            thing.Type = reader.ReadUInt16();
            thing.Flags = reader.ReadUInt16();
            things.Add(thing);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return things;
    }

    private List<WADVertex> ReadVertexes(BinaryReader reader, WADLump lump)
    {
        List<WADVertex> vertices = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 4)
        {
            short x = reader.ReadInt16();
            short y = reader.ReadInt16();
            vertices.Add(new WADVertex(x, y));
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return vertices;
    }

    private List<WADLineDef> ReadLineDefs(BinaryReader reader, WADLump lump)
    {
        List<WADLineDef> lines = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 14)
        {
            WADLineDef lineDef = new();
            lineDef.StartVertexId = reader.ReadUInt16();
            lineDef.EndVertexId = reader.ReadUInt16();
            lineDef.Flags = reader.ReadUInt16();
            lineDef.SpecialType = reader.ReadUInt16();
            lineDef.SectorTag = reader.ReadUInt16();
            lineDef.FrontSideDef = reader.ReadUInt16();
            lineDef.BackSideDef = reader.ReadUInt16();

            lines.Add(lineDef);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return lines;
    }

    private List<WADSideDef> ReadSideDefs(BinaryReader reader, WADLump lump)
    {
        List<WADSideDef> sideDefs = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 30)
        {
            WADSideDef sideDef = new();
            sideDef.XOffset = reader.ReadInt16();
            sideDef.YOffset = reader.ReadInt16();
            sideDef.UpperTextureName = new string(reader.ReadChars(8)).Replace("\0", "");
            sideDef.LowerTextureName = new string(reader.ReadChars(8)).Replace("\0", "");
            sideDef.MiddleTextureName = new string(reader.ReadChars(8)).Replace("\0", "");
            sideDef.SectorId = reader.ReadUInt16();
            sideDefs.Add(sideDef);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return sideDefs;
    }

    private List<WADSeg> ReadSegs(BinaryReader reader, WADLump lump)
    {
        List<WADSeg> segs = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 12)
        {
            WADSeg seg = new();
            seg.StartVertexId = reader.ReadInt16();
            seg.EndVertexId = reader.ReadInt16();
            seg.Angle = reader.ReadInt16();
            seg.LineDefId = reader.ReadInt16();
            seg.Direction = reader.ReadInt16();
            seg.Offset = reader.ReadInt16();
            segs.Add(seg);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return segs;
    }

    private List<WADSubSector> ReadSubSectors(BinaryReader reader, WADLump lump)
    {
        List<WADSubSector> subSectors = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 4)
        {
            WADSubSector subSector = new();
            subSector.SegCount = reader.ReadInt16();
            subSector.FirstSegId = reader.ReadInt16();
            subSectors.Add(subSector);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return subSectors;
    }

    private List<WADNode> ReadNodes(BinaryReader reader, WADLump lump)
    {
        List<WADNode> nodes = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 28)
        {
            WADNode node = new();
            node.XPartition = reader.ReadInt16();
            node.YPartition = reader.ReadInt16();
            node.DxPartition = reader.ReadInt16();
            node.DyPartition = reader.ReadInt16();
            node.RightBoundingBox = new WADBoundingBox()
            {
                Top = reader.ReadInt16(),
                Bottom = reader.ReadInt16(),
                Left = reader.ReadInt16(),
                Right = reader.ReadInt16()
            };
            node.LeftBoundingBox = new WADBoundingBox()
            {
                Top = reader.ReadInt16(),
                Bottom = reader.ReadInt16(),
                Left = reader.ReadInt16(),
                Right = reader.ReadInt16()
            };
            node.RightChildId = reader.ReadUInt16();
            node.LeftChildId = reader.ReadUInt16();

            nodes.Add(node);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return nodes;
    }

    private List<WADSector> ReadSectors(BinaryReader reader, WADLump lump)
    {
        List<WADSector> sectors = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 26)
        {
            WADSector sector = new();
            sector.FloorHeight = reader.ReadInt16();
            sector.CeilingHeight = reader.ReadInt16();
            sector.FloorTexture = new string(reader.ReadChars(8)).Replace("\0", "");
            sector.CeilingTexture = new string(reader.ReadChars(8)).Replace("\0", "");
            sector.LightLevel = reader.ReadInt16();
            sector.SpecialType = reader.ReadInt16();
            sector.Tag = reader.ReadInt16();
            sectors.Add(sector);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return sectors;
    }

    private List<WADGLVertex> ReadGLVertices(BinaryReader reader, WADLump lump, out WADGLVersion version)
    {
        List<WADGLVertex> vertices = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);

        string versionStr = new string(reader.ReadChars(4));

        // If there is no magic str for the version, it is gNd1.
        // Else it is gNd2, gNd3, gNd4 or gNd5.
        version = Enum.TryParse(versionStr, false, out WADGLVersion parsedVersion) ? parsedVersion : gNd1;

        for (uint i = 4; i < lump.Size; i += 8)
        {
            float x = (float)reader.ReadInt32() / (1 << 16);
            float y = (float)reader.ReadInt32() / (1 << 16);
            vertices.Add(new WADGLVertex(x, y));
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return vertices;
    }

    private List<WADGLSeg> ReadGLSegs(BinaryReader reader, WADLump lump, WADGLVersion version)
    {
        List<WADGLSeg> glSegs = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        if (version == gNd1 || version == gNd2)
        {
            for (uint i = 0; i < lump.Size; i += 10)
            {
                WADGLSeg glSeg = new();
                glSeg.Version = version;
                glSeg.StartVertexId = reader.ReadUInt16();
                glSeg.EndVertexId = reader.ReadUInt16();
                glSeg.LineDefId = reader.ReadUInt16();
                glSeg.Side = reader.ReadUInt16();
                glSeg.PartnerSegId = reader.ReadUInt16();
                glSegs.Add(glSeg);
            }
        }
        else
        {
            if (version == gNd3 || version == gNd4)
            {
                // Version 3 and 4 have a 4 byte version magic string. Does not exist in version 5.
                string versionStr = new string(reader.ReadChars(4));
            }

            for (uint i = 0; i < lump.Size; i += 16)
            {
                WADGLSeg glSeg = new();
                glSeg.Version = version;
                glSeg.StartVertexId = reader.ReadInt32();
                glSeg.EndVertexId = reader.ReadInt32();
                glSeg.LineDefId = reader.ReadUInt16();
                glSeg.Side = reader.ReadUInt16();
                glSeg.PartnerSegId = reader.ReadUInt32();
                glSegs.Add(glSeg);
            }
        }


        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return glSegs;
    }

    private List<WADGLSubSector> ReadGLSubSectors(BinaryReader reader, WADLump lump, WADGLVersion version)
    {
        List<WADGLSubSector> subSectors = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);

        if (version == gNd1 || version == gNd2)
        {
            for (uint i = 0; i < lump.Size; i += 4)
            {
                WADGLSubSector subSector = new();
                subSector.SegCount = (uint)reader.ReadInt16();
                subSector.FirstSegIdx = reader.ReadUInt16();
                subSectors.Add(subSector);
            }
        }
        else
        {
            if (version == gNd3 || version == gNd4)
            {
                // Version 3 and 4 have a 4 byte version magic string. Does not exist in version 5.
                string versionStr = new string(reader.ReadChars(4));
            }

            for (uint i = 0; i < lump.Size; i += 8)
            {
                WADGLSubSector subSector = new();
                subSector.SegCount = reader.ReadUInt32();
                subSector.FirstSegIdx = reader.ReadUInt32();
                subSectors.Add(subSector);
            }
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return subSectors;
    }

    private List<WADGLNode> ReadGLNodes(BinaryReader reader, WADLump lump)
    {
        List<WADGLNode> nodes = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        for (uint i = 0; i < lump.Size; i += 28)
        {
            WADGLNode node = new();
            node.XPartition = reader.ReadInt16();
            node.YPartition = reader.ReadInt16();
            node.DxPartition = reader.ReadInt16();
            node.DyPartition = reader.ReadInt16();
            node.RightBoundingBox = new WADBoundingBox()
            {
                Top = reader.ReadInt16(),
                Bottom = reader.ReadInt16(),
                Left = reader.ReadInt16(),
                Right = reader.ReadInt16()
            };
            node.LeftBoundingBox = new WADBoundingBox()
            {
                Top = reader.ReadInt16(),
                Bottom = reader.ReadInt16(),
                Left = reader.ReadInt16(),
                Right = reader.ReadInt16()
            };
            node.RightChildId = reader.ReadUInt16();
            node.LeftChildId = reader.ReadUInt16();

            nodes.Add(node);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return nodes;
    }

    private WADPlayPal ReadPlayPal(BinaryReader reader, WADLump lump)
    {
        WADPlayPal playpal = new();

        // Size of lump must be multiple of 768 ( 256 colors * 3 bytes per color ).
        if (lump.Size % 768 != 0)
        {
            string msg = $"The PLAYPAL lump size is not a multiple of 768: {lump.Size}";
            logger?.LogError(msg);
            throw new InvalidDataException(msg);
        }

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);

        // Go through each palette. 
        for (uint i = 0; i < lump.Size; i += 3)
        {
            byte r = reader.ReadByte();
            byte g = reader.ReadByte();
            byte b = reader.ReadByte();
            // Pallette colors are not transparent by default, therefore add 255.
            playpal.PalettesColors.Add(new WADColor(r, g, b, 255));
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return playpal;
    }

    /// <summary>
    /// Flat data is usually 64x64 pixels, but can be larger.
    /// </summary>
    /// <param name="reader">The <see cref="BinaryReader"/>.</param>
    /// <param name="lump">The <see cref="WADLump"/>.</param>
    /// <returns>The <see cref="WadFlat"/>.</returns>
    private WADFlat ReadFlat(BinaryReader reader, WADLump lump)
    {
        WADFlat flat = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        flat.Name = new string(lump.Name).Replace("\0", "");

        // TODO: see if there are different flats
        flat.Width = 64;
        flat.Height = 64;

        // For now, we only care about the 64x64 flats. If not that size, we will skip it.
        for (int i = 0; i < 64 * 64; i++)
        {
            // Read 4 bytes for each pixel.
            // Each byte is an index into the palette colors.
            byte index = reader.ReadByte();
            flat.IndicesIntoPaletteColors.Add(index);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return flat;
    }

    /// <summary>
    /// Loads the patches names.
    /// </summary>
    /// <param name="reader">The <see cref="BinaryReader"/>.</param>
    /// <param name="lump">The <see cref="WADLump"/>.</param>
    /// <returns>The <see cref="WadFlat"/>.</returns>
    private WADPNames LoadPatches(BinaryReader reader, WADLump lump, out long patchHeaderStart)
    {
        WADPNames pNames = new();

        patchHeaderStart = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        uint numPatches = reader.ReadUInt32();
        pNames.NumMapPatches = numPatches;
        for (uint i = 0; i < numPatches; i++)
        {
            string name = new string(reader.ReadChars(8)).Replace("\0", "").ToUpper();
            pNames.PatchNames.Add(name.Replace(" ", ""));
        }

        reader.BaseStream.Seek(patchHeaderStart, SeekOrigin.Begin);

        return pNames;
    }

    /// <summary>
    /// Loads the TEXTURE1 and TEXTURE2 lumps.
    /// </summary>
    /// <param name="reader">The <see cref="BinaryReader"/>.</param>
    /// <param name="lump">The <see cref="WADLump"/>.</param>
    /// <returns>The <see cref="WadFlat"/>.</returns>
    private WADTexture2 ReadTexture(BinaryReader reader, WADLump lump)
    {
        WADTexture2 texture1or2 = new();

        long currentPos = reader.BaseStream.Position;
        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);

        // 4 bytes for numtextures
        // 4 * numtextures for offsets. It is an array of offsets.
        // Flexible. Offset is the offset to the texture.

        uint numTextures = reader.ReadUInt32();
        List<int> offsets = new();
        for (uint i = 0; i < numTextures; i++)
        {
            offsets.Add(reader.ReadInt32());
        }

        for (int i = 0; i < numTextures; i++)
        {
            // Seek to the offset.
            reader.BaseStream.Seek(lump.FilePos + offsets[i], SeekOrigin.Begin);

            WADTexture texture = new();
            texture.Name = new string(reader.ReadChars(8)).Replace("\0", "");
            reader.ReadInt32(); // mask - which is unused.
            texture.Width = reader.ReadInt16();
            texture.Height = reader.ReadInt16();
            reader.ReadInt32(); // columndirectory - Obsolete, ignored by all DOOM versions.
            texture.PatchCount = reader.ReadInt16();

            for (int j = 0; j < texture.PatchCount; j++)
            {
                WADPatch patch = new();
                patch.OriginX = reader.ReadInt16();
                patch.OriginY = reader.ReadInt16();
                patch.PatchIndex = reader.ReadInt16();
                patch.StepDir = reader.ReadInt16();
                patch.ColorMap = reader.ReadInt16();
                texture.Patches.Add(patch);
            }

            texture1or2.Textures.Add(texture);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return texture1or2;
    }

    /// <summary>
    /// Loads the picture format. Picture format is used for most of DOOM graphics,
    /// except floor and ceilings, which are using flats.
    /// </summary>
    /// <param name="reader">The <see cref="BinaryReader"/>.</param>
    /// <param name="lump">The <see cref="WADLump"/>.</param>
    /// <param name="name">The of patch.</param>
    /// <returns>The <see cref="WadFlat"/>.</returns>
    private WADPictureFormat ReadPictureFormat(BinaryReader reader, WADLump lump, string name)
    {
        WADPictureFormat pictureFormat = new();

        long currentPos = reader.BaseStream.Position;

        reader.BaseStream.Seek(lump.FilePos, SeekOrigin.Begin);
        pictureFormat.Name = name;
        pictureFormat.Width = reader.ReadUInt16();
        pictureFormat.Height = reader.ReadUInt16();
        pictureFormat.LeftOffset = reader.ReadInt16();
        pictureFormat.TopOffset = reader.ReadInt16();

        // Column offsets are stored as an array of 4 byte integers.
        List<uint> offsets = new();
        for (int i = 0; i < pictureFormat.Width; i++)
        {
            offsets.Add(reader.ReadUInt32());
        }

        foreach (uint offset in offsets)
        {
            // Seek to the offset.
            reader.BaseStream.Seek(lump.FilePos + offset, SeekOrigin.Begin);

            List<WADPictureFormatPost> columnPosts = new();

            while (true)
            {
                byte topDelta = reader.ReadByte();
                if (topDelta == 0xFF) break;  // end of column

                byte length = reader.ReadByte();
                reader.ReadByte(); // unused

                WADPictureFormatPost post = new();
                post.TopDelta = topDelta;
                post.Length = length;

                for (int j = 0; j < length; j++)
                {
                    post.Data.Add(reader.ReadByte());
                }

                reader.ReadByte(); // unused

                columnPosts.Add(post);
            }

            pictureFormat.Columns.Add(columnPosts);
        }

        reader.BaseStream.Seek(currentPos, SeekOrigin.Begin);

        return pictureFormat;
    }
}

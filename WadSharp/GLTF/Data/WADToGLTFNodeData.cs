using SharpGLTF.Schema2;
using WadSharp.Parsing;

namespace WadSharp.GLTF.Data;

/// <summary>
/// The data where we store WAD to GLTF node data.
/// </summary>
/// <param name="mesh">The GLTF mesh.</param>
/// <param name="material"> The GLTF material.</param>
/// <param name="hasTransparency">Indicates if the material has transparency.</param>
/// <param name="wadSector"> The WAD sector associated with the mesh.</param>
internal record WADToGLTFNodeData(Mesh mesh, Material material, bool hasTransparency, ParserSector wadSector)
{ }


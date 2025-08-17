using WadSharp;
using WadSharp.GLTF;

args = Environment.GetCommandLineArgs();

Dictionary<string, string> arguments = new();
for(int i = 1; i < args.Length; i+=2)
{
    if (i + 1 < args.Length)
    {
        arguments.Add(args[i], args[i + 1]);
    }
}

// IWAD must be provided as it's the base game data file.
string? iwadFilePath = TryGetIWadFilePath(arguments);
if (iwadFilePath is null)
{
    return;
}

// PWAD is optional as it's a patch wad.
string? pwadFilePath = TryGetPWadFilePath(arguments);

// Level must be provided.
string? level = TryGetLevelName(arguments);
if(level is null)
{
    return;
}

// Get gltf output file path
string? gltfOutputFilePath = TryGetGltfOutputFilePath(arguments);
if (gltfOutputFilePath is null)
{
    return;
}


WADLoader wadLoader = new ();
WAD wad = pwadFilePath is null ? wadLoader.Load(iwadFilePath) : wadLoader.Load(iwadFilePath, pwadFilePath);
WADLevel wadLevel = wad.LoadLevel(level);

WadToGltf wadToGltf = new ();
wadToGltf.ToGLTF(wadLevel, gltfOutputFilePath);

string? TryGetIWadFilePath(Dictionary<string, string> arguments)
{
    if (!arguments.TryGetValue("--iwad", out string? iwadPath)) 
    {
        Console.WriteLine("--iwad argument must be provided.");
        return null;
    }

    if (!File.Exists(iwadPath))
    {
        Console.WriteLine($"IWAD file not found at: {iwadPath}");
        return null;
    }

    return iwadPath;
}

string? TryGetPWadFilePath(Dictionary<string, string> arguments)
{
    if (!arguments.TryGetValue("--pwad", out string? pwadPath))
    {
        Console.WriteLine("--pwad argument must be provided.");
        return null;
    }

    if (!File.Exists(pwadPath))
    {
        Console.WriteLine($"PWAD file not found at: {pwadPath}");
        return null;
    }

    return pwadPath;
}

string? TryGetLevelName(Dictionary<string, string> arguments)
{
    if (!arguments.TryGetValue("--level", out string? levelName))
    {
        Console.WriteLine("--level argument must be provided.");
        return null;
    }

    return levelName;
}

string? TryGetGltfOutputFilePath(Dictionary<string, string> arguments)
{
    if (!arguments.TryGetValue("--gltf", out string? gltfName))
    {
        Console.WriteLine("--gltf argument must be provided.");
        return null;
    }

    return gltfName;
}
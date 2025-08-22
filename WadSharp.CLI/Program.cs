using System.Text;
using WadSharp;
using WadSharp.CLI;
using WadSharp.GLTF;

args = Environment.GetCommandLineArgs();

// If printing help, do it and exit.
if (PrintHelp(args))
{
    return;
}


// IWAD must be provided as it's the base game data file.
string? iwadFilePath = Util.GetArgumentValue(args, "--iwad");
if (iwadFilePath is null)
{
    Console.WriteLine("--iwad argument must be provided.");
    return;
}

// PWAD is optional as it's a patch wad.
string? pwadFilePath = Util.GetArgumentValue(args, "--pwad");

// Level must be provided.
string? level = Util.GetArgumentValue(args, "--level");
if (level is null)
{
    Console.WriteLine("--level argument must be provided.");
    return;
}

// Get gltf output file path
string? gltfOutputFilePath = Util.GetArgumentValue(args, "--gltf");
if (gltfOutputFilePath is null)
{
    Console.WriteLine("--gltf argument must be provided.");
    return;
}


WADLoader wadLoader = new ();
WAD wad = pwadFilePath is null ? wadLoader.Load(iwadFilePath) : wadLoader.Load(iwadFilePath, pwadFilePath);
WADLevel wadLevel = wad.LoadLevel(level);

WadToGltf wadToGltf = new ();
wadToGltf.ToGLTF(wadLevel, gltfOutputFilePath);


bool PrintHelp(string[] arguments)
{
    if(Util.HasArgument(arguments, "--help", "-h"))
    {
        StringBuilder helpText = new();
        helpText.AppendLine("WAD to GLTF Converter");
        helpText.AppendLine();
        helpText.AppendLine("Usage:");
        helpText.AppendLine("  wad-sharp-cli --iwad <path_to_iwad> --pwad <path_to_pwad> --level <level_name> --gltf <output_gltf_path>");
        helpText.AppendLine();
        helpText.AppendLine("Options:");
        helpText.AppendLine("  --iwad <path_to_iwad>       Path to the IWAD file (required)");
        helpText.AppendLine("  --pwad <path_to_pwad>       Path to the PWAD file (optional)");
        helpText.AppendLine("  --level <level_name>        Name of the level to convert (required)");
        helpText.AppendLine("  --gltf <output_gltf_path>   Path to save the output GLTF file (required)");
        helpText.AppendLine("  -h, --help                  Show this help message and exit");
        
        Console.WriteLine(helpText.ToString());

        return true;
    }

    return false;
}
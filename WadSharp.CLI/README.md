## WAD SHARP CLI
---

WAD SHARP CLI is a command-line interface tool for converting WAD file to various 3D model formats, such as GLTF.


### Installation
---

For now you need to build it from source. Make sure you have .NET SDK installed.

Clone the repository and build the project:
   ```bash
   git clone https://github.com/luka712/WadSharp.git
   cd WadSharp
   cd src/WadSharp.Cli
   dotnet build -c Release
   ```

### Usage
---
After building, you can run the CLI tool from build folder using the following command:
   ```bash
   ./wad-sharp-cli --iwad <input.wad> --level <level> --gltf <output.gltf>
   ```

### Command line options
---
- `--iwad <path>`: Path to the input WAD file (required).
- `--pwad <path>`: Path to the PWAD file (optional).`
- - `--level <level>`: Level to convert (required).
- `--gltf <path>`: Path to the output GLTF file (optional).

### As as Library
---
To install as a library, you can use the following command:
   ```bash
   dotnet add package WadSharp 
   ```
Alternatively, find it on NuGet: https://www.nuget.org/packages/WadSharp/



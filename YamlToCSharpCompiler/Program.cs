using Spectre.Console.Cli;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YamlToCSharpCompiler;
// dotnet run /workspaces/yaml-to-class-compiler/test.yml -o /workspaces/yaml-to-class-compiler/genrated/
class Program
{  
    static void Main(string[] args)
    {
        var app = new CommandApp<CompilerCommand>();
        app.Run(args);
    }
}
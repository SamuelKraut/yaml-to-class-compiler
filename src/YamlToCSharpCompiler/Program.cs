using Spectre.Console;
using Spectre.Console.Cli;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;

namespace YamlToCSharpCompiler;
// dotnet run /workspaces/yaml-to-class-compiler/src/test.yml /workspaces/yaml-to-class-compiler/src/cSharp/genrated.cs
class Program
{
    
    static void Main(string[] args)
    {
        var app = new CommandApp<CompilerCommand>();
        app.Run(args);
    }
}
class CompilerCommand : Command<CompilerSettings>
{
    public override int Execute([NotNull] CommandContext context, [NotNull] CompilerSettings settings)
    {
        var c = new Compiler(settings.SourcePath,settings.TargetPath);
        c.Compile();
        return 0;
    }
}
public class CompilerSettings:CommandSettings{
    
    [CommandArgument(0,"<source-path>")]
    public string SourcePath { get; set; }="";
    [CommandArgument(1,"[target-path]")]
    public string TargetPath {get;set;}="";
    public string GetTargetFile(){
        if(TargetPath.EndsWith(".cs"))
            return TargetPath;
        if(string.IsNullOrWhiteSpace(TargetPath)|| TargetPath.EndsWith(Path.PathSeparator))
            return TargetPath+"GeneratedClasses.cs";
        return TargetPath;
    }
    public override ValidationResult Validate()
    {
        if(!File.Exists(SourcePath))
            return ValidationResult.Error("Sourec not exits");
        return ValidationResult.Success();
    }
}
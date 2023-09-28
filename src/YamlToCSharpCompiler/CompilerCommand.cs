using Spectre.Console;
using Spectre.Console.Cli;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace YamlToCSharpCompiler;

class CompilerCommand : Command<CompilerCommand.Settings>
{
    public class Settings:CommandSettings{
    
    [CommandArgument(0,"<source>")]
    [Description("Path to source yml file")]
    public string SourcePath { get; set; }="";
    
    [CommandOption("-o|--output")]
    [Description("Output file")]
    [DefaultValue("./GeneratedClasses.cs")]
    public string TargetPath {get;set;}
    [CommandOption("--debug")]
    [Description("Prints debug informations")]
    public bool Debug{get;set;}
    public string GetTargetFile(){
        if(TargetPath.EndsWith(Path.DirectorySeparatorChar))
            return TargetPath + "GeneratedClasses.cs";
        if(!TargetPath.EndsWith(".cs"))
            return TargetPath + ".cs";
        return TargetPath;
    }
    public override ValidationResult Validate()
    {
        if(!File.Exists(SourcePath))
            return ValidationResult.Error("Sourec not exits");
        return ValidationResult.Success();
    }
}
    public override int Execute([NotNull] CommandContext context, [NotNull] Settings settings)
    {
        var c = new Compiler(settings.SourcePath,settings.GetTargetFile());
        c.Compile();
        return 0;
    }
}
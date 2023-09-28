using YamlDotNet.RepresentationModel;
using System.Globalization;
using System.Text.RegularExpressions;
using Spectre.Console;
namespace YamlToCSharpCompiler;

public class Compiler
{
    const string IdentSpace = "    ";
    const string OpenScope = "{";
    const string CloseScope = "}";
    private readonly string source;
    private readonly string target;
    private readonly bool debug;
    public Compiler(string source,string target,bool debug){
        this.source=source;
        this.target=target;
        this.debug=debug;
    }
    public void Compile(){
        var yamlString = File.ReadAllText(source);
        var result = ParseYamlToDictionary(yamlString);
        // create diecetory if path not exists
        var outPutFile = string.IsNullOrWhiteSpace(target)
            ? "GeneratedClasses.cs"
            : target;
        var dirPath = Path.GetDirectoryName(outPutFile);
        if (!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        GenerateCSharpClasses(result, outPutFile);
    }
    private Dictionary<string, object> ParseYamlToDictionary(string yamlString)
    {
        var yamlStream = new YamlStream();
        yamlStream.Load(new StringReader(yamlString));
        var rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
        return ParseNode(rootNode);
    }
    private Dictionary<string, object> ParseNode(YamlNode node)
    {
        var result = new Dictionary<string, object>();

        foreach (var entry in ((YamlMappingNode)node).Children)
        {
            var key = GeneratePropertyName(((YamlScalarNode)entry.Key).Value);
            var valueNode = entry.Value;

            if (valueNode is YamlMappingNode)
            {
                result[key] = ParseNode(valueNode); // Recursively parse nested types
            }
            else if (valueNode is YamlSequenceNode)
            {
                result[key] = ParseList((YamlSequenceNode)valueNode); // Parse list (array)
            }
            else if (valueNode is YamlScalarNode)
            {
                result[key] = ((YamlScalarNode)valueNode).Value;
            }
            // Add more conditions for other YAML types as needed
        }

        return result;
    }
    // Parses a yaml list
    private List<object> ParseList(YamlSequenceNode node)
    {
        var list = new List<object>();

        foreach (var itemNode in node)
        {
            if (itemNode is YamlMappingNode)
            {
                list.Add(ParseNode(itemNode)); // Recursively parse nested types within the list
            }
            else if (itemNode is YamlScalarNode)
            {
                list.Add(((YamlScalarNode)itemNode).Value);
            }
            // Add more conditions for other YAML types as needed
        }

        return list;
    }
    private void GenerateCSharpClasses(Dictionary<string, object> data, string target)
    {
       
        /* creates a new instance of the `StreamWriter` class and associates it with the specified `target`.
        The `StreamWriter` is used to write the gerneated classes to a file. */
        using (StreamWriter writer = new StreamWriter(target))
        {
            GenerateClass(data, "", writer, "");
        }

        AnsiConsole.Write(new Panel(new TextPath(target)).Header($"C# classes generated and saved to"));
    }
    /// <summary>
    /// Create classes
    /// </summary>
    /// <param name="data">The properties for the class</param>
    /// <param name="className">The class name</param>
    /// <param name="writer">The stream writer where the strings write to</param>
    /// <param name="indentLevel">The indent spaces</param>
    private void GenerateClass(Dictionary<string, object> data, string className, StreamWriter writer, string indentLevel)
    {
        var line=indentLevel + GenerateClassHeader(className);
        writer.WriteLine(line);
        PrintDebug(line);
        line=indentLevel + OpenScope;
        writer.WriteLine(line);
        PrintDebug(line);

        foreach (var kvp in data)
        {
            switch (kvp.Value)
            {
                case Dictionary<string, object> nestedClass:
                    line=indentLevel + IdentSpace + GenerateProperty(GenerateClassName(kvp.Key), kvp.Key);
                    writer.WriteLine(line);
                    PrintDebug(line);
                    GenerateClass(nestedClass, kvp.Key, writer, indentLevel + IdentSpace);
                    break;
                case List<object> list:
                    line=indentLevel + IdentSpace + GenerateProperty($"List<{GetListType(list)}>", kvp.Key);
                    writer.WriteLine(line);
                    PrintDebug(line);
                    break;
                default:
                    line=indentLevel + IdentSpace + GenerateProperty("string", kvp.Key);
                    writer.WriteLine(line);
                    PrintDebug(line);
                    break;
            }
        }
        line=indentLevel + CloseScope;
        writer.WriteLine(); 
        PrintDebug(line);
    }
    
    private string GeneratePropertyName(string name)
    {
        // split string at "-", "_" and when switching from lowerCase to UpperCase
        var parts = Regex.Split(name, @"(?<=[a-z])(?=[A-Z])|[_-]");

        // join TitleCase
        // before: http_routerTest
        // after: HttpRouterTest
        return string.Join("", parts.Select(s => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())));
    }

    private string GenerateProperty(string type, string name)
    {
        return $"public {type} {GeneratePropertyName(name)} {{ get; set; }}";
    }

    private string GenerateClassHeader(string className) => $"public class {GenerateClassName(className)}";

    private string GenerateClassName(string name) => $"{GeneratePropertyName(name)}Configuration";

    private string GetListType(List<object> list) => list.Count > 0 ? list[0].GetType().Name.ToLower() : "object";
    private void PrintDebug(string text){
        if(debug){
            AnsiConsole.Write(new Markup($"[blue]{text}[/]\n"));
        }
    }
}
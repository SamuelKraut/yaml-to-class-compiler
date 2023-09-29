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
    private static readonly Regex intRegex=new("^-?\\d+$");
    private static readonly Regex boolRegex=new("^(?i:true|false)$");
    private static readonly Regex doubleRegex=new("^-?[1-9]\\d*.\\d+$");
    private static readonly Regex propertyNameRegex=new(@"(?<=[a-z])(?=[A-Z])|[_-]");
    private readonly string source;
    private readonly string target;
    private readonly bool debug;

    public Compiler(string source,string target, bool debug)
    {
        this.source = source;
        this.target = string.IsNullOrWhiteSpace(target) ? "GeneratedClasses.cs" : target;
        this.debug = debug;

        var dirPath = Path.GetDirectoryName(target);
        // create diecetory if path not exists
        if (!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
    }

    public void Compile()
    {
        var yamlString = File.ReadAllText(source);
        YamlMappingNode parsedYaml = ParseYaml(yamlString);
        /* creates a new instance of the `StreamWriter` class and associates it with the specified `target`.
        The `StreamWriter` is used to write the gerneated classes to a file. */
        using (StreamWriter writer = new StreamWriter(target))
        {
            GenerateClass(parsedYaml, "", writer, "");
        }
        AnsiConsole.Write(new Panel(new TextPath(target)).Header($"C# classes generated and saved to"));
    }
    private YamlMappingNode ParseYaml(string yamlString)
    {
        var yamlStream = new YamlStream();
        yamlStream.Load(new StringReader(yamlString));
        var parsed = (YamlMappingNode)yamlStream.Documents[0].RootNode;
        return parsed != null ? parsed : throw new Exception("Parsed YAML is empty.");
    }

    /// <summary>
    /// Create classes
    /// </summary>
    /// <param name="data">The properties for the class</param>
    /// <param name="className">The class name</param>
    /// <param name="writer">The stream writer where the strings write to</param>
    /// <param name="indentLevel">The indent spaces</param>
    private void GenerateClass(YamlMappingNode node, string className, StreamWriter writer, string indentLevel)
    {
        var line = indentLevel + GenerateClassHeader(className);
        writer.WriteLine(line);
        PrintDebug(line);
        line = indentLevel + OpenScope;
        writer.WriteLine(line);
        PrintDebug(line);

        foreach (var entry in node.Children)
        {
            var key = GeneratePropertyName(entry.Key.ToString());

            if (entry.Value is YamlMappingNode mappingNode)
            {
                line = indentLevel + IdentSpace + GenerateProperty(GenerateClassName(entry.Key.ToString()), entry.Key.ToString());
                writer.WriteLine(line);
                PrintDebug(line);
                GenerateClass(mappingNode, entry.Key.ToString(), writer, indentLevel + IdentSpace);
            }
            else if (entry.Value is YamlSequenceNode sequenceNode)
            {
                line = indentLevel + IdentSpace + GenerateProperty($"List<{GetListType(ParseList(sequenceNode))}>", entry.Key.ToString());
                writer.WriteLine(line);
                PrintDebug(line);
            }
            else if (entry.Value is YamlScalarNode scalarNode)
            {
                line = indentLevel + IdentSpace + GenerateProperty(GetTypeFromString(entry.Value.ToString()), entry.Key.ToString());
                writer.WriteLine(line);
                PrintDebug(line);
            }
            // Add more conditions for other YAML types as needed
        }

        line = indentLevel + CloseScope;
        writer.WriteLine(line);
        PrintDebug(line);
    }

    private List<object> ParseList(YamlSequenceNode node)
    {
        var list = new List<object>();

        foreach (var itemNode in node)
        {
            //if (itemNode is YamlMappingNode mappingNode)
            //{
            //    list.Add(ParseNode(mappingNode, "", writer, indentLevel + IdentSpace);); // Recursively parse nested types within the list
            //}
            if (itemNode is YamlScalarNode scalarNode)
            {
                list.Add(scalarNode.Value);
            }
            // Add more conditions for other YAML types as needed
        }

        return list;
    }

    private string GetTypeFromString(string? value)
    {
        if (value == null) return "string";

        value = RemoveStartEndQuotes(value);

        // value is a int
        if (intRegex.IsMatch(value)) return "int";
        
        // value is a boolean
        if (boolRegex.IsMatch(value)) return "bool";

        // value is a double
        if (doubleRegex.IsMatch(value)) return "double";

        return "string";
    }

    private string RemoveStartEndQuotes(string input)
    {
        if (input.StartsWith("\"") && input.EndsWith("\""))
        {
            return input.Substring(1, input.Length - 2);
        }
        return input;
    }

    private string GeneratePropertyName(string name)
    {
        // split string at "-", "_" and when switching from lowerCase to UpperCase
        var parts = propertyNameRegex.Split(name);

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

    private string GetListType(List<object> list)
    {
        string listType = "";

        foreach (var item in list)
        {
            var itemType = GetTypeFromString(item.ToString());
            if (listType.Length == 0)
            {
                listType = itemType;
                continue;
            }
            if (!listType.Equals(itemType) || listType.Equals("string")) return "string";
        }
        return listType;
    }

    private void PrintDebug(string text){
        if(debug)
        {
            AnsiConsole.Write(new Markup($"[blue]{text}[/]\n"));
        }
    }
}
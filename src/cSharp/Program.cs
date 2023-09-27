using YamlDotNet.RepresentationModel;
using System.Globalization;
using System.Collections.Generic;
using System.Text.RegularExpressions;

class Program
{
    const string IdentSpace = "    ";
    const string OpenScope = "{";
    const string CloseScope = "}";

    static void Main(string[] args)
    {
        if (args.Length < 1)
            throw new InvalidOperationException("Missing file name");
        var yamlString = File.ReadAllText(args[0]);
        var outPutFile = "GeneratedClasses.cs";
        if (args.Length == 2 && !string.IsNullOrWhiteSpace(args[1]))
        {
            outPutFile = args[1];
        }
        var result = ParseYamlToDictionary(yamlString);
        GenerateCSharpClasses(result, outPutFile);
    }

    static Dictionary<string, object> ParseYamlToDictionary(string yaml)
    {
        var yamlStream = new YamlStream();
        yamlStream.Load(new StringReader(yaml));
        var rootNode = (YamlMappingNode)yamlStream.Documents[0].RootNode;
        return ParseNode(rootNode);
    }

    static Dictionary<string, object> ParseNode(YamlNode node)
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

    static List<object> ParseList(YamlSequenceNode node)
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

    static void GenerateCSharpClasses(Dictionary<string, object> data, string outputFilePath)
    {
        var dirPath = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            GenerateClass(data, "", writer, "");
        }

        Console.WriteLine($"C# classes generated and saved to {outputFilePath}");
    }

    static void GenerateClass(Dictionary<string, object> data, string className, StreamWriter writer, string indentLevel)
    {
        writer.WriteLine(indentLevel + GenerateClassHeader(className));
        writer.WriteLine(indentLevel + OpenScope);

        foreach (var kvp in data)
        {
            switch (kvp.Value)
            {
                case Dictionary<string, object> nestedClass:
                    writer.WriteLine(indentLevel + IdentSpace + GenerateProperty(GenerateClassName(kvp.Key), kvp.Key));
                    GenerateClass(nestedClass, kvp.Key, writer, indentLevel + IdentSpace);
                    break;
                case List<object> list:
                    writer.WriteLine(indentLevel + IdentSpace + GenerateProperty($"List<{GetListType(list)}>", kvp.Key));
                    break;
                default:
                    writer.WriteLine(indentLevel + IdentSpace + GenerateProperty("string", kvp.Key));
                    break;
            }
        }

        writer.WriteLine(indentLevel + CloseScope); 
    }
    
    static string GeneratePropertyName(string name)
    {
        // split string at "-", "_" and when switching from lowerCase to UpperCase
        var parts = Regex.Split(name, @"(?<=[a-z])(?=[A-Z])|[_-]");

        // join TitleCase
        // before: http_routerTest
        // after: HttpRouterTest
        return string.Join("", parts.Select(s => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())));
    }

    static string GenerateProperty(string type, string name)
    {
        return $"public {type} {GeneratePropertyName(name)} {{ get; set; }}";
    }

    static string GenerateClassHeader(string className) => $"public class {GenerateClassName(className)}";

    static string GenerateClassName(string name) => $"{GeneratePropertyName(name)}Configuration";

    static string GetListType(List<object> list) => list.Count > 0 ? list[0].GetType().Name.ToLower() : "object";
}
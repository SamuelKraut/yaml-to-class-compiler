using YamlDotNet;
using YamlDotNet.RepresentationModel;
using System.Globalization;
using System.Text;

class Program
{
    const string IdentSpace="    ";
    static void Main(string[] args)
    {
        if(args.Length<1) 
            throw new InvalidOperationException("Missing file name");
		var yamlString = File.ReadAllText(args[0]);
        var outPutFile="GeneratedClasses.cs";
        if(args.Length==2 && !string.IsNullOrWhiteSpace(args[1])){
            outPutFile=args[1];
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
            var key = ToCSharpPropertyName(((YamlScalarNode)entry.Key).Value);
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
        var dirPath= Path.GetDirectoryName(outputFilePath);
        if(!string.IsNullOrWhiteSpace(dirPath) && !Directory.Exists(dirPath))
            Directory.CreateDirectory(dirPath);
		using (StreamWriter writer = new StreamWriter(outputFilePath))
		{
			GenerateClass(data, "", writer,"");
		}

		Console.WriteLine($"C# classes generated and saved to {outputFilePath}");
	}

	static void GenerateClass(Dictionary<string, object> data, string className, StreamWriter writer,string indentLevel)
	{
		writer.WriteLine($"{indentLevel}public class {ToCSharpConfigClassName(className)}");
		writer.WriteLine(indentLevel+"{");
		foreach (var kvp in data)
		{
			if (kvp.Value is Dictionary<string, object> nestedDict)
			{
				GenerateClass(nestedDict, ToCSharpPropertyName(kvp.Key), writer,indentLevel+IdentSpace); // Recursively generate nested classes
				writer.WriteLine($"{indentLevel}{IdentSpace}public {ToCSharpConfigClassName(kvp.Key)} {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
			else if (kvp.Value is List<object> list)
			{
				var listType = list.Count > 0 ? list[0].GetType().Name : "object";
				writer.WriteLine($"{indentLevel}{IdentSpace}public List<{listType}> {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
			else
			{
				writer.WriteLine($"{indentLevel}{IdentSpace}public string {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
		}
		writer.WriteLine(indentLevel+"}");
	}

	static string ToCSharpPropertyName(string name)
	{
		// Convert to PascalCase following C# conventions
		string[] parts = name.Split('_','-');
		var pascalCaseName = string.Join("", parts.Select(s => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())));
		return pascalCaseName;
	}

    static string ToCSharpConfigClassName(string name){
        return ToCSharpPropertyName(name)+"Configuration";
    }
}
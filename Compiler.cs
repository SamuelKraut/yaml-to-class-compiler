class Program
{
    static void Main()
    {
        string yamlString = @"
            person:
                name: John
                age: 30
                address:
                    street_name: 123 Main St
                    city_name: Anytown
                hobbies:
                    - Reading
                    - Swimming
            ";
		Path.GetFullPath(".").Dump();
        var result = ParseYamlToDictionary(yamlString);
        GenerateCSharpClasses(result, "GeneratedClasses.cs");
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
		using (StreamWriter writer = new StreamWriter(outputFilePath))
		{
			GenerateClass(data, "RootConfig", writer);
		}

		Console.WriteLine($"C# classes generated and saved to {outputFilePath}");
	}

	static void GenerateClass(Dictionary<string, object> data, string className, StreamWriter writer)
	{
		writer.WriteLine($"public class {ToCSharpConfigClassName(className)}");
		writer.WriteLine("{");
		foreach (var kvp in data)
		{
			if (kvp.Value is Dictionary<string, object> nestedDict)
			{
				GenerateClass(nestedDict, ToCSharpPropertyName(kvp.Key), writer); // Recursively generate nested classes
				writer.WriteLine($"    public {ToCSharpConfigClassName(kvp.Key)} {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
			else if (kvp.Value is List<object> list)
			{
				var listType = list.Count > 0 ? list[0].GetType().Name : "object";
				writer.WriteLine($"    public List<{listType}> {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
			else
			{
				writer.WriteLine($"    public string {ToCSharpPropertyName(kvp.Key)} {{ get; set; }}");
			}
		}
		writer.WriteLine("}");
	}

	static string ToCSharpPropertyName(string name)
	{
		// Convert to PascalCase following C# conventions
		string[] parts = name.Split('_');
		var pascalCaseName = string.Join("", parts.Select(s => CultureInfo.CurrentCulture.TextInfo.ToTitleCase(s.ToLower())));
		return pascalCaseName;
	}

	static string ToCSharpTypeName(string name)
	{
		// Convert to PascalCase following C# conventions
		return ToCSharpPropertyName(name);
	}
    static string ToCSharpConfigClassName(string name){
        return ToCSharpPropertyName(name)+"Configuration";
    }
}
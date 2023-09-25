import yaml

def parse_yaml_file(yaml_file_path):
    with open(yaml_file_path, 'r') as file:
        try:
            yaml_data = yaml.safe_load(file)
            return yaml_data
        except yaml.YAMLError as e:
            print("Fehler beim Parsen der YAML-Datei:", e)
            return None

def print_yaml_ast_as_xml(yaml_data, indent=0):
    for key, value in yaml_data.items():
        if isinstance(value, dict):
            print("  " * indent + f"<{key}>")
            print_yaml_ast_as_xml(value, indent + 1)
            print("  " * indent + f"</{key}>")
        elif isinstance(value, list):
            print("  " * indent + f"<{key}>")
            for item in value:
                if isinstance(item, dict):
                    print("  " * (indent + 1) + f"<{key}>")
                    print_yaml_ast_as_xml(item, indent + 2)
                    print("  " * (indent + 1) + f"</{key}>")
                else:
                    print("  " * (indent + 1) + f"<item>{item}</item>")
            print("  " * indent + f"</{key}>")
        else:
            print("  " * indent + f"<{key}>{value}</{key}>")

if __name__ == "__main__":
    parsed_data = parse_yaml_file('./src/test.yml')
    
    if(parsed_data != None):
        print_yaml_ast_as_xml(parsed_data)
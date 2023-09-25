import yaml

def parse_yaml_file(yaml_file_path):
    with open(yaml_file_path, 'r') as file:
        try:
            yaml_data = yaml.safe_load(file)
            return yaml_data
        except yaml.YAMLError as e:
            print("Fehler beim Parsen der YAML-Datei:", e)
            return None

if __name__ == "__main__":
    parsed_data = parse_yaml_file('./src/test.yaml')

    if parsed_data:
        print("Parsed YAML Data:")
        print(parsed_data['http']['routers']['api'])
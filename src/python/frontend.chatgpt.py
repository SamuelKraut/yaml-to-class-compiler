from arpeggio import ParserPython,PTNodeVisitor,visit_parse_tree ,Optional, ZeroOrMore, OneOrMore, EOF
from arpeggio import RegExMatch

def yaml_grammar():
    def document():
        return item, ZeroOrMore('\n', item)
    def comment():
        return "# ", value
    def attribute():
        return regex(r'\w+')
    def instruction():
        return "#", attribute ," ", value
    def item():
        return [sequence, mapping, value]

    def sequence():
        return "-", item

    def mapping():
        return key, ":", item
    
    def key():
        return regex(r'\w+')
    
    def value():
        return regex(r'[^\n]+')

    def regex(r):
        return RegExMatch(r)

    def _():
        return RegExMatch(r'\s*')

    return document

class YamlVisitor(PTNodeVisitor):
    def visit_document(self, node, children):
        return children

    def visit_sequence(self, node, children):
        return {"type": "sequence", "items": children}

    def visit_mapping(self, node, children):
        return {"type": "mapping", "key": children[0], "value": children[1]}

    def visit_key(self, node, children):
        return children[0]

    def visit_scalar(self, node, children):
        return children[0]

    def visit__(self, node, children):
        return None

def compile_yaml(yaml_text):
    parser = ParserPython(yaml_grammar,debug=True,skipws=False)
    parsed_yaml = parser.parse(yaml_text)
    # for i in parsed_yaml:
    #     print(i)
    # return visit_parse_tree(parsed_yaml, YamlVisitor())

if __name__ == "__main__": 
    yaml_string = open('./src/test.yml', 'r').read()
    parsed = compile_yaml(yaml_string)
    print(parsed)
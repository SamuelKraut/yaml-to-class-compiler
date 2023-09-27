from pypeg2 import *
from pypeg2.xmlast import thing2xml
import re


# int = re.compile("\d+")
# double = re.compile(r'[0-9]*\.[0-9]+')
# boolen = re.compile(r'(true|false)')
# string = re.compile(r'("[^"]*"|[^"\s]+)')


# Symbol.regex = re.compile(r"^[^\n\r\s]+")

# class root(Namespace):
#     grammar = name(), ":", endl, some(indent)

Symbol.regex = re.compile(r"\w+")


class Entry(Namespace):
    grammar = name(), ": ", separated, restline, endl
    
class Object(Namespace):
    grammar = name(), ":", endl, maybe_some(indent(Entry))

class YamlFile(Namespace):
    grammar = some(Object)
            
if __name__ == "__main__": 
    yaml_string = open('./src/test.yml', 'r').read()
    print(repr(yaml_string))
    parsed = parse(yaml_string, YamlFile)
    print(thing2xml(parsed, pretty=True).decode())
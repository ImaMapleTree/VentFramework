project = 'VentFramework'
version = '1.0'
release = '1.0'
copyright = ''

master_doc = 'index'
source_suffix = '.rst'
extensions = ['sphinxcsharp.csharp', 'sphinxsharp-pro.sphinxsharp']

pygments_style = 'sphinx'

nitpick_ignore = [
    ('csharp:type', 'void'),
    ('csharp:type', 'T'),
    ('csharp:type', 'List')
]
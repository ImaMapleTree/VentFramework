project = 'VentFramework'
version = '1.0'
release = '1.0'
copyright = ''

master_doc = 'index'
source_suffix = '.rst'
extensions = ['sphinxsharp-pro.sphinxsharp']

pygments_style = 'sphinx'

html_theme = 'default'
html_theme_path = ["_themes", ]

html_static_path = ['_static']
html_css_files = ["sphinxsharp-override.css"]

nitpick_ignore = [
    ('csharp:type', 'void'),
    ('csharp:type', 'T'),
    ('csharp:type', 'List')
]

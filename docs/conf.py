project = 'VentFramework'
version = '1.0'
release = '1.0'
copyright = ''

master_doc = 'index'
source_suffix = '.rst'
extensions = ['sphinxsharp-pro.sphinxsharp']

pygments_style = 'sphinx'

html_theme = 'sphinx_rtdm_theme'
html_theme_path = ["_themes/"]

html_static_path = ['_static']

nitpick_ignore = [
    ('csharp:type', 'void'),
    ('csharp:type', 'T'),
    ('csharp:type', 'List')
]
using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
    /// <summary>
    /// C# abstraction of Tile Map XML format
    /// Design goal:
    ///     - Maximum abstraction without invoking XML framework
    ///     - Learn and adopt code-efficient C# techniques
    /// Unimplemented list:
    ///     - objectgroups
    ///     - ZLib decompression
    ///     - TSX (Tileset XML) sources
    ///     - Layer Tile XElements
    ///     - orientation type checking
    /// Refactoring:
    ///     - 'null' handling, default cases
    ///     - Function separation
    ///     - File separation
    /// </summary>
    public class Map
    {
        public string version;              // TMX version
        public string orientation;          // orthogonal/isometric
        public int width, height;           // Tile count
        public int tilewidth, tileheight;   // Grid size
        
        public List<Tileset> tileset = new List<Tileset>();
        public List<Layer> layer = new List<Layer>();
        public List<ObjectGroup> objectgroup = new List<ObjectGroup>();
        public PropertyDict property;
		
        public Map(string xml_path)
        {
            var xml_doc = XDocument.Load(xml_path);
            var xml_map = xml_doc.Element("map");
			
            version = (string)xml_map.Attribute("version");
            orientation = (string)xml_map.Attribute("orientation");
			
            width = (int)xml_map.Attribute("width");
            height = (int)xml_map.Attribute("height");
            tilewidth = (int)xml_map.Attribute("tilewidth");
            tileheight = (int)xml_map.Attribute("tileheight");
            
            foreach (var t in xml_map.Elements("tileset"))
                tileset.Add(new Tileset(t));
			
            foreach (var e in xml_map.Elements("layer"))
                layer.Add(new Layer(e, width, height));
			
            property = new PropertyDict(xml_map.Element("properties"));
        }
    }

    public class PropertyDict : Dictionary<string, string>
    {
        public PropertyDict(XElement xml_prop)
        {
            if (xml_prop == null) return;
			
            foreach (var p in xml_prop.Elements("property"))
            {
                var pname = (string)p.Attribute("name");
                var pval = (string)p.Attribute("value");
                Add(pname, pval);
            }
        }
    }
}

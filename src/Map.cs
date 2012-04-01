using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Reflection;
using System.IO;

namespace TiledSharp
{
    public class Map
    {
        public string version;              // TMX version
        public Orientation orientation;     // Grid layout
        public int width, height;           // Tile count
        public int tilewidth, tileheight;   // Grid size
        
        public List<Tileset> tileset = new List<Tileset>();
        public List<Layer> layer = new List<Layer>();
        public List<ObjectGroup> objectgroup = new List<ObjectGroup>();
        public PropertyDict property;
        
        public Map(string filename)
        {
            XDocument xml_doc;
            
            var assembly = Assembly.GetCallingAssembly();
            var manifest = assembly.GetManifestResourceNames();
            
            // Process path with respect to asm manifest
            var file_asm = filename.Replace(
                                Path.DirectorySeparatorChar.ToString(), ".");
            var asm_path = Array.Find(manifest, s => s.EndsWith(file_asm));
            
            if (asm_path != null)
            {
                Stream map_stream = assembly.GetManifestResourceStream(asm_path);
                xml_doc = XDocument.Load(map_stream);
            }
            else xml_doc = XDocument.Load(filename);
            
            var xml_map = xml_doc.Element("map");
            
            version = (string)xml_map.Attribute("version");
            orientation = (Orientation) Enum.Parse(
                            typeof(Orientation),
                            xml_map.Attribute("orientation").Value,
                            true);
            width = (int)xml_map.Attribute("width");
            height = (int)xml_map.Attribute("height");
            tilewidth = (int)xml_map.Attribute("tilewidth");
            tileheight = (int)xml_map.Attribute("tileheight");
            
            foreach (var e in xml_map.Elements("tileset"))
                tileset.Add(new Tileset(e));
            
            foreach (var e in xml_map.Elements("layer"))
                layer.Add(new Layer(e, width, height));
            
            property = new PropertyDict(xml_map.Element("properties"));
        }
        
        public enum Orientation : byte
            { Orthogonal, Isometric, Hexagonal, Shifted }
    }
    
    public class PropertyDict : Dictionary<string, string>
    {
        public PropertyDict(XElement xml_prop)
        {
            if (xml_prop == null) return;
            
            foreach (var p in xml_prop.Elements("property"))
            {
                var pname = p.Attribute("name").Value;
                var pval = p.Attribute("value").Value;
                Add(pname, pval);
            }
        }
    }
}

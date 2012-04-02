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
        public MapObjectGroup objectgroup;
        public PropertyDict property;
        
        public Map(string filename)
        {
            XDocument xDoc = TiledIO.ReadXml(filename);
            var xMap = xDoc.Element("map");
            
            version = (string)xMap.Attribute("version");
            orientation = (Orientation) Enum.Parse(
                            typeof(Orientation),
                            xMap.Attribute("orientation").Value,
                            true);
            width = (int)xMap.Attribute("width");
            height = (int)xMap.Attribute("height");
            tilewidth = (int)xMap.Attribute("tilewidth");
            tileheight = (int)xMap.Attribute("tileheight");
            
            foreach (var e in xMap.Elements("tileset"))
                tileset.Add(new Tileset(e));
            
            foreach (var e in xMap.Elements("layer"))
                layer.Add(new Layer(e, width, height));
            
            objectgroup = new MapObjectGroup(xMap.Element("objectgroup"));
            
            property = new PropertyDict(xMap.Element("properties"));
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

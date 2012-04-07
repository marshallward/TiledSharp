using System;
using System.Xml.Linq;

namespace TiledSharp
{
    public class Map : TiledXML
    {
        public string version;              // TMX version
        public Orientation orientation;     // Grid layout
        public int width, height;           // Tile count
        public int tileWidth, tileHeight;   // Grid size
        
        public TiledList tileset = new TiledList();
        public TiledList layer = new TiledList();
        public TiledList objGroup = new TiledList();
        public PropertyDict property;
        
        public Map(string filename)
        {
            XDocument xDoc = ReadXml(filename);
            var xMap = xDoc.Element("map");
            
            version = (string)xMap.Attribute("version");
            orientation = (Orientation) Enum.Parse(
                            typeof(Orientation),
                            xMap.Attribute("orientation").Value,
                            true);
            width = (int)xMap.Attribute("width");
            height = (int)xMap.Attribute("height");
            tileWidth = (int)xMap.Attribute("tilewidth");
            tileHeight = (int)xMap.Attribute("tileheight");
            
            foreach (var e in xMap.Elements("tileset"))
                tileset.Add(new Tileset(e));
            
            foreach (var e in xMap.Elements("layer"))
                layer.Add(new Layer(e, width, height));
            
            foreach (var e in xMap.Elements("objectgroup"))
                objGroup.Add(new MapObjectGroup(e));
            
            property = new PropertyDict(xMap.Element("properties"));
        }
        
        public enum Orientation : byte
            { Orthogonal, Isometric, Hexagonal, Shifted }
    }
}

using System;
using System.Xml.Linq;

namespace TiledSharp
{
    public class Map : TiledXML
    {
        public string version;
        public OrientationType orientation;
        public int width;
        public int height;
        public int tileWidth;
        public int tileHeight;
        
        public TiledList tileset;
        public TiledList layer;
        public TiledList objGroup;
        public PropertyDict property;
        
        public Map(string filename)
        {
            XDocument xDoc = ReadXml(filename);
            var xMap = xDoc.Element("map");
            
            version = (string)xMap.Attribute("version");
            orientation = (OrientationType) Enum.Parse(
                            typeof(OrientationType),
                            xMap.Attribute("orientation").Value,
                            true);
            width = (int)xMap.Attribute("width");
            height = (int)xMap.Attribute("height");
            tileWidth = (int)xMap.Attribute("tilewidth");
            tileHeight = (int)xMap.Attribute("tileheight");
            
            tileset = new TiledList();
            foreach (var e in xMap.Elements("tileset"))
                tileset.Add(new Tileset(e));
            
            layer = new TiledList();
            foreach (var e in xMap.Elements("layer"))
                layer.Add(new Layer(e, width, height));
            
            objGroup = new TiledList();
            foreach (var e in xMap.Elements("objectgroup"))
                objGroup.Add(new MapObjectGroup(e));
            
            property = new PropertyDict(xMap.Element("properties"));
        }
        
        public enum OrientationType : byte
            { Orthogonal, Isometric, Hexagonal, Shifted }
    }
}

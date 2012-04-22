using System;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version {get; private set;}
        public OrientationType Orientation {get; private set;}
        public int width;
        public int height;
        public int tileWidth;
        public int tileHeight;
        
        public TmxList tileset;
        public TmxList layer;
        public TmxList objGroup;
        public PropertyDict property;
        
        public TmxMap(string filename)
        {
            XDocument xDoc = ReadXml(filename);
            var xMap = xDoc.Element("map");
            
            Version = (string)xMap.Attribute("version");
            Orientation = (OrientationType) Enum.Parse(
                                    typeof(OrientationType),
                                    xMap.Attribute("orientation").Value,
                                    true);
            width = (int)xMap.Attribute("width");
            height = (int)xMap.Attribute("height");
            tileWidth = (int)xMap.Attribute("tilewidth");
            tileHeight = (int)xMap.Attribute("tileheight");
            
            tileset = new TmxList();
            foreach (var e in xMap.Elements("tileset"))
                tileset.Add(new TmxTileset(e));
            
            layer = new TmxList();
            foreach (var e in xMap.Elements("layer"))
                layer.Add(new TmxLayer(e, width, height));
            
            objGroup = new TmxList();
            foreach (var e in xMap.Elements("objectgroup"))
                objGroup.Add(new TmxObjectGroup(e));
            
            property = new PropertyDict(xMap.Element("properties"));
        }
        
        public enum OrientationType : byte
            { Orthogonal, Isometric, Hexagonal, Shifted }
    }
}

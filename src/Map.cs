// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version {get; private set;}
        public OrientationType Orientation {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        
        public TmxList Tileset {get; private set;}
        public TmxList Layer {get; private set;}
        public TmxList ObjectGroup {get; private set;}
        public PropertyDict Property {get; private set;}
        
        public TmxMap(string filename)
        {
            XDocument xDoc = ReadXml(filename);
            var xMap = xDoc.Element("map");
            
            Version = (string)xMap.Attribute("version");
            Orientation = (OrientationType) Enum.Parse(
                                    typeof(OrientationType),
                                    xMap.Attribute("orientation").Value,
                                    true);
            Width = (int)xMap.Attribute("width");
            Height = (int)xMap.Attribute("height");
            TileWidth = (int)xMap.Attribute("tilewidth");
            TileHeight = (int)xMap.Attribute("tileheight");
            
            Tileset = new TmxList();
            foreach (var e in xMap.Elements("tileset"))
                Tileset.Add(new TmxTileset(e));
            
            Layer = new TmxList();
            foreach (var e in xMap.Elements("layer"))
                Layer.Add(new TmxLayer(e, Width, Height));
            
            ObjectGroup = new TmxList();
            foreach (var e in xMap.Elements("objectgroup"))
                ObjectGroup.Add(new TmxObjectGroup(e));
            
            Property = new PropertyDict(xMap.Element("properties"));
        }
        
        public enum OrientationType : byte
            { Orthogonal, Isometric, Hexagonal, Shifted }
    }
}

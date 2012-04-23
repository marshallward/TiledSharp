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
        
        public TmxList Tilesets {get; private set;}
        public TmxList Layers {get; private set;}
        public TmxList ObjectGroups {get; private set;}
        public PropertyDict Properties {get; private set;}
        
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
            
            Tilesets = new TmxList();
            foreach (var e in xMap.Elements("tileset"))
                Tilesets.Add(new TmxTileset(e));
            
            Layers = new TmxList();
            foreach (var e in xMap.Elements("layer"))
                Layers.Add(new TmxLayer(e, Width, Height));
            
            ObjectGroups = new TmxList();
            foreach (var e in xMap.Elements("objectgroup"))
                ObjectGroups.Add(new TmxObjectGroup(e));
            
            Properties = new PropertyDict(xMap.Element("properties"));
        }
        
        public enum OrientationType : byte
        {
            Orthogonal,
            Isometric,
            Hexagonal,
            Shifted
        }
    }
}

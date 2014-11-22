// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public OrientationType Orientation {get; private set;}
        public RenderOrderType RenderOrder {get; private set;}
        public TmxColor BackgroundColor {get; private set;}

        public TmxList<TmxTileset> Tilesets {get; private set;}
        public TmxList<TmxLayer> Layers {get; private set;}
        public TmxList<TmxObjectGroup> ObjectGroups {get; private set;}
        public TmxList<TmxImageLayer> ImageLayers {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxMap(string filename)
        {
            XDocument xDoc = ReadXml(filename);
            var xMap = xDoc.Element("map");

            Version = (string)xMap.Attribute("version");

            Width = (int)xMap.Attribute("width");
            Height = (int)xMap.Attribute("height");
            TileWidth = (int)xMap.Attribute("tilewidth");
            TileHeight = (int)xMap.Attribute("tileheight");
            BackgroundColor = new TmxColor(xMap.Attribute("backgroundcolor"));

            var orientDict = new Dictionary<string, OrientationType> {
                {"unknown", OrientationType.Unknown},
                {"orthogonal", OrientationType.Orthogonal},
                {"isometric", OrientationType.Isometric},
                {"staggered", OrientationType.Staggered}
            };

            var orientValue = (string) xMap.Attribute("orientation");
            if (orientValue != null)
                Orientation = orientDict[orientValue];

            var renderDict = new Dictionary<string, RenderOrderType> {
                {"right-down", RenderOrderType.RightDown},
                {"right-up", RenderOrderType.RightUp},
                {"left-down", RenderOrderType.LeftDown},
                {"left-up", RenderOrderType.LeftUp}
            };

            var renderValue = (string) xMap.Attribute("renderorder");
            if (renderValue != null)
                RenderOrder = renderDict[renderValue];

            Tilesets = new TmxList<TmxTileset>();
            foreach (var e in xMap.Elements("tileset"))
                Tilesets.Add(new TmxTileset(e, TmxDirectory));

            Layers = new TmxList<TmxLayer>();
            foreach (var e in xMap.Elements("layer"))
                Layers.Add(new TmxLayer(e, Width, Height));

            ObjectGroups = new TmxList<TmxObjectGroup>();
            foreach (var e in xMap.Elements("objectgroup"))
                ObjectGroups.Add(new TmxObjectGroup(e));

            ImageLayers = new TmxList<TmxImageLayer>();
            foreach (var e in xMap.Elements("imagelayer"))
                ImageLayers.Add(new TmxImageLayer(e, TmxDirectory));

            Properties = new PropertyDict(xMap.Element("properties"));
        }

        public enum OrientationType
        {
            Unknown,
            Orthogonal,
            Isometric,
            Staggered
        }

        public enum RenderOrderType
        {
            RightDown,
            RightUp,
            LeftDown,
            LeftUp
        }
    }
}

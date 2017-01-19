// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.IO;

namespace TiledSharp
{
    public class TmxMap : TmxDocument
    {
        public string Version {get; private set;}
        public int Width {get; private set;}
        public int Height {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int? HexSideLength {get; private set;}
        public OrientationType Orientation {get; private set;}
        public StaggerAxisType StaggerAxis {get; private set;}
        public StaggerIndexType StaggerIndex {get; private set;}
        public RenderOrderType RenderOrder {get; private set;}
        public TmxColor BackgroundColor {get; private set;}
        public int? NextObjectID {get; private set;}

        public TmxList<TmxTileset> Tilesets {get; private set;}
        public TmxList<TmxLayer> Layers {get; private set;}
        public TmxList<TmxObjectGroup> ObjectGroups {get; private set;}
        public TmxList<TmxImageLayer> ImageLayers {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxMap(string filename)
        {
            Load(ReadXml(filename));
        }

        public TmxMap(Stream inputStream)
        {
            Load(XDocument.Load(inputStream));
        }

        public TmxMap(XDocument xDoc)
        {
            Load(xDoc);
        }
         
        private void Load(XDocument xDoc)
        {
            var xMap = xDoc.Element("map");
            Version = (string) xMap.Attribute("version");

            Width = (int) xMap.Attribute("width");
            Height = (int) xMap.Attribute("height");
            TileWidth = (int) xMap.Attribute("tilewidth");
            TileHeight = (int) xMap.Attribute("tileheight");
            HexSideLength = (int?) xMap.Attribute("hexsidelength");

            // Map orientation type
            var orientDict = new Dictionary<string, OrientationType> {
                {"unknown", OrientationType.Unknown},
                {"orthogonal", OrientationType.Orthogonal},
                {"isometric", OrientationType.Isometric},
                {"staggered", OrientationType.Staggered},
                {"hexagonal", OrientationType.Hexagonal},
            };

            var orientValue = (string) xMap.Attribute("orientation");
            if (orientValue != null)
                Orientation = orientDict[orientValue];

            // Hexagonal stagger axis
            var staggerAxisDict = new Dictionary<string, StaggerAxisType> {
                {"x", StaggerAxisType.X},
                {"y", StaggerAxisType.Y},
            };

            var staggerAxisValue = (string) xMap.Attribute("staggeraxis");
            if (staggerAxisValue != null)
                StaggerAxis = staggerAxisDict[staggerAxisValue];

            // Hexagonal stagger index
            var staggerIndexDict = new Dictionary<string, StaggerIndexType> {
                {"odd", StaggerIndexType.Odd},
                {"even", StaggerIndexType.Even},
            };

            var staggerIndexValue = (string) xMap.Attribute("staggerindex");
            if (staggerIndexValue != null)
                StaggerIndex = staggerIndexDict[staggerIndexValue];

            // Tile render order
            var renderDict = new Dictionary<string, RenderOrderType> {
                {"right-down", RenderOrderType.RightDown},
                {"right-up", RenderOrderType.RightUp},
                {"left-down", RenderOrderType.LeftDown},
                {"left-up", RenderOrderType.LeftUp}
            };

            var renderValue = (string) xMap.Attribute("renderorder");
            if (renderValue != null)
                RenderOrder = renderDict[renderValue];

            NextObjectID = (int?)xMap.Attribute("nextobjectid");
            BackgroundColor = new TmxColor(xMap.Attribute("backgroundcolor"));

            Properties = new PropertyDict(xMap.Element("properties"));

            Tilesets = new TmxList<TmxTileset>();
			Layers = new TmxList<TmxLayer>();
			ObjectGroups = new TmxList<TmxObjectGroup>();
			ImageLayers = new TmxList<TmxImageLayer>();

			int layerIndex = 0;
			foreach (var e in xMap.Elements())
			{
				switch (e.Name.LocalName)
				{
					case "tileset":
						Tilesets.Add(new TmxTileset(e, TmxDirectory));
						break;
					case "layer":
						Layers.Add(new TmxLayer(e, layerIndex++, Width, Height));
						break;
					case "objectgroup":
						ObjectGroups.Add(new TmxObjectGroup(e, layerIndex++));
						break;
					case "imagelayer":
						ImageLayers.Add(new TmxImageLayer(e, layerIndex++, TmxDirectory));
						break;
				}
			}
        }
    }

    public enum OrientationType
    {
        Unknown,
        Orthogonal,
        Isometric,
        Staggered,
        Hexagonal
    }

    public enum StaggerAxisType
    {
        X,
        Y
    }

    public enum StaggerIndexType
    {
        Odd,
        Even
    }

    public enum RenderOrderType
    {
        RightDown,
        RightUp,
        LeftDown,
        LeftUp
    }
}

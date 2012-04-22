/* Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
 * Licensed under the Apache License, Version 2.0
 * http://www.apache.org/licenses/LICENSE-2.0 */
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace TiledSharp
{
    public class TmxTileset : TmxDocument, ITmxElement
    {
        public string Name {get; private set;}
        public uint FirstGid {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int Spacing {get; private set;}
        public int Margin {get; private set;}
        
        public TmxImage Image {get; private set;}
        public Dictionary<int, PropertyDict> Tile {get; private set;}
        public PropertyDict Property {get; private set;}
        
        // TSX file constructor
        public TmxTileset(XDocument xDoc) : this(xDoc.Element("tileset")) { }
        
        // TMX tileset element constructor
        public TmxTileset(XElement xTileset)
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string)xTileset.Attribute("source");
            
            if (source != null)
            {
                // source is always preceded by firstgid
                FirstGid = (uint)xFirstGid;
                
                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new TmxTileset(xDocTileset);
                Name = ts.Name;
                TileWidth = ts.TileWidth;
                TileHeight = ts.TileHeight;
                Spacing = ts.Spacing;
                Margin = ts.Margin;
                Image = ts.Image;
                Tile = ts.Tile;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    FirstGid = (uint)xFirstGid;
                
                Name = (string)xTileset.Attribute("name");
                Image = new TmxImage(xTileset.Element("image"));
                TileWidth = (int)xTileset.Attribute("tilewidth");
                TileHeight = (int)xTileset.Attribute("tileheight");
                
                var xSpacing = xTileset.Attribute("spacing");
                if (xSpacing == null)
                    Spacing = 0;
                else
                    Spacing = (int)xSpacing;
                
                var xMargin = xTileset.Attribute("margin");
                if (xMargin == null)
                    Margin = 0;
                else
                    Margin = (int)xMargin;
                
                Tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    Tile.Add(id, new PropertyDict(xProp));
                }
            }
        }
        
        public class TmxImage
        {
            public string Source {get; private set;}
            public uint? trans;                     
            public int Width {get; private set;}
            public int Height {get; private set;}
            
            public TmxImage(XElement xImage)
            {
                Source = (string)xImage.Attribute("source");
                
                var xTrans = (string)xImage.Attribute("trans");
                if (xTrans != null)
                    trans = UInt32.Parse(xTrans, NumberStyles.HexNumber);
                
                Width = (int)xImage.Attribute("width");
                Height = (int)xImage.Attribute("height");
            }
        }
    }
}

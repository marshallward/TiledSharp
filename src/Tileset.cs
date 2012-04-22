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
        public uint FirstGID {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int Spacing {get; private set;}
        public int Margin {get; private set;}
        
        public Image image;
        public Dictionary<int, PropertyDict> tile;
        public PropertyDict Property {get; private set;}
        
        // TSX file
        public TmxTileset(XDocument xDoc) : this(xDoc.Element("tileset")) { }
        
        // TMX tileset element
        public TmxTileset(XElement xTileset)
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string)xTileset.Attribute("source");
   
            if (source != null)
            {
                // source is always preceded by firstgid
                FirstGID = (uint)xFirstGid;
                    
                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new TmxTileset(xDocTileset);
                Name = ts.Name;
                TileWidth = ts.TileWidth;
                TileHeight = ts.TileHeight;
                Spacing = ts.Spacing;
                Margin = ts.Margin;
                image = ts.image;
                tile = ts.tile;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    FirstGID = (uint)xFirstGid;
                
                Name = (string)xTileset.Attribute("name");
                image = new Image(xTileset.Element("image"));
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
                
                tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    tile.Add(id, new PropertyDict(xProp));
                }
            }
        }
        
        public class Image
        {
            public string source;
            public uint? trans;  // 24-bit RGB transparent color
            public int width;
            public int height;
            
            public Image(XElement xImage)
            {
                source = (string)xImage.Attribute("source");
                
                var xml_trans = (string)xImage.Attribute("trans");
                if (xml_trans != null)
                    trans = UInt32.Parse(xml_trans, NumberStyles.HexNumber);
                
                width = (int)xImage.Attribute("width");
                height = (int)xImage.Attribute("height");
            }
        }
    }
}

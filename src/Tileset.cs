/* Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
 * Licensed under the Apache License, Version 2.0
 * http://www.apache.org/licenses/LICENSE-2.0 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;

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
        public Dictionary<int, PropertyDict> Tiles {get; private set;}
        public PropertyDict Properties {get; private set;}

        // TSX file constructor
        public TmxTileset(XDocument xDoc, string tmxDir) :
            this(xDoc.Element("tileset"), tmxDir) { }

        // TMX tileset element constructor
        public TmxTileset(XElement xTileset, string tmxDir = null)
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string)xTileset.Attribute("source");

            if (source != null)
            {
                // Prepend the parent TMX directory if necessary
                source = Path.Combine(tmxDir, source);

                // source is always preceded by firstgid
                FirstGid = (uint)xFirstGid;

                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new TmxTileset(xDocTileset, TmxDirectory);

                Name = ts.Name;
                TileWidth = ts.TileWidth;
                TileHeight = ts.TileHeight;
                Spacing = ts.Spacing;
                Margin = ts.Margin;
                Image = ts.Image;
                Tiles = ts.Tiles;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    FirstGid = (uint)xFirstGid;

                Name = (string)xTileset.Attribute("name");
                Image = new TmxImage(xTileset.Element("image"), tmxDir);
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

                Tiles = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    Tiles.Add(id, new PropertyDict(xProp));
                }
            }
        }
    }
}

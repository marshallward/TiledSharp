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
        public int FirstGid {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int Spacing {get; private set;}
        public int Margin {get; private set;}

        public TmxTileOffset TileOffset {get; private set;}
        public TmxImage Image {get; private set;}
        public TmxList Terrains {get; private set;}
        public Dictionary<int, TmxTile> Tiles {get; private set;}
        public PropertyDict Properties {get; private set;}

        // TSX file constructor
        public TmxTileset(XDocument xDoc, string tmxDir) :
            this(xDoc.Element("tileset"), tmxDir) { }

        // TMX tileset element constructor
        public TmxTileset(XElement xTileset, string tmxDir = "")
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string)xTileset.Attribute("source");

            if (source != null)
            {
                // Prepend the parent TMX directory if necessary
                source = Path.Combine(tmxDir, source);

                // source is always preceded by firstgid
                FirstGid = (int)xFirstGid;

                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new TmxTileset(xDocTileset, TmxDirectory);

                // TODO: Look into some way to iterate over fields (reflection?)
                Name = ts.Name;
                TileWidth = ts.TileWidth;
                TileHeight = ts.TileHeight;
                Spacing = ts.Spacing;
                Margin = ts.Margin;
                TileOffset = ts.TileOffset;
                Image = ts.Image;
                Terrains = ts.Terrains;
                Tiles = ts.Tiles;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    FirstGid = (int)xFirstGid;

                Name = (string)xTileset.Attribute("name");
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

                TileOffset = new TmxTileOffset(xTileset.Element("tileoffset"));
                Image = new TmxImage(xTileset.Element("image"), tmxDir);

                Terrains = new TmxList();
                var xTerrainType = xTileset.Element("terraintype");
                if (xTerrainType != null) {
                    foreach (var e in xTerrainType.Elements("terrain"))
                        Terrains.Add(new TmxTerrain(e));
                }

                Tiles = new Dictionary<int, TmxTile>();
                foreach (var xTile in xTileset.Elements("tile"))
                {
                    var id = (int)xTile.Attribute("id");
                    var tile = new TmxTile(xTile, tmxDir);
                    Tiles.Add(id, tile);
                }
            }
        }
    }

    public class TmxTileOffset
    {
        public int X {get; private set;}
        public int Y {get; private set;}

        public TmxTileOffset(XElement xTileOffset)
        {
            if (xTileOffset == null) {
                X = 0;
                Y = 0;
            } else {
                X = (int)xTileOffset.Attribute("x");
                Y = (int)xTileOffset.Attribute("y");
            }
        }
    }

    public class TmxTerrain : ITmxElement
    {
        public string Name {get; private set;}
        public int Tile {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxTerrain(XElement xTerrain)
        {
            Name = (string)xTerrain.Attribute("name");
            Tile = (int)xTerrain.Attribute("tile");
            Properties = new PropertyDict(xTerrain.Element("properties"));
        }
    }

    public class TmxTile
    {
        // TODO: List of TmxTerrain's, not int id's
        public List<int?> Terrain {get; private set;}
        public double Probability {get; private set;}
        public TmxImage Image {get; private set;}
        public PropertyDict Properties {get; private set;}

        // Human-readable aliases to the Terrain markers
        public int? TopLeft {
            get { return Terrain[0]; }
            private set { Terrain[0] = value; }
        }

        public int? TopRight {
            get { return Terrain[1]; }
            private set { Terrain[1] = value; }
        }

        public int? BottomLeft {
            get { return Terrain[2]; }
            private set { Terrain[2] = value; }
        }
        public int? BottomRight {
            get { return Terrain[3]; }
            private set { Terrain[3] = value; }
        }

        public TmxTile(XElement xTile, string tmxDir = "")
        {
            int result;
            var strTerrain = ((string)xTile.Attribute("terrain")).Split(',');

            for (var i = 0; i < 4; i++) {
                var success = int.TryParse(strTerrain[i], out result);
                if (success)
                    Terrain[i] = result;
                else
                    Terrain[i] = null;
            }

            var xProbability = xTile.Attribute("probability");
            if (xProbability != null) {
                Probability = (double)xProbability;
            } else {
                Probability = 1.0;
            }

            Image = new TmxImage(xTile.Element("image"), tmxDir);

            Properties = new PropertyDict(xTile.Element("properties"));
        }
    }
}

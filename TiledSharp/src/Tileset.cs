/* Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
 * Licensed under the Apache License, Version 2.0
 * http://www.apache.org/licenses/LICENSE-2.0 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Xml.Linq;

namespace TiledSharp
{
    // TODO: The design here is all wrong. A Tileset should be a list of tiles,
    //       it shouldn't force the user to do so much tile ID management

    public class TmxTileset : TmxDocument, ITmxElement
    {
        public int FirstGid {get; private set;}
        public string Name {get; private set;}
        public int TileWidth {get; private set;}
        public int TileHeight {get; private set;}
        public int Spacing {get; private set;}
        public int Margin {get; private set;}
        public int? Columns {get; private set;}
        public int? TileCount {get; private set;}

        public Dictionary<int, TmxTilesetTile> Tiles {get; private set;}
        public TmxTileOffset TileOffset {get; private set;}
        public PropertyDict Properties {get; private set;}
        public TmxImage Image {get; private set;}
        public TmxList<TmxTerrain> Terrains {get; private set;}

        // TSX file constructor
        public TmxTileset(XContainer xDoc, string tmxDir, ICustomLoader customLoader = null) :
            this(xDoc.Element("tileset"), tmxDir, customLoader) { }

        // TMX tileset element constructor
        public TmxTileset(XElement xTileset, string tmxDir = "", ICustomLoader customLoader = null) : base(customLoader)
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string) xTileset.Attribute("source");

            if (source != null)
            {
                // Prepend the parent TMX directory if necessary
                source = Path.Combine(tmxDir, source);

                // source is always preceded by firstgid
                FirstGid = (int) xFirstGid;

                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new TmxTileset(xDocTileset, TmxDirectory, CustomLoader);
                Name = ts.Name;
                TileWidth = ts.TileWidth;
                TileHeight = ts.TileHeight;
                Spacing = ts.Spacing;
                Margin = ts.Margin;
                Columns = ts.Columns;
                TileCount = ts.TileCount;
                TileOffset = ts.TileOffset;
                Image = ts.Image;
                Terrains = ts.Terrains;
                Tiles = ts.Tiles;
                Properties = ts.Properties;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    FirstGid = (int) xFirstGid;

                Name = (string) xTileset.Attribute("name");
                TileWidth = (int) xTileset.Attribute("tilewidth");
                TileHeight = (int) xTileset.Attribute("tileheight");
                Spacing = (int?) xTileset.Attribute("spacing") ?? 0;
                Margin = (int?) xTileset.Attribute("margin") ?? 0;
                Columns = (int?) xTileset.Attribute("columns");
                TileCount = (int?) xTileset.Attribute("tilecount");
                TileOffset = new TmxTileOffset(xTileset.Element("tileoffset"));
                Image = new TmxImage(xTileset.Element("image"), tmxDir);

                Terrains = new TmxList<TmxTerrain>();
                var xTerrainType = xTileset.Element("terraintypes");
                if (xTerrainType != null) {
                    foreach (var e in xTerrainType.Elements("terrain"))
                        Terrains.Add(new TmxTerrain(e));
                }

                Tiles = new Dictionary<int, TmxTilesetTile>();
                foreach (var xTile in xTileset.Elements("tile")) {
                    var tile = new TmxTilesetTile(xTile, Terrains, tmxDir);
                    Tiles[tile.Id] = tile;
                }

                Properties = new PropertyDict(xTileset.Element("properties"));
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

    public class TmxTilesetTile
    {
        public int Id {get; private set;}
        public Collection<TmxTerrain> TerrainEdges {get; private set;}
        public double Probability {get; private set;}
        public string Type { get; private set; }

        public PropertyDict Properties {get; private set;}
        public TmxImage Image {get; private set;}
        public TmxList<TmxObjectGroup> ObjectGroups {get; private set;}
        public Collection<TmxAnimationFrame> AnimationFrames {get; private set;}

        // Human-readable aliases to the Terrain markers
        public TmxTerrain TopLeft {
            get { return TerrainEdges[0]; }
        }

        public TmxTerrain TopRight {
            get { return TerrainEdges[1]; }
        }

        public TmxTerrain BottomLeft {
            get { return TerrainEdges[2]; }
        }
        public TmxTerrain BottomRight {
            get { return TerrainEdges[3]; }
        }

        public TmxTilesetTile(XElement xTile, TmxList<TmxTerrain> Terrains,
                       string tmxDir = "")
        {
            Id = (int)xTile.Attribute("id");

            TerrainEdges = new Collection<TmxTerrain>();

            int result;
            TmxTerrain edge;

            var strTerrain = (string)xTile.Attribute("terrain") ?? ",,,";
            foreach (var v in strTerrain.Split(',')) {
                var success = int.TryParse(v, out result);
                if (success)
                    edge = Terrains[result];
                else
                    edge = null;
                TerrainEdges.Add(edge);

                // TODO: Assert that TerrainEdges length is 4
            }

            Probability = (double?)xTile.Attribute("probability") ?? 1.0;
            if (xTile.Attribute("class") != null)
                Type = (string)xTile.Attribute("class") ?? String.Empty;
            else
                Type = (string)xTile.Attribute("type") ?? String.Empty;
            Image = new TmxImage(xTile.Element("image"), tmxDir);

            ObjectGroups = new TmxList<TmxObjectGroup>();
            foreach (var e in xTile.Elements("objectgroup"))
                ObjectGroups.Add(new TmxObjectGroup(e));

            AnimationFrames = new Collection<TmxAnimationFrame>();
            if (xTile.Element("animation") != null) {
                foreach (var e in xTile.Element("animation").Elements("frame"))
                    AnimationFrames.Add(new TmxAnimationFrame(e));
            }

            Properties = new PropertyDict(xTile.Element("properties"));
        }
    }

    public class TmxAnimationFrame
    {
        public int Id {get; private set;}
        public int Duration {get; private set;}

        public TmxAnimationFrame(XElement xFrame)
        {
            Id = (int)xFrame.Attribute("tileid");
            Duration = (int)xFrame.Attribute("duration");
        }
    }
}

// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Xml.Linq;
using System.IO;
using System.Linq;
namespace TiledSharp
{
    public class TmxLayer : ITmxLayer
    {
        public string Name {get; private set;}
        // TODO: Legacy (Tiled Java) attributes (x, y, width, height)
        public double Opacity {get; private set;}
        public bool Visible {get; private set; }
        public double? OffsetX {get; private set;}
        public double? OffsetY {get; private set;}
        public TmxColor Tint { get; private set; }
        public Collection<TmxLayerTile> Tiles {get; private set;}
        public PropertyDict Properties {get; private set;}
        public TmxLayer(XElement xLayer, int width, int height)
        {
            Name = (string) xLayer.Attribute("name");
            Opacity = (double?) xLayer.Attribute("opacity") ?? 1.0;
            Visible = (bool?) xLayer.Attribute("visible") ?? true;
            OffsetX = (double?) xLayer.Attribute("offsetx") ?? 0.0;
            OffsetY = (double?) xLayer.Attribute("offsety") ?? 0.0;
            Tint = new TmxColor(xLayer.Attribute("tint"));
            var xData = xLayer.Element("data");
            var encoding = (string)xData.Attribute("encoding");
            Tiles = new Collection<TmxLayerTile>();
            IEnumerable<XElement> xChunks = xData.Elements("chunk").ToList();
            if (xChunks.Any())
            {
                foreach (XElement xChunk in xChunks)
                {
                    int chunkWidth = (int)xChunk.Attribute("width");
                    int chunkHeight = (int)xChunk.Attribute("height");
                    int chunkX = (int)xChunk.Attribute("x");
                    int chunkY = (int)xChunk.Attribute("y");
                    ReadChunk(chunkWidth, chunkHeight, chunkX, chunkY, encoding, xChunk);
                }
            }
            else
            {
                ReadChunk(width, height, 0, 0, encoding, xData);
            }
            Properties = new PropertyDict(xLayer.Element("properties"));
        }
        private void ReadChunk(int width, int height, int startX, int startY, string encoding, XElement xData)
        {
            if (encoding == "base64")
            {
                var decodedStream = new TmxBase64Data(xData);
                var stream = decodedStream.Data;
                using (var br = new BinaryReader(stream))
                    for (int j = 0; j < height; j++)
                    for (int i = 0; i < width; i++)
                        Tiles.Add(new TmxLayerTile(br.ReadUInt32(), i + startX, j + startY));
            }
            else if (encoding == "csv")
            {
                var csvData = (string) xData.Value;
                int k = 0;
                foreach (var s in csvData.Split(new[] {',', '\n'}, StringSplitOptions.RemoveEmptyEntries))
                {
                    var gid = uint.Parse(s.Trim());
                    var x = k % width;
                    var y = k / width;
                    Tiles.Add(new TmxLayerTile(gid, x + startX, y + startY));
                    k++;
                }
            }
            else if (encoding == null)
            {
                int k = 0;
                foreach (var e in xData.Elements("tile"))
                {
                    var gid = (uint?) e.Attribute("gid") ?? 0;
                    var x = k % width;
                    var y = k / width;
                    Tiles.Add(new TmxLayerTile(gid, x + startX, y + startY));
                    k++;
                }
            }
            else throw new Exception("TmxLayer: Unknown encoding.");
        }
    }
    public class TmxLayerTile
    {
        // Tile flip bit flags
        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
        const uint FLIPPED_VERTICALLY_FLAG   = 0x40000000;
        const uint FLIPPED_DIAGONALLY_FLAG   = 0x20000000;
        public int Gid {get; private set;}
        public int X {get; private set;}
        public int Y {get; private set;}
        public bool HorizontalFlip {get; private set;}
        public bool VerticalFlip {get; private set;}
        public bool DiagonalFlip {get; private set;}
        public TmxLayerTile(uint id, int x, int y)
        {
            var rawGid = id;
            X = x;
            Y = y;
            // Scan for tile flip bit flags
            bool flip;
            flip = (rawGid & FLIPPED_HORIZONTALLY_FLAG) != 0;
            HorizontalFlip = flip ? true : false;
            flip = (rawGid & FLIPPED_VERTICALLY_FLAG) != 0;
            VerticalFlip = flip ? true : false;
            flip = (rawGid & FLIPPED_DIAGONALLY_FLAG) != 0;
            DiagonalFlip = flip ? true : false;
            // Zero the bit flags
            rawGid &= ~(FLIPPED_HORIZONTALLY_FLAG |
                        FLIPPED_VERTICALLY_FLAG |
                        FLIPPED_DIAGONALLY_FLAG);
            // Save GID remainder to int
            Gid = (int)rawGid;
        }
    }
}

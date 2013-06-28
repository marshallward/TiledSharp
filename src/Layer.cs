// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;

namespace TiledSharp
{
    public class TmxLayer : ITmxElement
    {
        public string Name {get; private set;}
        public double Opacity {get; private set;}
        public bool Visible {get; private set;}

        public List<TmxLayerTile> Tiles {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxLayer(XElement xLayer, int width, int height)
        {
            Name = (string)xLayer.Attribute("name");
            Opacity = (double?)xLayer.Attribute("opacity") ?? 1.0;
            Visible = (bool?)xLayer.Attribute("visible") ?? true;

            var xData = xLayer.Element("data");
            var encoding = (string)xData.Attribute("encoding");

            Tiles = new List<TmxLayerTile>();
            if (encoding == "base64")
            {
                var decodedStream = new TmxBase64Data(xData);
                var stream = decodedStream.Data;

                using (var br = new BinaryReader(stream))
                    for (int j = 0; j < height; j++)
                        for (int i = 0; i < width; i++)
                            Tiles.Add(new TmxLayerTile(br.ReadUInt32(), i, j));
            }
            else if (encoding == "csv")
            {
                var csvData = (string)xData.Value;
                int k = 0;
                foreach (var s in csvData.Split(','))
                {
                    var gid = uint.Parse(s.Trim());
                    var x = k % width;
                    var y = k / width;
                    Tiles.Add(new TmxLayerTile(gid, x, y));
                    k++;
                }
            }
            else if (encoding == null)
            {
                int k = 0;
                foreach (var e in xData.Elements("tile"))
                {
                    var gid = (uint)e.Attribute("gid");
                    var x = k % width;
                    var y = k / width;
                    Tiles.Add(new TmxLayerTile(gid, x, y));
                    k++;
                }
            }
            else throw new Exception("TmxLayer: Unknown encoding.");

            Properties = new PropertyDict(xLayer.Element("properties"));
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

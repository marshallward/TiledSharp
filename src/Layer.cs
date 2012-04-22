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
        public string Name {get; set;}
        
        public double opacity = 1.0;
        public bool visible = true;
        
        public List<LayerTile> tile = new List<LayerTile>();
        public PropertyDict property;
        
        public TmxLayer(XElement xLayer, int width, int height)
        {
            Name = (string)xLayer.Attribute("name");
            
            var xOpacity = xLayer.Attribute("opacity");
            if (xOpacity != null)
                opacity = (double)xOpacity;
            
            var xVisible = xLayer.Attribute("visible");
            if (xVisible != null)
                visible = (bool)xVisible;
            
            var xData = xLayer.Element("data");
            var encoding = (string)xData.Attribute("encoding");
            
            if (encoding == "base64")
            {
                var base64data = Convert.FromBase64String((string)xData.Value);
                Stream stream = new MemoryStream(base64data, false);
                
                var compression = (string)xData.Attribute("compression");
                if (compression == "gzip")
                    stream = new GZipStream(stream, CompressionMode.Decompress,
                                            false);
                else if (compression == "zlib")
                    stream = new Ionic.Zlib.ZlibStream(stream,
                                Ionic.Zlib.CompressionMode.Decompress, false);
                else if (compression != null)
                    throw new Exception("Tiled: Unknown compression.");
                
                using (stream)
                using (var br = new BinaryReader(stream))
                    for (int j = 0; j < height; j++)
                        for (int i = 0; i < width; i++)
                            tile.Add(new LayerTile(br.ReadUInt32(), i, j));
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
                    tile.Add(new LayerTile(gid, x, y));
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
                    tile.Add(new LayerTile(gid, x, y));
                    k++;
                }
            }
            else throw new Exception("Tiled: Unknown encoding.");
            
            property = new PropertyDict(xLayer.Element("properties"));
        }
    }
    
    public class LayerTile
    {
        // Tile flip bit flags
        const uint FLIPPED_HORIZONTALLY_FLAG = 0x80000000;
        const uint FLIPPED_VERTICALLY_FLAG   = 0x40000000;
        const uint FLIPPED_DIAGONALLY_FLAG   = 0x20000000;
        
        public uint gid;            // Global tile ID
        public int x, y;            // Coordinate position
        public bool hflip = false;  // Horizontal flip
        public bool vflip = false;  // Vertical flip
        public bool dflip = false;  // Diagonal flip 
        
        public LayerTile(uint id, int xi, int yi)
        {
            gid = id;
            x = xi;
            y = yi;
            
            // Scan for tile flip bit flags
            if ( (gid & FLIPPED_HORIZONTALLY_FLAG) != 0)
                hflip = true;
            if ( (gid & FLIPPED_VERTICALLY_FLAG) != 0)
                vflip = true;
            if ( (gid & FLIPPED_DIAGONALLY_FLAG) != 0)
                dflip = true;
            
            // Zero the bit flags
            gid &= ~(FLIPPED_HORIZONTALLY_FLAG |
                     FLIPPED_VERTICALLY_FLAG |
                     FLIPPED_DIAGONALLY_FLAG);
        }
    }
}

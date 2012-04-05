using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;

namespace TiledSharp
{
    public class Layer : ITiledClass
    {
        public string Name {get; set;}
        public double opacity = 1.0;
        public bool visible = true;
        
        public List<Tile> tiles = new List<Tile>();
        public PropertyDict property;
        
        public Layer(XElement xLayer, int width, int height)
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
                            tiles.Add(new Tile(br.ReadUInt32(), i, j));
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
                    tiles.Add(new Tile(gid, x, y));
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
                    tiles.Add(new Tile(gid, x, y));
                    k++;
                }
            }
            else throw new Exception("Tiled: Unknown encoding.");
            
            property = new PropertyDict(xLayer.Element("properties"));
        }
        
        public class Tile
        {
            public uint gid;            // Global tile ID
            public int x, y;            // Coordinate position
            public bool hflip = false;  // Horizontal flip
            public bool vflip = false;  // Vertical flip
            public bool dflip = false;  // Diagonal flip 
            
            public Tile(uint id, int xi, int yi)
            {
                // Temporary
                gid = id;
                x = xi;
                y = yi;
                
                // Scan for flip flags
                if ( (gid & TiledIO.FLIPPED_HORIZONTALLY_FLAG) != 0)
                    hflip = true;
                if ( (gid & TiledIO.FLIPPED_VERTICALLY_FLAG) != 0)
                    vflip = true;
                if ( (gid & TiledIO.FLIPPED_DIAGONALLY_FLAG) != 0)
                    dflip = true;
                
                // Zero the top three bits
                gid &= ~(TiledIO.FLIPPED_HORIZONTALLY_FLAG |
                         TiledIO.FLIPPED_VERTICALLY_FLAG |
                         TiledIO.FLIPPED_DIAGONALLY_FLAG);
            }
        }
    }
}

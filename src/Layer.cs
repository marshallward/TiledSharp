using System;
using System.Xml.Linq;
using System.IO;
using System.IO.Compression;

namespace TiledSharp
{
    public class Layer
    {
        public string name;
        public double opacity = 1.0;
        public bool visible = true;
        
        public PropertyDict property;
        public uint[,] data;
        
        public Layer(XElement xLayer, int width, int height)
        {
            name = (string)xLayer.Attribute("name");
            
            var xOpacity = xLayer.Attribute("opacity");
            if (xOpacity != null)
                opacity = (double)xOpacity;
            
            var xVisible = xLayer.Attribute("visible");
            if (xVisible != null)
                visible = (bool)xVisible;
            
            // Allocate the tile index map
            data = new uint[width, height];
            
            var xData = xLayer.Element("data");
            switch ((string)xData.Attribute("encoding"))
            {
                case "base64":
                    var base64data = Convert.FromBase64String((string)xData.Value);
                    Stream stream = new MemoryStream(base64data, false);
                    
                    switch ((string)xData.Attribute("compression"))
                    {
                        case "gzip":
                            stream = new GZipStream(stream, CompressionMode.Decompress, false);
                            break;
                        case "zlib":
                            stream = new Ionic.Zlib.ZlibStream(stream, Ionic.Zlib.CompressionMode.Decompress, false);
                            break;
                        case null:
                            // stream unmodified
                            break;
                        default:
                            throw new Exception("Tiled: Unknown compression.");
                    } // compression switch
                    
                    using (stream)
                    using (var br = new BinaryReader(stream))
                        for (int j = 0; j < height; j++)
                            for (int i = 0; i < width; i++)
                                data[i, j] = br.ReadUInt32();
                    break;
                case "csv":
                    var csvData = (string)xData.Value;
                    int k = 0;
                    foreach (var s in csvData.Split(','))
                    {
                        data[k % width, k / width] = uint.Parse(s.Trim());
                        k++;
                    }
                    break;
                case null:
                    k = 0;  // Declared in case "csv"
                    foreach (var e in xData.Elements("tile"))
                    {
                        var gid = (int)e.Attribute("gid");
                        data[k % width, k / width] = (uint)gid;
                        k++;
                    }
                    break;
                default:
                    throw new Exception("Tiled: Unknown encoding.");
            } // encoding switch
            
            property = new PropertyDict(xLayer.Element("properties"));
        }
    }
}

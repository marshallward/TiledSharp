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

        public Layer(XElement xml_layer, int width, int height)
        {
            name = (string)xml_layer.Attribute("name");

            var xml_opacity = xml_layer.Attribute("opacity");
            if (xml_opacity != null)
                opacity = (double)xml_opacity;

            var xml_visible = xml_layer.Attribute("visible");
            if (xml_visible != null)
                visible = (bool)xml_visible;

            // Allocate the tile index map
            data = new uint[width, height];

            var xml_data = xml_layer.Element("data");
            switch ((string)xml_data.Attribute("encoding"))
            {
                case "base64":
                    var base64_data = Convert.FromBase64String((string)xml_data.Value);
                    Stream stream = new MemoryStream(base64_data, false);

                    switch ((string)xml_data.Attribute("compression"))
                    {
                        case "gzip":
                            stream = new GZipStream(stream, CompressionMode.Decompress, false);
                            break;
                        case "zlib":
                            throw new Exception("Tiled: zlib compression unsupported.");
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
                    var csv_data = (string)xml_data.Value;
                    int k = 0;
                    foreach (var s in csv_data.Split(','))
                    {
                        data[k % width, k / width] = uint.Parse(s.Trim());
                        k++;
                    }
                    break;
                case null:
                    k = 0;  // Declared in case "csv"
                    foreach (var e in xml_data.Elements("tile"))
                    {
                        var gid = (int)e.Attribute("gid");
                        data[k % width, k / width] = (uint)gid;
                        k++;
                    }
                    break;
                default:
                    throw new Exception("Tiled: Unknown encoding.");
            } // encoding switch

            property = new PropertyDict(xml_layer.Element("properties"));
        }
    }
}
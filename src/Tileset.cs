using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace TiledSharp
{
    public class Tileset
    {
        public string name;
        public int firstgid;
        public string source;
        public int? tilewidth, tileheight;
        public int? spacing;
        public int? margin;

        public Image image;
        public PropertyDict property;
        public Dictionary<int, PropertyDict> tile;

        public Tileset(XElement xml_tileset)
        {
            name = (string)xml_tileset.Attribute("name");
            firstgid = (int)xml_tileset.Attribute("firstgid");
            source = (string)xml_tileset.Attribute("source");
            tilewidth = (int?)xml_tileset.Attribute("tilewidth");
            tileheight = (int?)xml_tileset.Attribute("tileheight");

            spacing = (int?)xml_tileset.Attribute("spacing");
            margin = (int?)xml_tileset.Attribute("margin");

            if (source != null)
                throw new Exception("Tiled: TSX tilesets unsupported.");
            else
                image = new Image(xml_tileset.Element("image"));

            property = new PropertyDict(xml_tileset.Element("properties"));

            tile = new Dictionary<int, PropertyDict>();
            foreach (var xml_tile in xml_tileset.Elements("tile"))
            {
                var tid = (int)xml_tile.Attribute("id");
                var xml_prop = xml_tile.Element("properties");
                tile.Add(tid, new PropertyDict(xml_prop));
            }
        }

        public class Image
        {
            public string source;
            public uint? trans;  // 24-bit RGB transparent color

            public Image(XElement xml_image)
            {
                source = (string)xml_image.Attribute("source");

                var xml_trans = (string)xml_image.Attribute("trans");
                if (xml_trans != null)
                    trans = UInt32.Parse(xml_trans, NumberStyles.HexNumber);
            }
        }
    }
}
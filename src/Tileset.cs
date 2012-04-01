using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;
using System.Reflection;

namespace TiledSharp
{
    public class Tileset
    {
        public string source;
        public int? firstgid;   // Required for TMX, but not TSX
        public string name;
        public int? tilewidth, tileheight;
        public int? spacing;
        public int? margin;
        
        public Image image;
        public PropertyDict property;
        public Dictionary<int, PropertyDict> tile;
        
        // Assumes one tileset entry per TSX file
        public Tileset(XDocument xDoc) : this(xDoc.Element("tileset")) { }
        
        public Tileset(XElement xml_tileset)
        {
            source = (string)xml_tileset.Attribute("source");
            firstgid = (int?)xml_tileset.Attribute("firstgid");            
            if (source == null)
            {
                name = (string)xml_tileset.Attribute("name");
                image = new Image(xml_tileset.Element("image"));
                tilewidth = (int?)xml_tileset.Attribute("tilewidth");
                tileheight = (int?)xml_tileset.Attribute("tileheight");
                
                spacing = (int?)xml_tileset.Attribute("spacing");
                margin = (int?)xml_tileset.Attribute("margin");
                
                tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xml_tileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xml_prop = xml_tile.Element("properties");
                    tile.Add(id, new PropertyDict(xml_prop));
                }
            }
            else
            {
                var xDocTileset = TiledIO.ReadXml(source);
                var ts = new Tileset(xDocTileset);
                name = ts.name;
                tilewidth = ts.tilewidth;
                tileheight = ts.tileheight;
                spacing = ts.spacing;
                margin = ts.margin;
                image = ts.image;
                tile = ts.tile;
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

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
        
        public Tileset(XElement xTileset)
        {
            source = (string)xTileset.Attribute("source");
            firstgid = (int?)xTileset.Attribute("firstgid");            
            if (source == null)
            {
                name = (string)xTileset.Attribute("name");
                image = new Image(xTileset.Element("image"));
                tilewidth = (int?)xTileset.Attribute("tilewidth");
                tileheight = (int?)xTileset.Attribute("tileheight");
                
                spacing = (int?)xTileset.Attribute("spacing");
                margin = (int?)xTileset.Attribute("margin");
                
                tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    tile.Add(id, new PropertyDict(xProp));
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
            
            public Image(XElement xImage)
            {
                source = (string)xImage.Attribute("source");
                
                var xml_trans = (string)xImage.Attribute("trans");
                if (xml_trans != null)
                    trans = UInt32.Parse(xml_trans, NumberStyles.HexNumber);
            }
        }
    }
}

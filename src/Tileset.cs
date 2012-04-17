using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Globalization;

namespace TiledSharp
{
    public class Tileset : TiledXML, ITiledElement
    {
        public string Name {get; set;}
        
        public uint firstGid;
        public int tileWidth, tileHeight;
        public int spacing = 0;
        public int margin = 0;
        
        public Image image;
        public Dictionary<int, PropertyDict> tile;
        public PropertyDict property;
        
        // TSX file
        public Tileset(XDocument xDoc) : this(xDoc.Element("tileset")) { }
        
        // TMX tileset element
        public Tileset(XElement xTileset)
        {
            var xFirstGid = xTileset.Attribute("firstgid");
            var source = (string)xTileset.Attribute("source");
   
            if (source != null)
            {
                // source is always preceded by firstgid
                firstGid = (uint)xFirstGid;
                    
                // Everything else is in the TSX file
                var xDocTileset = ReadXml(source);
                var ts = new Tileset(xDocTileset);
                Name = ts.Name;
                tileWidth = ts.tileWidth;
                tileHeight = ts.tileHeight;
                spacing = ts.spacing;
                margin = ts.margin;
                image = ts.image;
                tile = ts.tile;
            }
            else
            {
                // firstgid is always in TMX, but not TSX
                if (xFirstGid != null)
                    firstGid = (uint)xFirstGid;
                
                Name = (string)xTileset.Attribute("name");
                image = new Image(xTileset.Element("image"));
                tileWidth = (int)xTileset.Attribute("tilewidth");
                tileHeight = (int)xTileset.Attribute("tileheight");
                
                var xSpacing = xTileset.Attribute("spacing");
                if (xSpacing != null)
                    spacing = (int)xSpacing;
                
                var xMargin = (int?)xTileset.Attribute("margin");
                if (xMargin != null)
                    margin = (int)xMargin;
                
                tile = new Dictionary<int, PropertyDict>();
                foreach (var xml_tile in xTileset.Elements("tile"))
                {
                    var id = (int)xml_tile.Attribute("id");
                    var xProp = xml_tile.Element("properties");
                    tile.Add(id, new PropertyDict(xProp));
                }
            }
        }
        
        public class Image
        {
            public string source;
            public uint? trans;  // 24-bit RGB transparent color
            public int width;
            public int height;
            
            public Image(XElement xImage)
            {
                source = (string)xImage.Attribute("source");
                
                var xml_trans = (string)xImage.Attribute("trans");
                if (xml_trans != null)
                    trans = UInt32.Parse(xml_trans, NumberStyles.HexNumber);
                
                width = (int)xImage.Attribute("width");
                height = (int)xImage.Attribute("height");
            }
        }
    }
}

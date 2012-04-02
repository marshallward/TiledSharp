using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
    public class MapObjectGroup
    {
        public string name;
        public uint? color;
        public double opacity = 1.0;
        public bool visible = true;
        
        public List<MapObject> obj = new List<MapObject>();
        
        public MapObjectGroup(XElement xObjectGroup)
        {
            name = (string)xObjectGroup.Attribute("name");
            color = (uint?)xObjectGroup.Attribute("color");
            
            var xOpacity = xObjectGroup.Attribute("opacity");
            if (xOpacity != null)
                opacity = (double)xOpacity;
            
            var xVisible = xObjectGroup.Attribute("visible");
            if (xVisible != null)
                visible = (bool)xVisible;
            
            foreach (var e in xObjectGroup.Elements("object"))
                obj.Add(new MapObject(e));
        }
        
        public class MapObject
        {
            public string name;
            public string type;
            public int x, y;
            public int width, height;
            public string gid;
            public List<Tuple<int,int>> points;
            
            public MapObject(XElement xObject)
            {
                name = (string)xObject.Attribute("name");
                type = (string)xObject.Attribute("type");
                x = (int)xObject.Attribute("x");
                y = (int)xObject.Attribute("y");
                width = (int)xObject.Attribute("width");
                height = (int)xObject.Attribute("height");
                gid = (string)xObject.Attribute("gid");
                    
                // Polygon, Polyline
            }
        }
    }
}

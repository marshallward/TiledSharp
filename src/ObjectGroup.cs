using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace TiledSharp
{
    public class MapObjectGroup : ITiledClass
    {
        public string Name {get; set;}
        
        public uint? color;
        public double opacity = 1.0;
        public bool visible = true;
        
        public TiledList obj = new TiledList();
        public PropertyDict property;
        
        public MapObjectGroup(XElement xObjectGroup)
        {
            Name = (string)xObjectGroup.Attribute("name");
            
            var xColor = (string)xObjectGroup.Attribute("color");
            if (xColor != null)
            {
                xColor = xColor.TrimStart("#".ToCharArray());
                color = UInt32.Parse(xColor, NumberStyles.HexNumber);
            }
            
            var xOpacity = xObjectGroup.Attribute("opacity");
            if (xOpacity != null)
                opacity = (double)xOpacity;
            
            var xVisible = xObjectGroup.Attribute("visible");
            if (xVisible != null)
                visible = (bool)xVisible;
            
            foreach (var e in xObjectGroup.Elements("object"))
                obj.Add(new MapObject(e));
            
            property = new PropertyDict(xObjectGroup.Element("properties"));
        }
        
        public class MapObject : ITiledClass
        {
            public string Name {get; set;}
            
            public MapObjectType objType;
            public string type;
            public int x, y;
            public int? width, height;
            public int? gid;
            
            public List<Tuple<int,int>> points;
            public PropertyDict property;
            
            public MapObject(XElement xObject)
            {
                Name = (string)xObject.Attribute("name");
                type = (string)xObject.Attribute("type");
                x = (int)xObject.Attribute("x");
                y = (int)xObject.Attribute("y");
                width = (int?)xObject.Attribute("width");
                height = (int?)xObject.Attribute("height");
                
                // Assess object type and assign appropriate content
                var xGid = xObject.Attribute("gid");
                var xPolygon = xObject.Element("polygon");
                var xPolyline = xObject.Element("polyline");
                
                if (xGid != null)
                {
                    gid = (int?)xGid;
                    objType = MapObjectType.Tile;
                }
                else if (xPolygon != null)
                {
                    points = ParsePoints(xPolygon);  // Fill this in
                    objType = MapObjectType.Polygon;
                }
                else if (xPolyline != null)
                {
                    points = ParsePoints(xPolyline);  // Fill in
                    objType = MapObjectType.Polyline;
                }
                else objType = MapObjectType.Basic;
                
                property = new PropertyDict(xObject.Element("properties"));
            }
            
            public List<Tuple<int, int>> ParsePoints(XElement xPoints)
            {
                var points = new List<Tuple<int, int>>();
                
                var pointString = (string)xPoints.Attribute("points");
                var pointStringPair = pointString.Split(' ');
                foreach (var s in pointStringPair)
                {
                    var pt = s.Split(',');
                    var x = int.Parse(pt[0]);
                    var y = int.Parse(pt[1]);
                    points.Add(Tuple.Create<int, int>(x, y));
                }
                return points;
            }
        }
        
        public enum MapObjectType : byte
        {
            Basic,
            Tile,
            Polygon,
            Polyline
        }
    }
}

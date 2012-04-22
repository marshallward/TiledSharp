// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxObjectGroup : ITmxElement
    {
        public string Name {get; private set;}
        public uint? color;
        public double Opacity {get; private set;}
        public bool Visible {get; private set;}
        
        // Naughty boy, using a keyword...
        public TmxList obj;
        public PropertyDict Property {get; private set;}
        
        public TmxObjectGroup(XElement xObjectGroup)
        {
            Name = (string)xObjectGroup.Attribute("name");
            
            var xColor = (string)xObjectGroup.Attribute("color");
            if (xColor != null)
            {
                xColor = xColor.TrimStart("#".ToCharArray());
                color = UInt32.Parse(xColor, NumberStyles.HexNumber);
            }
            
            var xOpacity = xObjectGroup.Attribute("opacity");
            if (xOpacity == null)
                Opacity = 1.0;
            else
                Opacity = (double)xOpacity;
            
            var xVisible = xObjectGroup.Attribute("visible");
            if (xVisible == null)
                Visible = true;
            else
                Visible = (bool)xVisible;
            
            obj = new TmxList();
            foreach (var e in xObjectGroup.Elements("object"))
                obj.Add(new TmxObject(e));
            
            Property = new PropertyDict(xObjectGroup.Element("properties"));
        }
        
        public class TmxObject : ITmxElement
        {
            public string Name {get; private set;}
            
            public TmxObjectType objType;
            public string Type {get; private set;}
            public int X {get; private set;}
            public int Y {get; private set;}
            public int? width, height;
            public int? gid;
            
            public List<Tuple<int,int>> points;
            public PropertyDict Property {get; private set;}
            
            public TmxObject(XElement xObject)
            {
                var xName = xObject.Attribute("name");
                if (xName == null) Name = "";
                else Name = (string)xName;
                
                Type = (string)xObject.Attribute("type");
                X = (int)xObject.Attribute("x");
                Y = (int)xObject.Attribute("y");
                width = (int?)xObject.Attribute("width");
                height = (int?)xObject.Attribute("height");
                
                // Assess object type and assign appropriate content
                var xGid = xObject.Attribute("gid");
                var xPolygon = xObject.Element("polygon");
                var xPolyline = xObject.Element("polyline");
                
                if (xGid != null)
                {
                    gid = (int?)xGid;
                    objType = TmxObjectType.Tile;
                }
                else if (xPolygon != null)
                {
                    points = ParsePoints(xPolygon);  // Fill this in
                    objType = TmxObjectType.Polygon;
                }
                else if (xPolyline != null)
                {
                    points = ParsePoints(xPolyline);  // Fill in
                    objType = TmxObjectType.Polyline;
                }
                else objType = TmxObjectType.Basic;
                
                Property = new PropertyDict(xObject.Element("properties"));
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
        
        public enum TmxObjectType : byte
        {
            Basic,
            Tile,
            Polygon,
            Polyline
        }
    }
}

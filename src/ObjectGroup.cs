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
        public TmxColor Color {get; private set;}
        public double Opacity {get; private set;}
        public bool Visible {get; private set;}

        public TmxList<TmxObject> Objects {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxObjectGroup(XElement xObjectGroup)
        {
            Name = (string)xObjectGroup.Attribute("name");
            Color = new TmxColor(xObjectGroup.Attribute("color"));

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

            Objects = new TmxList<TmxObject>();
            foreach (var e in xObjectGroup.Elements("object"))
                Objects.Add(new TmxObject(e));

            Properties = new PropertyDict(xObjectGroup.Element("properties"));
        }

        public class TmxObject : ITmxElement
        {
            // Many TmxObjectTypes are distinguished by null values in fields
            // It might be smart to subclass TmxObject
            public string Name {get; private set;}
            public TmxObjectType ObjectType {get; private set;}
            public string Type {get; private set;}
            public int X {get; private set;}
            public int Y {get; private set;}
            public int Width {get; private set;}
            public int Height {get; private set;}
            public double Rotation {get; private set;}
            public int? Gid {get; private set;}
            public bool Visible {get; private set;}

            public List<Tuple<int,int>> Points {get; private set;}
            public PropertyDict Properties {get; private set;}

            public TmxObject(XElement xObject)
            {
                var xName = xObject.Attribute("name");
                if (xName == null)
                    Name = "";
                else
                    Name = (string)xName;

                Type = (string)xObject.Attribute("type");
                X = (int)xObject.Attribute("x");
                Y = (int)xObject.Attribute("y");

                var xVisible = xObject.Attribute("visible");
                if (xVisible == null)
                    Visible = true;
                else
                    Visible = (bool)xVisible;

                var xWidth = xObject.Attribute("width");
                if (xWidth == null)
                    Width = 0;
                else
                    Width = (int)xWidth;

                var xHeight = xObject.Attribute("height");
                if (xHeight == null)
                    Height = 0;
                else
                    Height = (int)xHeight;

                var xRotation = xObject.Attribute("rotation");
                if (xRotation == null)
                    Rotation = 0.0;
                else
                    Rotation = (double)xRotation;

                // Assess object type and assign appropriate content
                var xGid = xObject.Attribute("gid");
                var xEllipse = xObject.Element("ellipse");
                var xPolygon = xObject.Element("polygon");
                var xPolyline = xObject.Element("polyline");

                if (xGid != null)
                {
                    Gid = (int?)xGid;
                    ObjectType = TmxObjectType.Tile;
                }
                else if (xEllipse != null)
                {
                    ObjectType = TmxObjectType.Ellipse;
                }
                else if (xPolygon != null)
                {
                    Points = ParsePoints(xPolygon);
                    ObjectType = TmxObjectType.Polygon;
                }
                else if (xPolyline != null)
                {
                    Points = ParsePoints(xPolyline);
                    ObjectType = TmxObjectType.Polyline;
                }
                else ObjectType = TmxObjectType.Basic;

                Properties = new PropertyDict(xObject.Element("properties"));
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
            Ellipse,
            Polygon,
            Polyline
        }
    }
}

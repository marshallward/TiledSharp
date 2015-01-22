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
        public DrawOrderType DrawOrder {get; private set;}
        public double Opacity {get; private set;}
        public bool Visible {get; private set;}

        public TmxList<TmxObject> Objects {get; private set;}
        public PropertyDict Properties {get; private set;}

        public TmxObjectGroup(XElement xObjectGroup)
        {
            Name = (string)xObjectGroup.Attribute("name");
            Color = new TmxColor(xObjectGroup.Attribute("color"));
            Opacity = (double?)xObjectGroup.Attribute("opacity") ?? 1.0;
            Visible = (bool?)xObjectGroup.Attribute("visible") ?? true;

            var drawOrderDict = new Dictionary<string, DrawOrderType> {
                {"unknown", DrawOrderType.UnknownOrder},
                {"topdown", DrawOrderType.IndexOrder},
                {"index", DrawOrderType.TopDown}
            };

            var drawOrderValue = (string) xObjectGroup.Attribute("draworder");
            if (drawOrderValue != null)
                DrawOrder = drawOrderDict[drawOrderValue];

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
            public double X {get; private set;}
            public double Y {get; private set;}
            public double Width {get; private set;}
            public double Height {get; private set;}
            public double Rotation {get; private set;}
            public TmxLayerTile Tile {get; private set;}
            public bool Visible {get; private set;}

            public List<Tuple<double, double>> Points {get; private set;}
            public PropertyDict Properties {get; private set;}

            public TmxObject(XElement xObject)
            {
                Name = (string)xObject.Attribute("name") ?? "";
                X = (double)xObject.Attribute("x");
                Y = (double)xObject.Attribute("y");
                Width = (double?)xObject.Attribute("width") ?? 0.0;
                Height = (double?)xObject.Attribute("height") ?? 0.0;
                Type = (string)xObject.Attribute("type");
                Visible = (bool?)xObject.Attribute("visible") ?? true;
                Rotation = (double?)xObject.Attribute("rotation") ?? 0.0;

                // Assess object type and assign appropriate content
                var xGid = xObject.Attribute("gid");
                var xEllipse = xObject.Element("ellipse");
                var xPolygon = xObject.Element("polygon");
                var xPolyline = xObject.Element("polyline");

                if (xGid != null)
                {
                    Tile = new TmxLayerTile((uint)xGid,
                                            Convert.ToInt32(Math.Round(X)),
                                            Convert.ToInt32(Math.Round(X)));
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

            public List<Tuple<double, double>> ParsePoints(XElement xPoints)
            {
                var points = new List<Tuple<double, double>>();

                var pointString = (string)xPoints.Attribute("points");
                var pointStringPair = pointString.Split(' ');
                foreach (var s in pointStringPair)
                {
                    var pt = s.Split(',');
                    var x = double.Parse(pt[0]);
                    var y = double.Parse(pt[1]);
                    points.Add(Tuple.Create<double, double>(x, y));
                }
                return points;
            }
        }

        public enum TmxObjectType
        {
            Basic,
            Tile,
            Ellipse,
            Polygon,
            Polyline
        }

        public enum DrawOrderType
        {
            UnknownOrder = -1,
            TopDown,
            IndexOrder
        }
    }
}

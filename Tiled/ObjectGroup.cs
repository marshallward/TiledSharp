using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Tiled
{
    public class ObjectGroup
    {
        public string name;
        public uint color;
        public float opacity = 1.0f;
        public bool visible = true;

        public ObjectGroup(XElement xml_objectgroup)
        { }

        public class Object
        { }

        public class Polygon
        { }

        public class Polyline
        { }
    }
}
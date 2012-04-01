using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace TiledSharp
{
    public class ObjectGroup
    {
        public string name;
        public uint color;
        public float opacity = 1.0f;
        public bool visible = true;
        
        public ObjectGroup(XElement xml_objectgroup)
        {
            
        }
        
        public class Object
        {
            public string name;
            public string type;
            public int x, y;
            public int width, height;
            public string gid;
            public List<Tuple<int,int>> points;
        }
    }
}

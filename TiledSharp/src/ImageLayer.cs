// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxImageLayer : ITmxElement
    {
        public string Name {get; private set;}
		public int Index {get; private set;}

        // TODO: Legacy (Tiled Java) attributes (x, y, width, height)
        public int? Width {get; private set;}
        public int? Height {get; private set;}

        public bool Visible {get; private set;}
        public double Opacity {get; private set;}
        public double OffsetX {get; private set;}
        public double OffsetY {get; private set;}

        public TmxImage Image {get; private set;}

        public PropertyDict Properties {get; private set;}

        public TmxImageLayer(XElement xImageLayer, int index, string tmxDir = "")
        {
            Name = (string) xImageLayer.Attribute("name");
			Index = index;

            Width = (int?) xImageLayer.Attribute("width");
            Height = (int?) xImageLayer.Attribute("height");
            Visible = (bool?) xImageLayer.Attribute("visible") ?? true;
            Opacity = (double?) xImageLayer.Attribute("opacity") ?? 1.0;
            OffsetX = (double?) xImageLayer.Attribute("offsetx") ?? 0.0;
            OffsetY = (double?) xImageLayer.Attribute("offsety") ?? 0.0;

            Image = new TmxImage(xImageLayer.Element("image"), tmxDir);

            Properties = new PropertyDict(xImageLayer.Element("properties"));
        }
    }
}

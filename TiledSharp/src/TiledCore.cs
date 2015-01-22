// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxDocument
    {
        public string TmxDirectory {get; private set;}

        protected XDocument ReadXml(string filepath)
        {
            XDocument xDoc;

            var asm = Assembly.GetEntryAssembly();
            var manifest = new string[0];

            if (asm != null)
                manifest = asm.GetManifestResourceNames();

            var fileResPath = filepath.Replace(
                    Path.DirectorySeparatorChar.ToString(), ".");
            var fileRes = Array.Find(manifest, s => s.EndsWith(fileResPath));

            // If there is a resource in the assembly, load the resource
            // Otherwise, assume filepath is an explicit path
            if (fileRes != null)
            {
                Stream xmlStream = asm.GetManifestResourceStream(fileRes);
                xDoc = XDocument.Load(xmlStream);
                TmxDirectory = "";
            }
            else
            {
                // TODO: Check for existence of file

                xDoc = XDocument.Load(filepath);
                TmxDirectory = Path.GetDirectoryName(filepath);
            }

            return xDoc;
        }
    }

    public interface ITmxElement
    {
        string Name {get;}
    }

    public class TmxList<T> : KeyedCollection<string, T> where T : ITmxElement
    {
        public static Dictionary<Tuple<TmxList<T>, string>, int> nameCount
            = new Dictionary<Tuple<TmxList<T>, string>, int>();

        public new void Add(T t)
        {
            // Rename duplicate entries by appending a number
            var key = Tuple.Create<TmxList<T>, string> (this, t.Name);
            if (this.Contains(t.Name))
                nameCount[key] += 1;
            else
                nameCount.Add(key, 0);
            base.Add(t);
        }

        protected override string GetKeyForItem(T t)
        {
            var key = Tuple.Create<TmxList<T>, string> (this, t.Name);
            var count = nameCount[key];

            var dupes = 0;
            var itemKey = t.Name;

            // For duplicate keys, append a counter
            // For pathological cases, insert underscores to ensure uniqueness
            while (Contains(itemKey)) {
                itemKey = t.Name + String.Concat(Enumerable.Repeat("_", dupes))
                            + count;
                dupes++;
            }

            return itemKey;
        }
    }

    public class PropertyDict : Dictionary<string, string>
    {
        public PropertyDict(XElement xmlProp)
        {
            if (xmlProp == null) return;

            foreach (var p in xmlProp.Elements("property"))
            {
                var pname = p.Attribute("name").Value;
                var pval = p.Attribute("value").Value;
                Add(pname, pval);
            }
        }
    }

    public class TmxImage
    {
        public string Source {get; private set;}
        public string Format {get; private set;}
        public Stream Data {get; private set;}
        public TmxColor Trans {get; private set;}
        public int? Width {get; private set;}
        public int? Height {get; private set;}

        public TmxImage(XElement xImage, string tmxDir = "")
        {
            if (xImage == null) return;

            var xSource = xImage.Attribute("source");

            if (xSource != null)
                // Append directory if present
                Source = Path.Combine(tmxDir, (string)xSource);
            else {
                Format = (string)xImage.Attribute("format");
                var xData = xImage.Element("data");
                var decodedStream = new TmxBase64Data(xData);
                Data = decodedStream.Data;
            }

            Trans = new TmxColor(xImage.Attribute("trans"));
            Width = (int?)xImage.Attribute("width");
            Height = (int?)xImage.Attribute("height");
        }
    }

    public class TmxColor
    {
        public int R;
        public int G;
        public int B;

        public TmxColor(XAttribute xColor)
        {
            if (xColor == null) return;

            var colorStr = ((string)xColor).TrimStart("#".ToCharArray());

            R = int.Parse(colorStr.Substring(0, 2), NumberStyles.HexNumber);
            G = int.Parse(colorStr.Substring(2, 2), NumberStyles.HexNumber);
            B = int.Parse(colorStr.Substring(4, 2), NumberStyles.HexNumber);
        }
    }

    public class TmxBase64Data
    {
        public Stream Data {get; private set;}

        public TmxBase64Data(XElement xData)
        {
            if ((string)xData.Attribute("encoding") != "base64")
                throw new Exception(
                    "TmxBase64Data: Only Base64-encoded data is supported.");

            var rawData = Convert.FromBase64String((string)xData.Value);
            Data = new MemoryStream(rawData, false);

            var compression = (string)xData.Attribute("compression");
            if (compression == "gzip")
                Data = new GZipStream(Data, CompressionMode.Decompress, false);
            else if (compression == "zlib")
                Data = new Ionic.Zlib.ZlibStream(Data,
                        Ionic.Zlib.CompressionMode.Decompress, false);
            else if (compression != null)
                throw new Exception("TmxBase64Data: Unknown compression.");
        }
    }
}

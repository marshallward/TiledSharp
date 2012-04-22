// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TmxDocument
    {
        // Subclass XDocument? Override XDocument.Load?
        protected XDocument ReadXml(string filepath)
        {
            XDocument xDoc;
            
            var asm = Assembly.GetEntryAssembly();
            var manifest = asm.GetManifestResourceNames();
            
            var fileResPath = filepath.Replace(
                    Path.DirectorySeparatorChar.ToString(), ".");
            var fileRes = Array.Find(manifest, s => s.EndsWith(fileResPath));
            
            // If there is a resource in the assembly, load the resource
            // Otherwise, assume filepath is an explicit path
            if (fileRes != null)
            {
                Stream xmlStream = asm.GetManifestResourceStream(fileRes);
                xDoc = XDocument.Load(xmlStream);
            }
            else xDoc = XDocument.Load(filepath);
            
            return xDoc;
        }
    }
    
    public interface ITmxElement
    {
        string Name {get;}
    }
    
    public class TmxList : KeyedCollection<string, ITmxElement>
    {
        public static Dictionary<Tuple<TmxList, string>, int> nameCount
            = new Dictionary<Tuple<TmxList, string>, int>();
        
        public new void Add(ITmxElement t)
        {
            // Rename duplicate entries by appending a number
            var key = Tuple.Create<TmxList, string> (this, t.Name);
            if (this.Contains(t.Name))
                nameCount[key] += 1;
            else
                nameCount.Add(key, 0);
            base.Add(t);
        }
        
        protected override string GetKeyForItem(ITmxElement t)
        {
            var key = Tuple.Create<TmxList, string> (this, t.Name);
            var count = nameCount[key];
            if (count == 0)
                return t.Name;
            else
                return t.Name + count;
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
}

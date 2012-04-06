using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace TiledSharp
{
    public class TiledXML
    {
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
    
    public interface ITiledElement
    {
        string Name {get; set;}
    }
    
    public class TiledList : KeyedCollection<string, ITiledElement>
    {
        public static Dictionary<Tuple<TiledList, string>, int> nameCount
            = new Dictionary<Tuple<TiledList, string>, int>();
        
        public new void Add(ITiledElement tList)
        {
            // Rename duplicate entries by appending a number
            var key = Tuple.Create<TiledList, string> (this, tList.Name);
            if (this.Contains(tList.Name))
            {
                nameCount[key] += 1;
                tList.Name = tList.Name + " " + nameCount[key];
            }
            else nameCount.Add(key, 0);
            
            base.Add(tList);
        }
        
        protected override string GetKeyForItem(ITiledElement tList)
        {
            return tList.Name;
        }
    }
    
    public class PropertyDict : Dictionary<string, string>
    {
        public PropertyDict(XElement xml_prop)
        {
            if (xml_prop == null) return;
            
            foreach (var p in xml_prop.Elements("property"))
            {
                var pname = p.Attribute("name").Value;
                var pval = p.Attribute("value").Value;
                Add(pname, pval);
            }
        }
    }
}

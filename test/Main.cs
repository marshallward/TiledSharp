using System;
using System.Collections.Generic;
using TiledSharp;

namespace UnitTest
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("TiledSharp Unit Test Suite");
            
            List<string> testSuite = new List<string>();
            testSuite.Add("empty.tmx");
            testSuite.Add("default.tmx");
            
            foreach (var tmxFile in testSuite)
            {
                Console.WriteLine("***");
                Console.WriteLine(tmxFile);
                TmxDump(new Map(tmxFile));
            }
        }
        
        public static void TmxDump(Map map)
        {
            Console.WriteLine("TMX version: " + map.version);
            Console.WriteLine("Orientation: " + map.orientation);
            Console.WriteLine("Tilecount: " + map.width + ", " + map.height);
            Console.WriteLine("Tilesize: " + map.tilewidth + ", "
                                + map.tileheight);
            
            foreach (var layer in map.layer)
            {
                Console.WriteLine("Layer: " + layer.name);
            }
        }
    }
}

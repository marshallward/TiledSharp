// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System.IO;
using System.Reflection;
using TiledSharp;
using NUnit.Framework;

namespace TiledSharpTesting
{
    [TestFixture]
    public class TiledSharpTest
    {
        const string examplePath = "assets";

        [Test]
        public void VersionTest()
        {
            var mapPath = GetAssetPath("minimal.tmx");

            TmxMap map = new TmxMap(mapPath);
            string version = map.Version;
            Assert.AreEqual(version, "1.0");
        }

        public string GetAssetPath(string mapFilename)
        {
            var exePath = Assembly.GetExecutingAssembly().Location;
            var rootPath = Directory.GetParent(exePath).Parent.Parent.Parent;
            var mapPath = Path.Combine(rootPath.ToString(), examplePath,
                                       mapFilename);
            return mapPath;
        }
    }

}


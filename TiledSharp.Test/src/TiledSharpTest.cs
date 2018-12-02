// Distributed as part of TiledSharp, Copyright 2012 Marshall Ward
// Licensed under the Apache License, Version 2.0
// http://www.apache.org/licenses/LICENSE-2.0
using System.IO;
using System.Reflection;
using TiledSharp;
using NUnit.Framework;
using System;

namespace TiledSharpTesting
{
    [TestFixture]
    public class TiledSharpTest
    {
        const string mapPath = "assets/minimal.tmx";

        [Test]
        public void VersionTest()
        {
            TmxMap map = new TmxMap(mapPath);
            string version = map.Version;
         
            Assert.AreEqual(version, "1.0");
        }

        [Test]
        public void SizeTest()
        {
            TmxMap map = new TmxMap(mapPath);
            int width = map.Width;
            int height = map.Height;

            Assert.AreEqual(width, 20);
            Assert.AreEqual(height, 20);
        }

        [Test]
        public void TileSizeTest()
        {
            TmxMap map = new TmxMap(mapPath);
            int tileWidth = map.TileWidth;
            int tileHeight = map.TileHeight;

            Assert.AreEqual(tileWidth, 32);
            Assert.AreEqual(tileHeight, 32);
        }
    }
}

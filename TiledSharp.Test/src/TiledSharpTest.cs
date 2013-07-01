using System;
using TiledSharp;
using NUnit.Framework;

namespace TiledSharpTesting
{
    [TestFixture]
    public class TiledSharpTest
    {
        [Test]
        public void VersionTest()
        {
            TmxMap map = new TmxMap("minimal.tmx");
            string version = map.Version;
            Assert.AreEqual(version, "1.0");
        }
    }
}


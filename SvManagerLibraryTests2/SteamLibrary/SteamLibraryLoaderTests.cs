using CommonExtensionLib.Extensions;
using CommonCoreLib.CommonPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SvManagerLibrary.SteamLibrary.Tests
{
    [TestFixture]
    public class SteamLibraryLoaderTests
    {
        [Test]
        public void SteamLibraryLoaderTest()
        {
            var vdfPath = $"{AppDomain.CurrentDomain.BaseDirectory}\\TestData\\libraryfolders.vdf".UnifiedSystemPathSeparator();
            var loader = new SteamLibraryLoader(vdfPath);
            var act = loader.SteamLibraryPathList;
            var exp = new List<SteamLibraryPath>
            {
                new SteamLibraryPath("D:\\Valve\\Steam")
            };

            CollectionAssert.AreEqual(exp, act);
        }
    }
}
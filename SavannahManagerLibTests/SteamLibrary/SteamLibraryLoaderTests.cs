using CommonExtensionLib.Extensions;
using CommonCoreLib.CommonPath;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SvManagerLibrary.SteamLibrary;

namespace SvManagerLibraryTests2.SteamLibrary
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
                new SteamLibraryPath("E:\\Steam"),
                new SteamLibraryPath("D:\\Steam")
            };

            CollectionAssert.AreEqual(exp, act);
        }
    }
}
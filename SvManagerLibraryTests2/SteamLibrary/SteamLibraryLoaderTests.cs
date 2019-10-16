using CommonExtensionLib.Extensions;
using SvManagerLibrary.SteamLibrary;
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
            var vdfPath = "{0}\\{1}".FormatString(AppDomain.CurrentDomain.BaseDirectory, "TestData\\libraryfolders.vdf");
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
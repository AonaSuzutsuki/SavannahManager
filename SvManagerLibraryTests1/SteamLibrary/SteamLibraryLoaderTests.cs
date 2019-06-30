using CommonExtensionLib.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SvManagerLibrary.SteamLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvManagerLibrary.SteamLibrary.Tests
{
    [TestClass()]
    public class SteamLibraryLoaderTests
    {
        [TestMethod()]
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
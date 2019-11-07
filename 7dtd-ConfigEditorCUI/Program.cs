using System;
using _7dtd_ConfigEditorCUI.XMLWrapper;

namespace dtdConfigEditorCUI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var xmlReader = new CommonXmlReader2("Config/vehicles.xml");
            var elem = xmlReader.GetValues();
            //foreach (var item in items)
            //{
            //    var list = xmlReader.GetAttributes("name", $"/vehicles/vehicle[@name='{item}']/property");
            //    list.ForEach(Console.WriteLine);
            //}
            //Console.ReadLine();
        }
    }
}

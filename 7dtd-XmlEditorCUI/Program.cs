using System;
using SavannahXmlLib.XmlWrapper;

namespace dtdConfigEditorCUI
{
    class MainClass
    {
        public static void Main()
        {
            var xmlReader = new SavannahXmlReader("Config/vehicles.xml");
            var elem = xmlReader.GetAllNodes();
            _ = xmlReader.GetNode("/vehicles/vehicle[@name='vehicleGyrocopter']");

            var xmlWriter = new SavannahXmlWriter();
            xmlWriter.Write("test.xml", elem);
            //foreach (var item in items)
            //{
            //    var list = xmlReader.GetAttributes("name", $"/vehicles/vehicle[@name='{item}']/property");
            //    list.ForEach(Console.WriteLine);
            //}
            //Console.ReadLine();
        }
    }
}

using System;
using SvManagerLibrary.XmlWrapper;

namespace dtdConfigEditorCUI
{
    class MainClass
    {
        public static void Main(string[] args)
        {
            var xmlReader = new CommonXmlReader("Config/vehicles.xml");
            var elem = xmlReader.GetAllNodes();
            var elem2 = xmlReader.GetNode("/vehicles/vehicle[@name='vehicleGyrocopter']");

            var xmlWriter = new CommonXmlWriter();
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

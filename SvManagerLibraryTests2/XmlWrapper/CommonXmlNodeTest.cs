using System;
using NUnit.Framework;
using SvManagerLibrary.XmlWrapper;

namespace SvManagerLibraryTests2.XmlWrapper
{
    [TestFixture]
    public class CommonXmlNodeTest
    {
        [Test]
        public void EqualsTest()
        {
            var commonXmlNode1 = CommonXmlNode.CreateRoot("root");
            commonXmlNode1.CreateChildElement("ChildNode", new AttributeInfo[]
            {
                new AttributeInfo
                {
                    Name = "name",
                    Value = "attr"
                }
            }, null, "Value");

            var commonXmlNode2 = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        TagName = "ChildNode",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "attr"
                            }
                        },
                        InnerText = new CommonXmlText { Text = "Value" }
                    }
                }
            };

            var value = commonXmlNode1.Equals(commonXmlNode2);
            Assert.AreEqual(true, value);
        }
    }
}

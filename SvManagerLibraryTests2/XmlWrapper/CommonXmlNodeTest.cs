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
                        InnerText = "Value"
                    }
                }
            };

            var value = commonXmlNode1.Equals(commonXmlNode2);
            Assert.AreEqual(true, value);
        }

        [Test]
        public void ToStringTest()
        {
            var commonXmlNode2 = new CommonXmlNode
            {
                TagName = "root",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        NodeType = XmlNodeType.Tag,
                        TagName = "ChildNode",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "attr"
                            }
                        },
                        ChildNodes = new CommonXmlNode[]
                        {
                            new CommonXmlNode
                            {
                                NodeType = XmlNodeType.Text,
                                InnerText = "Value"
                            }
                        }
                    }
                }
            };

            var act = commonXmlNode2.ToString();
        }
    }
}

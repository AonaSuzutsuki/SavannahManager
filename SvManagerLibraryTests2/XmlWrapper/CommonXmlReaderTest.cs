using System;
using NUnit.Framework;
using SvManagerLibrary.XmlWrapper;
using System.Collections.Generic;

namespace SvManagerLibraryTests2.XmlWrapper
{
    [TestFixture]
    public class CommonXmlReaderTest
    {
        [Test]
        public void DeclarationTest()
        {
            var exp = "version=\"1.0\" encoding=\"UTF-8\"";

            var reader = new CommonXmlReader("TestData/Test.xml");
            var declaration = reader.Declaration;

            Assert.AreEqual(exp, declaration);
        }

        [Test]
        public void GetAttributesTest()
        {
            var exp = new string[]
            {
                "ServerName",
                "ServerName2",
                "ServerDescription",
                "ServerWebsiteURL",
                "Nested",
            };

            var reader = new CommonXmlReader("TestData/Test.xml");
            var names = reader.GetAttributes("name", "/ServerSettings/property");

            CollectionAssert.AreEqual(exp, names);
        }

        [Test]
        public void GetValuesTest()
        {
            var reader = new CommonXmlReader("TestData/Test.xml");
            var attributes = reader.GetValues("/ServerSettings/property"); //[not(@name='Nested')]

            var exp = new List<string>
            {
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。",
                "サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n    test",
                "サーバーの説明を設定します。",
                "サーバーのウェブサイトを設定します。"
            };

            CollectionAssert.AreEqual(exp, attributes);
        }

        [Test]
        public void GetNodeTest()
        {
            var exp = new CommonXmlNode
            {
                TagName = "property",
                Attributes = new List<AttributeInfo>
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "ServerName"
                            },
                            new AttributeInfo
                            {
                                Name = "value",
                                Value = "My Game Host"
                            }
                        },
                InnerText = new CommonXmlText
                {
                    Text = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n  "
                }
            };

            var reader = new CommonXmlReader("TestData/Test.xml");
            var node = reader.GetNode("/ServerSettings/property[@name='ServerName']");

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void GetNodesTest()
        {
            var exp = new CommonXmlNode[]
            {
                new CommonXmlNode
                {
                    TagName = "property",
                    Attributes = new AttributeInfo[]
                            {
                                new AttributeInfo
                                {
                                    Name = "name",
                                    Value = "ServerName"
                                },
                                new AttributeInfo
                                {
                                    Name = "value",
                                    Value = "My Game Host"
                                }
                            },
                    InnerText = new CommonXmlText
                    {
                        Text = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n  "
                    }
                },
                new CommonXmlNode
                {
                    TagName = "property",
                    Attributes = new AttributeInfo[]
                            {
                                new AttributeInfo
                                {
                                    Name = "name",
                                    Value = "ServerName2"
                                },
                                new AttributeInfo
                                {
                                    Name = "value",
                                    Value = "My Game Host"
                                }
                            },
                    InnerText = new CommonXmlText
                    {
                        Text = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n        test\n  "
                    }
                },
            };

            var reader = new CommonXmlReader("TestData/Test.xml");
            var node = reader.GetNodes("/ServerSettings/property[contains(@name, 'ServerName')]");

            Assert.AreEqual(exp, node);
        }

        [Test]
        public void GetAllNodesTest()
        {
            var exp = new CommonXmlNode
            {
                TagName = "ServerSettings",
                ChildNodes = new CommonXmlNode[]
                {
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerName"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "My Game Host"
                                    }
                                },
                        InnerText = new CommonXmlText
                        {
                            Text = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n  "
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerName2"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "My Game Host"
                                    }
                                },
                        InnerText = new CommonXmlText
                        {
                            Text = "\n    サーバー名を設定します。サーバーリストにはこの名前で表示されます。\n        test\n  "
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerDescription"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = "A 7 Days to Die server"
                                    }
                                },
                        InnerText = new CommonXmlText
                        {
                            Text = "サーバーの説明を設定します。"
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "ServerWebsiteURL"
                                    },
                                    new AttributeInfo
                                    {
                                        Name = "value",
                                        Value = ""
                                    }
                                },
                        InnerText = new CommonXmlText
                        {
                            Text = "サーバーのウェブサイトを設定します。"
                        }
                    },
                    new CommonXmlNode
                    {
                        TagName = "property",
                        Attributes = new AttributeInfo[]
                        {
                            new AttributeInfo
                            {
                                Name = "name",
                                Value = "Nested"
                            }
                        },
                        ChildNodes = new CommonXmlNode[]
                        {
                            new CommonXmlNode
                            {
                                TagName = "property",
                                Attributes = new AttributeInfo[]
                                {
                                    new AttributeInfo
                                    {
                                        Name = "name",
                                        Value = "NestedElem"
                                    }
                                },
                                InnerText = new CommonXmlText { Text = "Value" }
                            }
                        }
                    }
                }
            };

            var reader = new CommonXmlReader("TestData/Test.xml");
            var node = reader.GetAllNodes();

            Assert.AreEqual(exp, node);
        }
    }
}

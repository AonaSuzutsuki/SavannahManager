using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Update.Views;
using CommonExtensionLib.Extensions;
using SvManagerLibrary.XmlWrapper;
using CommonStyleLib.File;
using Color = System.Drawing.Color;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdateManager
    {
        public Dictionary<string, IEnumerable<RichTextItem>> Updates { get; }

        public bool IsUpdate = false;
        public bool IsUpdUpdate = false;
        public string Version = "1.0.0.0";

        public UpdateManager(UpdateLink updLink, string updFilePath)
        {
            var ret = CheckUpdate(updLink.VersionUrl, ConstantValues.Version);
            IsUpdate = ret.Item1;
            Version = ret.Item2;
            ret = CheckUpdate(updLink.UpdVersionUrl, CommonCoreLib.CommonFile.Version.GetVersion(updFilePath));
            IsUpdUpdate = ret.Item1;

            byte[] data;
            using (var wc = new WebClient())
            {
                data = wc.DownloadData(updLink.XmlUrl);
            }

            using var stream = new MemoryStream(data);
            var reader = new CommonXmlReader(stream);
            var nodes = reader.GetNodes("/updates/update");

            Updates = Analyze(nodes);

            //var items = (from node in nodes
            //            let attr = node.GetAttribute("version")
            //            let value = node.InnerText
            //            select new { Attribute = attr, Value = value }).ToList();

            //var count = items.Count;
            //Updates = new Dictionary<string, string>(count);
            //foreach (var item in items)
            //{
            //    Updates.Add(item.Attribute.Value, item.Value);
            //}
        }

        private Dictionary<string, IEnumerable<RichTextItem>> Analyze(IEnumerable<CommonXmlNode> nodes)
        {
            var dict = new Dictionary<string, IEnumerable<RichTextItem>>();

            foreach (var node in nodes)
            {
                var items = new List<RichTextItem>();
                if (node.ChildNodes.Any())
                    Analyze2(node.ChildNodes, items);
                dict.Add(node.GetAttribute("version").Value, items);
            }

            return dict;
        }

        private void Analyze2(IEnumerable<CommonXmlNode> nodes, List<RichTextItem> items)
        {
            foreach (var node in nodes)
            {

                if (node.NodeType == XmlNodeType.Text)
                {
                    var array = node.InnerText.UnifiedBreakLine().Split('\n');
                    foreach (var text in array)
                    {
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph
                        };
                        paragraph.AddChildren(new RichTextItem
                        {
                            Text = text,
                        });
                        items.Add(paragraph);
                    }
                }
                else
                {
                    if (node.TagName == "font")
                    {
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph,
                            Children = new []
                            {
                                Analyze3(node)
                            }
                        };
                        items.Add(paragraph);
                    }
                    else if (node.TagName == "a")
                    {
                        var link = node.GetAttribute("href").Value;
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph
                        };
                        var item = new RichTextItem
                        {
                            TextType = RichTextType.Hyperlink,
                            Link = link
                        };

                        foreach (var child in node.ChildNodes)
                        {
                            item.AddChildren(Analyze3(child));
                        }

                        paragraph.AddChildren(item);
                        items.Add(paragraph);
                    }
                    else
                    {
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph
                        };
                        items.Add(paragraph);
                    }
                }
            }
        }

        private static RichTextItem Analyze3(CommonXmlNode node)
        {
            if (node.TagName == "font")
            {
                var colorCode = node.GetAttribute("color").Value;
                var c = ColorTranslator.FromHtml(colorCode);
                var color = System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);

                var item = new RichTextItem
                {
                    Text = node.InnerText,
                    Foreground = color
                };

                return item;
            }

            return null;
        }

        public static (bool, string) CheckUpdate(string url, string version)
        {
            using var wc = new WebClient();
            var data = wc.DownloadData(url);
            var urlVersion = Encoding.UTF8.GetString(data);

            return (!urlVersion.Equals(version), urlVersion);
        }

        public static async Task<(bool, string)> CheckUpdateAsync(string url, string version)
        {
            using var wc = new WebClient();
            var data = await wc.DownloadDataTaskAsync(url);
            var urlVersion = Encoding.UTF8.GetString(data);

            return (!urlVersion.Equals(version), urlVersion);
        }
    }
}

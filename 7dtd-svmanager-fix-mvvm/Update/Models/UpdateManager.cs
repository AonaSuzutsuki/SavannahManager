using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Update.Views;
using CommonExtensionLib.Extensions;
using SavannahXmlLib.XmlWrapper;
using CommonStyleLib.File;
using SavannahXmlLib.XmlWrapper.Nodes;
using UpdateLib.Http;
using UpdateLib.Update;
using Color = System.Drawing.Color;

namespace _7dtd_svmanager_fix_mvvm.Update.Models
{
    public class UpdateManager
    {
        public Dictionary<string, IEnumerable<RichTextItem>> Updates { get; private set; }

        public bool IsUpdate;
        public bool IsUpdUpdate;

        private UpdateClient _updateClient;

        public string LatestVersion { get; private set; } = "1.0.0.0";
        public string CurrentVersion { get; } = ConstantValues.Version;

        public async Task Initialize()
        {
            _updateClient = GetUpdateClient();

            try
            {
                var latestVersion = await _updateClient.GetVersion("main");
                var latestUpdVersion = await _updateClient.GetVersion("updater");
                var updVersion = CommonCoreLib.File.Version.GetVersion(ConstantValues.UpdaterFilePath);

                IsUpdate = latestVersion != CurrentVersion;
                IsUpdUpdate = updVersion != latestUpdVersion;

                var details = await _updateClient.DownloadFile(_updateClient.DetailVersionInfoDownloadUrlPath);

                using var stream = new MemoryStream(details);
                var reader = new SavannahXmlReader(stream);
                var nodes = reader.GetNodes("/updates/update").OfType<SavannahTagNode>();

                Updates = Analyze(nodes);

                LatestVersion = latestVersion;
            }
            catch (NotEqualsHashException e)
            {
                Console.WriteLine(e.StackTrace);
                throw;
            }
        }

        public IEnumerable<string> GetVersions()
        {
            return Updates.Keys.ToList();
        }

        public async Task ApplyUpdUpdate(string extractDirPath)
        {
            var updData = await _updateClient.DownloadUpdateFile();
            using var ms = new MemoryStream(updData.Length);
            ms.Write(updData, 0, updData.Length);
            ms.Seek(0, SeekOrigin.Begin);
            using var zip = new UpdateLib.Archive.Zip(ms, ConstantValues.AppDirectoryPath + "/");
            zip.Extract();
        }

        public static UpdateClient GetUpdateClient()
        {
            return GetClient().Item1;
        }

        public ProcessStartInfo GetUpdaterInfo(int pid)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = ConstantValues.UpdaterFilePath,
                Arguments = $"{pid} SavannahManager2.exe \"{_updateClient.WebClient.BaseUrl}\" \"{_updateClient.MainDownloadUrlPath}\""
            };

            return startInfo;
        }

        private Dictionary<string, IEnumerable<RichTextItem>> Analyze(IEnumerable<SavannahTagNode> nodes)
        {
            var dict = new Dictionary<string, IEnumerable<RichTextItem>>();

            foreach (var node in nodes)
            {
                var items = new List<RichTextItem>();
                if (node.ChildNodes.Any())
                    AddRichTextItem(node.ChildNodes, items);
                dict.Add(node.GetAttribute("version").Value, items);
            }

            return dict;
        }

        private static void AddRichTextItem(IEnumerable<AbstractSavannahXmlNode> nodes, List<RichTextItem> items)
        {
            foreach (var node in nodes)
            {
                if (node is SavannahTextNode)
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
                            Text = text
                        });
                        items.Add(paragraph);
                    }
                }
                else if (node is SavannahTagNode tagNode)
                {
                    if (tagNode.TagName == "nobr")
                    {
                        items.Add(new RichTextItem
                        {
                            TextType = RichTextType.NoBreakLine
                        });
                    }
                    else if (tagNode.TagName == "space")
                    {
                        items.Add(new RichTextItem
                        {
                            TextType = RichTextType.Space
                        });
                    }
                    else if (tagNode.TagName == "font")
                    {
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph,
                            Children = new []
                            {
                                AnalyzeTag(tagNode)
                            }
                        };
                        items.Add(paragraph);
                    }
                    else if (tagNode.TagName == "a")
                    {
                        var link = tagNode.GetAttribute("href").Value;
                        var paragraph = new RichTextItem
                        {
                            TextType = RichTextType.Paragraph
                        };
                        var item = new RichTextItem
                        {
                            TextType = RichTextType.Hyperlink,
                            Link = link
                        };

                        foreach (var child in tagNode.ChildNodes)
                        {
                            item.AddChildren(AnalyzeTag(child));
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

        private static RichTextItem AnalyzeTag(AbstractSavannahXmlNode node)
        {
            if (node.TagName == "font" && node is SavannahTagNode tagNode)
            {
                var colorCode = tagNode.GetAttribute("color").Value;
                var c = ColorTranslator.FromHtml(colorCode);
                var color = System.Windows.Media.Color.FromRgb(c.R, c.G, c.B);

                var item = new RichTextItem
                {
                    Text = node.InnerText,
                    Foreground = color
                };

                return item;
            }

            if (node is SavannahTextNode)
            {
                return new RichTextItem
                {
                    Text = node.InnerText
                };
            }

            return null;
        }

        private static (UpdateClient, UpdateWebClient) GetClient()
        {
            var webClient = new UpdateWebClient
            {
                BaseUrl = "https://aonsztk.xyz/KimamaLab/Updates/SavannahManager2/"
            };
            var updateClient = new UpdateClient(webClient)
            {
                DetailVersionInfoDownloadUrlPath = $"details/{LangResources.UpdResources.Updater_XMLNAME}"
            };

            return (updateClient, webClient);
        }

        public static async Task<bool> CheckCanUpdate(UpdateClient updateClient)
        {
            var currentVersion = ConstantValues.Version;
            var latestVersion = await updateClient.GetVersion("main");

            return currentVersion != latestVersion;
        }
    }
}

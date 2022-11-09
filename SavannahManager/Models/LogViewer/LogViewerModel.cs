using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer
{
    public class LogViewerModel : ModelBase
    {
        private LogFileInfo _currentFileInfo;
        private ObservableCollection<RichTextItem> _richLogDetailItems = new();
        private readonly Dictionary<string, LogCacheItem> _cache = new();
        private ObservableCollection<ChatInfo> _chatInfos = new();
        private ObservableCollection<PlayerItemInfo> _playerItemInfos = new();

        public ObservableCollection<LogFileInfo> LogFileList { get; set; } = new();

        public ObservableCollection<RichTextItem> RichLogDetailItems
        {
            get => _richLogDetailItems;
            set => SetProperty(ref _richLogDetailItems, value);
        }

        public ObservableCollection<ChatInfo> ChatInfos
        {
            get => _chatInfos;
            set => SetProperty(ref _chatInfos, value);
        }

        public ObservableCollection<PlayerItemInfo> PlayerItemInfos
        {
            get => _playerItemInfos;
            set => SetProperty(ref _playerItemInfos, value);
        }

        public void Load()
        {
            var files = CommonCoreLib.File.DirectorySearcher.GetAllFiles("logs", "*.log").ToList();
            files.Reverse();

            foreach (var file in files)
            {
                LogFileList.Add(new LogFileInfo(file));
            }
        }

        public async Task AnalyzeLogFile(int index)
        {
            if (index < 0 && index >= LogFileList.Count)
                return;

            _currentFileInfo?.ClearCache();

            var logFileInfo = LogFileList[index];
            _currentFileInfo = logFileInfo;
            if (_cache.ContainsKey(logFileInfo.FullPath))
            {
                var cache = _cache[logFileInfo.FullPath];
                RichLogDetailItems = cache.RichTextItems;
                ChatInfos = cache.ChatItems;
                PlayerItemInfos = cache.PlayerItems;

                return;
            }

            await Task.Factory.StartNew(() =>
            {
                var list = new List<RichTextItem>(LogFileList.Count);
                var chats = new List<ChatInfo>();
                var players = new Dictionary<string, List<PlayerItemInfo>>();
                using var reader = logFileInfo.GetStringReader();
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var paragraph = AnalyzeLine(line);
                    list.Add(paragraph);

                    var chatInfo = ChatInfoConverter.ConvertChat(line);
                    if (chatInfo != null)
                        chats.Add(chatInfo);

                    AnalyzePlayer(players, line);
                }

                RichLogDetailItems = new ObservableCollection<RichTextItem>(list);
                ChatInfos = new ObservableCollection<ChatInfo>(chats);

                var playerItemList = new List<PlayerItemInfo>();
                foreach (var playerItems in players)
                {
                    playerItemList.AddRange(playerItems.Value);
                }

                PlayerItemInfos = new ObservableCollection<PlayerItemInfo>(playerItemList.OrderBy(x => x.Date));

                _cache.Add(logFileInfo.FullPath, new LogCacheItem
                {
                    RichTextItems = RichLogDetailItems,
                    ChatItems = ChatInfos,
                    PlayerItems = PlayerItemInfos
                });
            });
        }

        public async Task ChangeFilter(string filter)
        {
            var fileInfo = _currentFileInfo;

            if (fileInfo == null)
                return;

            if (!_cache.ContainsKey(fileInfo.FullPath))
                return;

            if (string.IsNullOrEmpty(filter))
            {
                var cache = _cache[fileInfo.FullPath];
                RichLogDetailItems = cache.RichTextItems;
                return;
            }

            await Task.Factory.StartNew(() =>
            {
                var regex = new Regex(filter);

                var list = new List<RichTextItem>(LogFileList.Count);
                using var reader = _currentFileInfo.GetStringReader();
                while (reader.Peek() > -1)
                {
                    var line = reader.ReadLine();
                    if (line == null)
                        continue;

                    var match = regex.Match(line);

                    if (!match.Success)
                        continue;

                    var paragraph = AnalyzeLine(line);
                    list.Add(paragraph);
                }

                RichLogDetailItems = new ObservableCollection<RichTextItem>(list);
            });
        }

        private static void AnalyzePlayer(Dictionary<string, List<PlayerItemInfo>> playerDict, string line)
        {
            List<PlayerItemInfo> GetPlayerList(string id)
            {
                if (!playerDict.ContainsKey(id))
                {
                    var players = new List<PlayerItemInfo>();
                    playerDict.Add(id, players);
                    return players;
                }
                else
                {
                    var players = playerDict[id];
                    return players;
                }
            }

            var connectedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF Player connected, entityid=(?<id>[0-9]+), name=(?<name>.*), pltfmid=(?<steamId>[a-zA-Z0-9_]+), crossid=[a-zA-Z_0-9]+, steamOwner=[a-zA-Z_0-9]+, ip=(?<ip>[a-zA-Z0-9.:]+)$");
            var spawnedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF PlayerSpawnedInWorld \\(reason:(?<reason>.*), position: (?<position>[0-9, -]+)\\): EntityID=(?<id>[0-9-]+), PltfmId='[a-zA-Z0-9_]+', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)'$");
            var disconnectedRegex = new Regex("^(?<date>[0-9a-zA-Z-:]+) (?<tick>[0-9.]+) INF Player disconnected: EntityID=(?<id>[0-9]+), PltfmId='(?<steamId>[a-zA-Z0-9_]+)', CrossId='[a-zA-Z0-9_]+', OwnerID='[a-zA-Z0-9_]+', PlayerName='(?<name>.*)'$");

            var connectedMatch = connectedRegex.Match(line);
            var spawnedMatch = spawnedRegex.Match(line);
            var disconnectedMatch = disconnectedRegex.Match(line);

            if (connectedMatch.Success)
            {
                var id = connectedMatch.Groups["id"].Value;
                var players = GetPlayerList(id);
                
                players.Add(new PlayerItemInfo
                {
                    InOut = PlayerItemInfo.InOutType.In,
                    Date = connectedMatch.Groups["date"].Value,
                    Id = id,
                    Name = connectedMatch.Groups["name"].Value,
                    SteamId = connectedMatch.Groups["steamId"].Value,
                    IpAddress = connectedMatch.Groups["ip"].Value
                });
            }
            else if (disconnectedMatch.Success)
            {
                var id = disconnectedMatch.Groups["id"].Value;
                var players = GetPlayerList(id);

                players.Add(new PlayerItemInfo
                {
                    InOut = PlayerItemInfo.InOutType.Out,
                    Date = disconnectedMatch.Groups["date"].Value,
                    Id = id,
                    Name = disconnectedMatch.Groups["name"].Value,
                    SteamId = disconnectedMatch.Groups["steamId"].Value
                });
            }
            else if (spawnedMatch.Success)
            {
                var id = spawnedMatch.Groups["id"].Value;
                var players = GetPlayerList(id);
                var player = players.LastOrDefault(x => x.InOut == PlayerItemInfo.InOutType.In);

                if (player != null)
                {
                    player.Position = spawnedMatch.Groups["position"].Value;
                }
            }
        }

        private static RichTextItem AnalyzeLine(string line)
        {
            var item = AnalyzeComment(line);
            if (item != null)
                return item;

            var commonItem = AnalyzeCommonText(line);
            if (commonItem != null)
                return commonItem;

            var text = CreateTextItem(line);

            return text;
        }

        private static RichTextItem CreateTextItem(string line)
        {
            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = line
            });

            return paragraph;
        }

        private static RichTextItem AnalyzeCommonText(string line)
        {
            var expression = @"^(?<Time>[0-9]{4}-[0-9]{2}-[0-9]{2}T[0-9]{2}:[0-9]{2}:[0-9]{2} [0-9]+\.[0-9]+) (?<InfoType>[a-zA-Z]+) (?<Message>.*)$";
            var regex = new Regex(expression);
            var match = regex.Match(line);
            if (!match.Success)
                return null;

            var spaceItem = new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = " "
            };
            var time = match.Groups["Time"].Value;
            var infoType = match.Groups["InfoType"].Value;
            var message = match.Groups["Message"].Value;

            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(64, 155, 81),
                Text = time
            });
            paragraph.AddChildren(spaceItem);
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(69, 157, 255),
                Text = infoType
            });
            paragraph.AddChildren(spaceItem);
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Text = message
            });
            return paragraph;
        }

        private static RichTextItem AnalyzeComment(string line)
        {
            var expression = @"^\*\*\* (.*)$";
            var regex = new Regex(expression);
            var match = regex.Match(line);
            if (!match.Success)
                return null;

            var paragraph = new RichTextItem
            {
                TextType = RichTextType.Paragraph
            };
            paragraph.AddChildren(new RichTextItem
            {
                TextType = RichTextType.Text,
                Foreground = Color.FromRgb(64, 155, 81),
                Text = line
            });
            return paragraph;
        }
    }
}

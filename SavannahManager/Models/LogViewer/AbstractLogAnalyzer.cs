using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;
using SvManagerLibrary.Chat;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer;

public abstract class AbstractLogAnalyzer : ILogAnalyzer
{
    public List<RichTextItem> LogRichTextList { get; set; }
    public List<ChatInfo> ChatList { get; set; }
    public Dictionary<string, List<PlayerItemInfo>> PlayerInfos { get; set; }

    public void Analyze(LogFileInfo logFileInfo)
    {
        var list = new List<RichTextItem>();
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

            var chatInfo = AnalyzeChat(line);
            if (chatInfo != null)
                chats.Add(chatInfo);

            AnalyzePlayer(players, line);
        }

        LogRichTextList = list;
        ChatList = chats;
        PlayerInfos = players;
    }

    public List<RichTextItem> Analyze(LogFileInfo logFileInfo, string filter)
    {
        var regex = new Regex(filter);

        var list = new List<RichTextItem>();
        using var reader = logFileInfo.GetStringReader();
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

        return list;
    }

    protected abstract void AnalyzePlayer(Dictionary<string, List<PlayerItemInfo>> playerDict, string line);

    protected abstract ChatInfo AnalyzeChat(string line);

    protected virtual RichTextItem AnalyzeLine(string line)
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

    protected static List<PlayerItemInfo> GetPlayerList(Dictionary<string, List<PlayerItemInfo>> playerDict, string id)
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
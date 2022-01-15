using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Views.UserControls;

public enum RichTextType
{
    Text,
    Hyperlink,
    Paragraph,
    NoBreakLine,
    Space
}

public class RichTextItem
{
    public RichTextType TextType { get; set; } = RichTextType.Text;
    public string Text { get; set; }
    public string Link { get; set; }
    public Color Foreground { get; set; } = Colors.White;
    public FontWeight FontWeight { get; set; }
    public Thickness Margin { get; set; }

    public IEnumerable<RichTextItem> Children
    {
        get => _children;
        set => _children = new List<RichTextItem>(value);
    }

    private List<RichTextItem> _children = new List<RichTextItem>();

    public void AddChildren(RichTextItem item)
    {
        _children.Add(item);
    }

    public override string ToString()
    {
        var sb = new StringBuilder();
        foreach (var item in Children)
        {
            sb.Append(item.Text + " ");
        }
        return sb.ToString();
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using _7dtd_svmanager_fix_mvvm.Views.Update;
using _7dtd_svmanager_fix_mvvm.Views.UserControls;

namespace _7dtd_svmanager_fix_mvvm.Views.Converter;

public class RichTextItemsToDocumentConverter : MarkupExtension, IValueConverter
{
    public object Convert(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        var doc = new FlowDocument();

        var items = value as ICollection<RichTextItem>;
        InsertParagraph(doc.Blocks, null, items);

        return doc;
    }

    public void InsertParagraph(BlockCollection block, InlineCollection inlineCollection, IEnumerable<RichTextItem> items)
    {
        var isNoBreakLine = false;
        foreach (var item in items)
        {
            if (item == null)
                continue;

            if (item.TextType == RichTextType.NoBreakLine)
            {
                isNoBreakLine = true;
            }
            else if (item.TextType == RichTextType.Space)
            {
                var paragraph = block.LastOrDefault() as Paragraph;
                paragraph?.Inlines.Add(new Run(" "));
            }
            else if (item.TextType == RichTextType.Text)
            {
                inlineCollection.Add(new Run(item.Text)
                {
                    Foreground = new SolidColorBrush(item.Foreground)
                });
            }
            else if (item.TextType == RichTextType.Hyperlink)
            {
                var hyperlink = new Hyperlink()
                {
                    NavigateUri = new Uri(item.Link)
                };
                InsertParagraph(block, hyperlink.Inlines, item.Children);
                inlineCollection.Add(hyperlink);
            }
            else
            {
                var paragraph = new Paragraph()
                {
                    Margin = new Thickness(0, 3, 0, 3)
                };

                if (isNoBreakLine)
                {
                    paragraph = block.LastOrDefault() as Paragraph;
                    isNoBreakLine = false;
                }

                InsertParagraph(block, paragraph.Inlines, item.Children);
                block.Add(paragraph);
            }
        }
    }

    public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
    {
        throw new System.NotImplementedException();
    }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
        return this;
    }
}
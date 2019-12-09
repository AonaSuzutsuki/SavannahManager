using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Update.Views
{
    public class RichTextItemsToDocumentConverter : IValueConverter
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
            foreach (var item in items)
            {
                if (item.TextType == RichTextType.Text)
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
                        Margin = new Thickness(0,3,0,3)
                    };
                    InsertParagraph(block, paragraph.Inlines, item.Children);
                    block.Add(paragraph);
                }
            }
        }

        public object ConvertBack(object value, System.Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new System.NotImplementedException();
        }
    }

    public enum RichTextType
    {
        Text,
        Hyperlink,
        Paragraph
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
            get => children;
            set => children = new List<RichTextItem>(value);
        }

        private List<RichTextItem> children = new List<RichTextItem>();

        public void AddChildren(RichTextItem item)
        {
            children.Add(item);
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

    public class BindableRichTextBox : RichTextBox
    {
        #region 依存関係プロパティ
        public static readonly DependencyProperty DocumentProperty = DependencyProperty.Register("Document", typeof(FlowDocument),
            typeof(BindableRichTextBox), new UIPropertyMetadata(null, OnRichTextItemsChanged));
        #endregion  // 依存関係プロパティ

        #region 公開プロパティ
        public new FlowDocument Document
        {
            get => (FlowDocument)GetValue(DocumentProperty);
            set => SetValue(DocumentProperty, value);
        }
        #endregion  // 公開プロパティ

        #region イベントハンドラ
        private static void OnRichTextItemsChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is RichTextBox control)
            {
                control.Document = e.NewValue as FlowDocument;
            }
        }
        #endregion  // イベントハンドラ
    }
}

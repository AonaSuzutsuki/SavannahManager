using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace _7dtd_svmanager_fix_mvvm.Views.UserControls
{
    public class BindableRichTextBox : RichTextBox
    {
        #region Fields

        private readonly float _dpiX;
        
        private List<Block> _blocks;

        #endregion

        #region Properties

        public int SplitLine { get; set; } = 128;
        public bool AutoSplitLine { get; set; } = true;
        public bool IsLazyLoad { get; set; } = false;

        public int Lines => _blocks?.Count ?? 0;
        public int CurrentLines { get; private set; } = 0;

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty BindingDocumentProperty = DependencyProperty.Register("BindingDocument", typeof(FlowDocument),
            typeof(BindableRichTextBox), new UIPropertyMetadata(null, BindingDocumentChanged));

        public static readonly DependencyProperty WordWrappingProperty = DependencyProperty.Register("WordWrapping", typeof(bool),
            typeof(BindableRichTextBox), new UIPropertyMetadata(true, WordWrappingChanged));

        public static readonly DependencyProperty TextChangedCommandProperty = DependencyProperty.Register("TextChangedCommand", typeof(ICommand),
            typeof(BindableRichTextBox), new UIPropertyMetadata(null));

        public static readonly DependencyProperty ScrollEndedCommandProperty = DependencyProperty.Register("ScrollEndedCommand", typeof(ICommand),
            typeof(BindableRichTextBox), new UIPropertyMetadata(null));

        #endregion

        #region Properties

        public FlowDocument BindingDocument
        {
            get => (FlowDocument)GetValue(BindingDocumentProperty);
            set => SetValue(BindingDocumentProperty, value);
        }

        public bool WordWrapping
        {
            get => (bool)GetValue(WordWrappingProperty);
            set => SetValue(WordWrappingProperty, value);
        }

        public ICommand TextChangedCommand
        {
            get => (ICommand)GetValue(TextChangedCommandProperty);
            set => SetValue(TextChangedCommandProperty, value);
        }

        public ICommand ScrollEndedCommand
        {
            get => (ICommand)GetValue(ScrollEndedCommandProperty);
            set => SetValue(ScrollEndedCommandProperty, value);
        }

        #endregion

        #region Event Methods

        /// <summary>
        /// A callback function that is executed when the value of the BindingDocument is changed.
        /// </summary>
        /// <param name="sender">The object that issued the event.</param>
        /// <param name="e">Event parameter.</param>
        private static void BindingDocumentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is not BindableRichTextBox control || e.NewValue is not FlowDocument flowDocument)
                return;

            if (control.IsLazyLoad)
            {
                var blocks = new List<Block>(flowDocument.Blocks.Count);
                blocks.AddRange(flowDocument.Blocks);

                control._blocks = blocks;

                var sliceDocument = new FlowDocument();
                var takeBlocks = blocks.Take(control.SplitLine).ToList();
                sliceDocument.Blocks.AddRange(takeBlocks);

                control.CurrentLines = takeBlocks.Count;
                control.Document = sliceDocument;
            }
            else
            {
                control.Document = flowDocument;
            }
        }

        /// <summary>
        /// A callback function that is executed when the value of the WordWrapping is changed.
        /// </summary>
        /// <param name="sender">The object that issued the event</param>
        /// <param name="e">Event parameter.</param>
        private static void WordWrappingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is BindableRichTextBox bindableRichTextBox)
            {
                ResizeDocument(bindableRichTextBox);
            }
        }

        #endregion

        /// <summary>
        /// Resizes the width of BindableRichTextBox according to the value of WordWrapping.
        /// </summary>
        /// <param name="control">BindableRichTextBox object</param>
        private static void ResizeDocument(BindableRichTextBox control)
        {
            if (control.WordWrapping)
            {
                control.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;

                control.Document.PageWidth = double.NaN;
            }
            else
            {
                control.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;

                var text = new TextRange(control.Document.ContentStart, control.Document.ContentEnd).Text;
                var size = MeasureString(control, text);
                control.Document.PageWidth = size.Width + 15;
            }
        }

        /// <summary>
        /// Calculate text size on RichTextBox.
        /// </summary>
        /// <param name="control">RichTextBox object</param>
        /// <returns>Size of text on RichTextBox.</returns>
        private static System.Windows.Size MeasureString(BindableRichTextBox control, string text)
        {
            var formattedText = new FormattedText(text,
                CultureInfo.CurrentCulture,
                FlowDirection.LeftToRight,
                new Typeface(control.FontFamily, control.FontStyle, control.FontWeight, control.FontStretch),
                control.FontSize,
                System.Windows.Media.Brushes.Black,
                control._dpiX);

            return new System.Windows.Size(formattedText.Width, formattedText.Height);
        }

        private void SetSplitSize()
        {
            if (!AutoSplitLine)
                return;

            var lineSize = MeasureString(this, "text");
            var splitLine = (int)(ActualHeight / lineSize.Height) * 4;
            SplitLine = splitLine;
        }

        public BindableRichTextBox()
        {
            using var graphics = Graphics.FromHwnd(IntPtr.Zero);
            _dpiX = graphics.DpiX;

            Loaded += (sender, args) =>
            {
                if (Template.FindName("PART_ContentHost", this) is not ScrollViewer scrollViewer)
                    return;

                SetSplitSize();
                
                var prevVerticalOffset = .0;
                var prevScrollableHeight = .0;
                scrollViewer.ScrollChanged += (_, _) =>
                {
                    if (prevVerticalOffset.Equals(scrollViewer.VerticalOffset) && prevScrollableHeight.Equals(scrollViewer.ScrollableHeight))
                        return;

                    prevVerticalOffset = scrollViewer.VerticalOffset;
                    prevScrollableHeight = scrollViewer.ScrollableHeight;

                    var isEnded = scrollViewer.VerticalOffset.Equals(scrollViewer.ScrollableHeight);
                    if (!isEnded)
                        return;

                    ScrollEndedCommand?.Execute(this);
                };
            };

            TextChanged += (_, _) =>
            {
                ResizeDocument(this);
                TextChangedCommand?.Execute(this);
            };
            SizeChanged += (sender, args) =>
            {
                SetSplitSize();
            };

        }

        public void ShowNext()
        {
            if (!IsLazyLoad)
                return;

            var takeBlocks = _blocks.Skip(CurrentLines).Take(SplitLine).ToList();
            Document.Blocks.AddRange(takeBlocks);
            CurrentLines += takeBlocks.Count;
        }
    }
}

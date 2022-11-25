using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonStyleLib.Models;
using CommonStyleLib.Views.Behaviors.ListBoxBehavior;

namespace _7dtd_svmanager_fix_mvvm.Models.LogViewer
{
    public class LogExportModel : ModelBase
    {
        private readonly List<Dictionary<string, string>> _logList;

        private ObservableCollection<MoveableListBoxItem> _columnItems;
        private string _previewText;

        public string Mode { get;  private set; }

        public ObservableCollection<MoveableListBoxItem> ColumnItems
        {
            get => _columnItems;
            set => SetProperty(ref _columnItems, value);
        }

        public string PreviewText
        {
            get => _previewText;
            set => SetProperty(ref _previewText, value);
        }

        public LogExportModel(IEnumerable<string> names, List<Dictionary<string, string>> map, string mode)
        {
            ColumnItems = new ObservableCollection<MoveableListBoxItem>(names.Select(x => new MoveableListBoxItem
            {
                Text = x
            }));

            _logList = map;

            DisplayPreview();

            Mode = mode;
        }

        public void DisplayPreview()
        {
            PreviewText = CreateText();
        }

        public void SaveFile(string path)
        {
            if (string.IsNullOrEmpty(path))
                return;

            File.WriteAllText(path, CreateText());
        }

        private string CreateText()
        {

            var sb = new StringBuilder();
            sb.AppendLine(string.Join("\t", ColumnItems.Select(x => x.Text)));
            foreach (var dict in _logList)
            {
                foreach (var columnItem in ColumnItems)
                {
                    sb.Append($"{dict.GetValueOrDefault(columnItem.Text)}\t");
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}

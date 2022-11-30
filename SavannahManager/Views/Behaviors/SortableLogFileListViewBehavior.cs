using SavannahManagerStyleLib.Views.Behaviors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using _7dtd_svmanager_fix_mvvm.Models.Comparer;
using _7dtd_svmanager_fix_mvvm.ViewModels.LogViewer;
using Newtonsoft.Json.Linq;

namespace _7dtd_svmanager_fix_mvvm.Views.Behaviors
{
    public class SortableLogFileListViewBehavior : SortableListViewBehavior
    {
        private readonly IComparer _descFileSizeComparer = new FileSizeDescComparer();
        private readonly IComparer _ascFileSizeComparer = new FileSizeAscComparer();

        protected override void GridViewColumnHeaderSort(ListView listView, OrderType order, string headerName, Action<string> setContentSuffixAction)
        {
            if (headerName == "FileSizeText")
            {
                var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                collectionView.SortDescriptions.Clear();

                if (order == OrderType.Asc)
                {
                    collectionView.CustomSort = _ascFileSizeComparer;
                    setContentSuffixAction?.Invoke("↑");
                }
                else
                {
                    collectionView.CustomSort = _descFileSizeComparer;
                    setContentSuffixAction?.Invoke("↓");
                }
                collectionView.Refresh();
            }
            else
            {
                base.GridViewColumnHeaderSort(listView, order, headerName, setContentSuffixAction);
            }
        }

        protected override void GridViewColumnHeaderSort(ListView listView, string headerName, Action<string> setContentSuffixAction)
        {
            if (headerName == "FileSizeText")
            {
                var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);

                collectionView.SortDescriptions.Clear();

                if (collectionView.CustomSort is FileSizeAscComparer)
                {
                    collectionView.CustomSort = _descFileSizeComparer;
                    setContentSuffixAction?.Invoke("↓");
                }
                else
                {
                    collectionView.CustomSort = _ascFileSizeComparer;
                    setContentSuffixAction?.Invoke("↑");
                }
                collectionView.Refresh();
            }
            else
            {
                base.GridViewColumnHeaderSort(listView, headerName, setContentSuffixAction);
            }
        }
    }
}

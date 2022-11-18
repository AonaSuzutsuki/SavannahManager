using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Xaml.Behaviors;

namespace SavannahManagerStyleLib.Views.Behaviors
{
    public class SortableListViewBehavior : Behavior<ListView>
    {

        #region Fields

        private readonly Dictionary<GridViewColumnHeader, string> _headers =
            new Dictionary<GridViewColumnHeader, string>();

        private bool _isInitialized = false;

        #endregion

        #region Dependency Properties

        public string FirstSort
        {
            get => (string)GetValue(FirstSortProperty);
            set => SetValue(FirstSortProperty, value);
        }

        public static readonly DependencyProperty FirstSortProperty = DependencyProperty.Register(
            nameof(FirstSort),
            typeof(string),
            typeof(SortableListViewBehavior),
            new UIPropertyMetadata(null));

        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();

            AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();

            if (!(AssociatedObject.View is GridView gridView))
                return;

            AssociatedObject.Loaded -= AssociatedObjectOnLoaded;
            foreach (var gridViewColumn in gridView.Columns)
            {
                var header = (GridViewColumnHeader)gridViewColumn.Header;
                header.Click -= HeaderOnClick;
            }
        }

        private void HeaderOnClick(object sender, RoutedEventArgs e)
        {
            if (!(e.OriginalSource is GridViewColumnHeader header))
                return;

            // 全てのヘッダーを初期表示に戻す
            foreach (var h in _headers)
            {
                h.Key.Content = h.Value;
            }

            var content = header.Content.ToString();

            GridViewColumnHeaderSort(AssociatedObject, header.Tag.ToString(),
                arg => header.Content = $"{content} {arg}");
        }

        private void AssociatedObjectOnLoaded(object sender, RoutedEventArgs e)
        {
            if (_isInitialized)
                return;

            if (!(AssociatedObject.View is GridView gridView))
                return;

            // GridViewColumnHeaderのClickイベントにメソッドを登録
            // 同時にヘッダーの初期表示を記録
            foreach (var gridViewColumn in gridView.Columns)
            {
                if (!(gridViewColumn.Header is GridViewColumnHeader header) || _headers.ContainsKey(header))
                    continue;

                _headers.Add(header, header.Content.ToString());
                header.Click += (_, args) => HeaderOnClick(AssociatedObject, args);
            }

            // 初期表示時にソートするカラムがなければソートしない
            if (string.IsNullOrEmpty(FirstSort))
                return;

            var firstHeader = _headers.Keys.First();
            var content = firstHeader.Content.ToString();
            GridViewColumnHeaderSort(AssociatedObject, FirstSort,
                arg => firstHeader.Content = $"{content} {arg}");

            _isInitialized = true;
        }


        private static void GridViewColumnHeaderSort(ListView listView, string headerName, Action<string> setContentSuffixAction)
        {
            // ListView#ItemsSourceからCollectionViewを取得
            var collectionView = CollectionViewSource.GetDefaultView(listView.ItemsSource);

            // CollectionViewからSortDescriptionsの最初の要素を取得
            var sortDescription = collectionView.SortDescriptions.FirstOrDefault();
            // ソート方法をクリア
            collectionView.SortDescriptions.Clear();
            if (sortDescription.PropertyName == headerName)
            {
                // SortDescriptionsのヘッダ名とクリックしたヘッダ名が同一なら降順か昇順かをチェンジする
                if (sortDescription.Direction == ListSortDirection.Ascending)
                {
                    collectionView.SortDescriptions.Add(new SortDescription(headerName, ListSortDirection.Descending));
                    setContentSuffixAction?.Invoke("↓");
                }
                else
                {
                    collectionView.SortDescriptions.Add(new SortDescription(headerName, ListSortDirection.Ascending));
                    setContentSuffixAction?.Invoke("↑");
                }
            }
            else
            {
                // 昇順でソートする
                collectionView.SortDescriptions.Add(new SortDescription(headerName, ListSortDirection.Ascending));
                setContentSuffixAction?.Invoke("↑");
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using SavannahManagerStyleLib.Attributes;

namespace SavannahManagerStyleLib.Views.Behaviors
{
    public class SortableAnnotationListViewBehavior : SortableListViewBehavior
    {
        private string _previousHeaderName = string.Empty;
        private OrderType _previousOrderType = OrderType.Asc;
        private readonly Dictionary<Type, IComparer> _compareCache = new();

        protected override void OnAttached()
        {
            var itemsObj = ((INotifyCollectionChanged)AssociatedObject.Items);
            if (AssociatedObject.Items.Count < 1)
                itemsObj.CollectionChanged += ItemsObjOnCollectionChanged;
            else
                AssociatedObject.Loaded += AssociatedObjectOnLoaded;
        }

        private void ItemsObjOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Add)
                return;

            if (IsInitialized)
                return;

            AssociatedObjectOnLoaded(AssociatedObject, e);

            var itemsObj = ((INotifyCollectionChanged)AssociatedObject.Items);
            itemsObj.CollectionChanged -= ItemsObjOnCollectionChanged;
        }

        protected override void GridViewColumnHeaderSort(ListView listView, OrderType order, string headerName, Action<string> setContentSuffixAction)
        {
            var sortAttr = GetSortingAttribute(listView, headerName);
            _previousHeaderName = headerName;

            var result = CheckProc(sortAttr, () => base.GridViewColumnHeaderSort(listView, order, headerName, setContentSuffixAction));

            if (result == null)
                return;

            var ascComparerType = result.Value.ascComparerType;
            var descComparerType = result.Value.descComparerType;

            var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            collectionView.SortDescriptions.Clear();

            if (order == OrderType.Asc)
            {
                collectionView.CustomSort = GetComparer(ascComparerType);
                setContentSuffixAction?.Invoke("↑");

                _previousOrderType = OrderType.Asc;
            }
            else
            {
                collectionView.CustomSort = GetComparer(descComparerType);
                setContentSuffixAction?.Invoke("↓");

                _previousOrderType = OrderType.Desc;
            }
        }

        protected override void GridViewColumnHeaderSort(ListView listView, string headerName, Action<string> setContentSuffixAction)
        {
            var sortAttr = GetSortingAttribute(listView, headerName);
            _previousHeaderName = headerName;

            var result = CheckProc(sortAttr, () => base.GridViewColumnHeaderSort(listView, headerName, setContentSuffixAction));

            if (result == null)
                return;

            var ascComparerType = result.Value.ascComparerType;
            var descComparerType = result.Value.descComparerType;

            var collectionView = (ListCollectionView)CollectionViewSource.GetDefaultView(listView.ItemsSource);
            collectionView.SortDescriptions.Clear();

            if (_previousHeaderName == headerName)
            {
                if (_previousOrderType == OrderType.Asc)
                {
                    collectionView.CustomSort = GetComparer(descComparerType);
                    setContentSuffixAction?.Invoke("↓");

                    _previousOrderType = OrderType.Desc;
                }
                else
                {
                    collectionView.CustomSort = GetComparer(ascComparerType);
                    setContentSuffixAction?.Invoke("↑");

                    _previousOrderType = OrderType.Asc;
                }
            }
            else
            {
                collectionView.CustomSort = GetComparer(ascComparerType);
                setContentSuffixAction?.Invoke("↑");

                _previousOrderType = OrderType.Asc;
            }
        }

        private IComparer GetComparer(Type type)
        {
            if (_compareCache.ContainsKey(type))
                return _compareCache[type];

            var comparer = Activator.CreateInstance(type) as IComparer;
            _compareCache.Add(type, comparer);

            return comparer;
        }

        private static (Type ascComparerType, Type descComparerType)? CheckProc(SortingAttribute? sortAttr, Action falseCallback)
        {
            if (sortAttr == null)
            {
                falseCallback.Invoke();
                return null;
            }

            var ascComparerType = sortAttr.AscComparer;
            var descComparerType = sortAttr.DescComparer;

            if (ascComparerType == null || descComparerType == null)
            {
                falseCallback.Invoke();
                return null;
            }

            return (ascComparerType, descComparerType);
        }

        private static SortingAttribute GetSortingAttribute(ListView listView, string headerName)
        {
            if (listView.Items.Count <= 0)
                return null;

            var first = listView.Items[0];
            var type = first.GetType();
            var targetInfo = type.GetProperties().FirstOrDefault(x => x.Name == headerName);
            if (targetInfo == null)
                return null;

            var attrs = Attribute.GetCustomAttributes(targetInfo, typeof(SortingAttribute));
            var attr = attrs.FirstOrDefault();
            
            if (attr is not SortingAttribute sortAttr)
            {
                return null;
            }

            return sortAttr;
        }
    }
}

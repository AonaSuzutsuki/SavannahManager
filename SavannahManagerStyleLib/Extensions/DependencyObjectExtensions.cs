using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace SavannahManagerStyleLib.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T GetParent<T>(this DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            return parent switch
            {
                null => default,
                T ret => ret,
                _ => parent.GetParent<T>()
            };
        }

        public static T GetParentNonVisualTreeHelper<T>(this DependencyObject obj)
        {
            if (obj is not FrameworkElement element)
                return default;

            var parent = element.Parent;
            return parent switch
            {
                null => default,
                T ret => ret,
                _ => parent.GetParentNonVisualTreeHelper<T>()
            };
        }

        //--- 子要素を取得
        public static IEnumerable<DependencyObject> Children(this DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var count = VisualTreeHelper.GetChildrenCount(obj);
            if (count == 0)
                yield break;

            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                yield return child;
            }
        }

        //--- 子孫要素を取得
        public static IEnumerable<DependencyObject> Descendants(this DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            foreach (var child in obj.Children())
            {
                yield return child;
                foreach (var grandChild in child.Descendants())
                    yield return grandChild;
            }
        }

        //--- 特定の型の子要素を取得
        public static IEnumerable<T> Children<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Children().OfType<T>();
        }

        //--- 特定の型の子孫要素を取得
        public static IEnumerable<T> Descendants<T>(this DependencyObject obj)
            where T : DependencyObject
        {
            return obj.Descendants().OfType<T>();
        }
    }
}

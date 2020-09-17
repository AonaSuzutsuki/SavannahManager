using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace _7dtd_XmlEditor.Extensions
{
    public static class DependencyObjectExtensions
    {
        public static T Parent<T>(this DependencyObject obj)
        {
            var parent = VisualTreeHelper.GetParent(obj);
            switch (parent)
            {
                case null:
                    return default;
                case T ret:
                    return ret;
                default:
                    return parent.Parent<T>();
            }
        }

        //--- 子要素を取得
        public static IEnumerable<DependencyObject> Children(this DependencyObject obj)
        {
            if (obj == null)
                throw new ArgumentNullException("obj");

            var count = VisualTreeHelper.GetChildrenCount(obj);
            if (count == 0)
                yield break;

            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null)
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

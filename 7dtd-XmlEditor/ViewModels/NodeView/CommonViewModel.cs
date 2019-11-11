using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using _7dtd_XmlEditor.Models.NodeView;
using CommonStyleLib.ViewModels;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.ViewModels.NodeView
{
    public class CommonViewModel
    {
        public CommonViewModel(CommonModel model)
        {
            FullPath = model.ObserveProperty(m => m.FullPath).ToReactiveProperty();
            Attributes = model.Attributes.ToReadOnlyReactiveCollection(m => m);
            InnerXml = model.ObserveProperty(m => m.InnerXml).ToReactiveProperty();
        }

        public ReactiveProperty<string> FullPath { get; set; }
        public ReadOnlyReactiveCollection<AttributeInfo> Attributes { get; set; }
        public ReactiveProperty<string> InnerXml { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using _7dtd_XmlEditor.Models.NodeView;
using CommonStyleLib.ViewModels;
using Prism.Commands;
using Reactive.Bindings;
using Reactive.Bindings.Extensions;
using SvManagerLibrary.XmlWrapper;

namespace _7dtd_XmlEditor.ViewModels.NodeView
{
    public class CommonViewModel
    {
        public CommonViewModel(CommonModel model)
        {
            this.model = model;

            FullPath = model.ObserveProperty(m => m.FullPath).ToReactiveProperty();
            Attributes = model.Attributes.ToReadOnlyReactiveCollection(m => m);
            InnerXml = model.ToReactivePropertyAsSynchronized(m => m.InnerXml);

            InnerXmlTextChanged = new DelegateCommand(InnerXml_TextChanged);
        }

        private CommonModel model;

        public ReactiveProperty<string> FullPath { get; set; }
        public ReadOnlyReactiveCollection<AttributeInfo> Attributes { get; set; }
        public ReactiveProperty<string> InnerXml { get; set; }

        public ICommand InnerXmlTextChanged { get; set; }

        public void InnerXml_TextChanged()
        {
            model.ChangeInnerXml();
        }
    }
}

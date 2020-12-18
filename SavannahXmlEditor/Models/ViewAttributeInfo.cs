using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Prism.Commands;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_XmlEditor.Models
{
    public class ViewAttributeInfo
    {
        public ViewAttributeInfo()
        {
            AttributeNameLostFocus = new DelegateCommand(AttributeName_LostFocus);
            AttributeNameTextChanged = new DelegateCommand(AttributeName_TextChanged);
        }

        public bool IsEdited { get; private set; }
        public AttributeInfo Attribute { get; set; } = new AttributeInfo();
        public Action<ViewAttributeInfo> LostFocusAction { get; set; }

        public ICommand AttributeNameLostFocus { get; set; }
        public ICommand AttributeNameTextChanged { get; set; }

        public void AttributeName_LostFocus()
        {
            LostFocusAction?.Invoke(this);
            IsEdited = false;
        }

        public void AttributeName_TextChanged()
        {
            IsEdited = true;
        }
    }
}

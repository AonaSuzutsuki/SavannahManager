using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _7dtd_XmlEditor.Models.NodeView;
using _7dtd_XmlEditor.Models.TreeView;

namespace _7dtd_XmlEditor.Views.NodeView
{
    public interface INodeView
    {
        ICommonModel Model { get; }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using CommonStyleLib.Models;
using Prism.Commands;

namespace _7dtd_svmanager_fix_mvvm.Settings.Models
{
    public class PermissionInfo
    {
        public enum PermissionItemType
        {
            Dummy,
            Real
        }

        public PermissionItemType ItemType = PermissionItemType.Real;

        public string Name { get; set; }
        public int? Permission { get; set; } = 0;


        public Action AddDummyAction { get; set; }

        public ICommand TextChangedCommand { get; set; }

        public PermissionInfo()
        {
            TextChangedCommand = new DelegateCommand(() =>
            {
                if (ItemType == PermissionItemType.Dummy)
                {
                    ItemType = PermissionItemType.Real;
                    AddDummyAction();
                }
            });
        }
    }

    public class PermissionEditorModel : ModelBase
    {
        private ObservableCollection<PermissionInfo> commandPermissions;

        public ObservableCollection<PermissionInfo> CommandPermissions
        {
            get => commandPermissions;
            set => SetProperty(ref commandPermissions, value);
        }

        public PermissionEditorModel()
        {
            CommandPermissions = new ObservableCollection<PermissionInfo>
            {
                new PermissionInfo
                {
                    Name = "test",
                    Permission = 1000
                },
                new PermissionInfo
                {
                    Name = "test2",
                    Permission = 2000
                }
            };
            AssertTextChangedCommand(CommandPermissions);
            AddDummyItem(CommandPermissions);
        }

        public void AssertTextChangedCommand(ICollection<PermissionInfo> collection)
        {
            foreach (var permissionInfo in collection)
            {
                permissionInfo.AddDummyAction = () => AddDummyItem(collection);
            }
        }

        public void AddDummyItem(ICollection<PermissionInfo> collection)
        {
            collection.Add(new PermissionInfo
            {
                ItemType = PermissionInfo.PermissionItemType.Dummy,
                Permission = null,
                AddDummyAction = () => AddDummyItem(collection)
            });
        }
    }
}

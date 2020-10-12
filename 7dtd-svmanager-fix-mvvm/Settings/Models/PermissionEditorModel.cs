using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;
using CommonExtensionLib.Extensions;
using CommonStyleLib.Models;
using Prism.Commands;
using SavannahXmlLib.XmlWrapper;

namespace _7dtd_svmanager_fix_mvvm.Settings.Models
{
    public abstract class PermissionBase
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

        protected PermissionBase()
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
    public class PermissionInfo : PermissionBase
    {

    }

    public class AdminPermissionInfo : PermissionBase
    {
        public string SteamId { get; set; }
        public int? DefaultPermission { get; set; }
        public int? ModeratorPermission { get; set; }
    }

    public class BlackListPermissionInfo : PermissionBase
    {
        public string SteamId { get; set; }
        public string UnBanDate { get; set; }
        public string Reason { get; set; }
    }

    public class PermissionEditorModel : ModelBase
    {
        #region Fields

        private ObservableCollection<PermissionInfo> commandPermissions = new ObservableCollection<PermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> adminPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> adminGroupPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> whitelistPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> whitelistGroupPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<BlackListPermissionInfo> blacklistPermissions = new ObservableCollection<BlackListPermissionInfo>();

        #endregion

        #region Properties

        public ObservableCollection<PermissionInfo> CommandPermissions
        {
            get => commandPermissions;
            set => SetProperty(ref commandPermissions, value);
        }
        public ObservableCollection<AdminPermissionInfo> AdminPermissions
        {
            get => adminPermissions;
            set => SetProperty(ref adminPermissions, value);
        }

        public ObservableCollection<AdminPermissionInfo> AdminGroupPermissions
        {
            get => adminGroupPermissions;
            set => SetProperty(ref adminGroupPermissions, value);
        }
        public ObservableCollection<AdminPermissionInfo> WhitelistPermissions
        {
            get => whitelistPermissions;
            set => SetProperty(ref whitelistPermissions, value);
        }

        public ObservableCollection<AdminPermissionInfo> WhitelistGroupPermissions
        {
            get => whitelistGroupPermissions;
            set => SetProperty(ref whitelistGroupPermissions, value);
        }
        public ObservableCollection<BlackListPermissionInfo> BlacklistPermissions
        {
            get => blacklistPermissions;
            set => SetProperty(ref blacklistPermissions, value);
        }

        #endregion

        public PermissionEditorModel()
        {
            var path = @"C:\Users\rvv\AppData\Roaming\7DaysToDie\Saves\serveradmin.xml";
            var reader = new SavannahXmlReader(path);
            var cmdPermissions = LoadCommand(reader);
            var (adminPlayers, adminGroups) = LoadAdmin(reader);
            var (whitelistPlayers, whitelistGroups) = LoadWhitelist(reader);
            var blacklistPlayers = LoadBlacklist(reader);

            CreatePermissions(CommandPermissions, cmdPermissions, info => info.Permission = null);
            CreatePermissions(AdminPermissions, adminPlayers, info => info.Permission = null);
            CreatePermissions(AdminGroupPermissions, adminGroups, info =>
            {
                info.DefaultPermission = null;
                info.ModeratorPermission = null;
            });
            CreatePermissions(WhitelistPermissions, whitelistPlayers, null);
            CreatePermissions(WhitelistGroupPermissions, whitelistGroups, null);
            CreatePermissions(BlacklistPermissions, blacklistPlayers, null);
        }

        private static IEnumerable<PermissionInfo> LoadCommand(SavannahXmlReader reader)
        {
            var nodes = reader.GetNode("/adminTools/permissions")?.ChildNodes;
            var permissions = from node in nodes
                let cmd = node.GetAttribute("cmd")
                let permission = node.GetAttribute("permission_level")
                where cmd != null && permission != null
                select new PermissionInfo
                {
                    Name = cmd.Value,
                    Permission = permission.Value.ToInt()
                };

            return permissions;
        }

        private static (IEnumerable<AdminPermissionInfo>, IEnumerable<AdminPermissionInfo>) LoadAdmin(SavannahXmlReader reader)
        {
            var admin = reader.GetNode("/adminTools/admins");
            if (admin == null)
                return (new AdminPermissionInfo[0], new AdminPermissionInfo[0]);

            var groups = new List<AdminPermissionInfo>();
            var players = new List<AdminPermissionInfo>();
            var children = admin.ChildNodes;
            foreach (var node in children)
            {
                var info = new AdminPermissionInfo
                {
                    SteamId = node.GetAttribute("steamID")?.Value,
                    Name = node.GetAttribute("name")?.Value
                };

                if (node.TagName == "user")
                {
                    info.Permission = node.GetAttribute("permission_level")?.Value.ToInt();
                    players.Add(info);
                }
                else
                {
                    info.DefaultPermission = node.GetAttribute("permission_level_default")?.Value.ToInt();
                    info.ModeratorPermission = node.GetAttribute("permission_level_mod")?.Value.ToInt();
                    groups.Add(info);
                }
            }

            return (players, groups);
        }

        private static (IEnumerable<AdminPermissionInfo>, IEnumerable<AdminPermissionInfo>) LoadWhitelist(SavannahXmlReader reader)
        {
            var admin = reader.GetNode("/adminTools/whitelist");
            if (admin == null)
                return (new AdminPermissionInfo[0], new AdminPermissionInfo[0]);

            var groups = new List<AdminPermissionInfo>();
            var players = new List<AdminPermissionInfo>();
            var children = admin.ChildNodes;
            foreach (var node in children)
            {
                var info = new AdminPermissionInfo
                {
                    SteamId = node.GetAttribute("steamID")?.Value,
                    Name = node.GetAttribute("name")?.Value
                };

                if (node.TagName == "user")
                    players.Add(info);
                else
                    groups.Add(info);
            }

            return (players, groups);
        }

        private static IEnumerable<BlackListPermissionInfo> LoadBlacklist(SavannahXmlReader reader)
        {
            var nodes = reader.GetNode("/adminTools/blacklist")?.ChildNodes;
            var permissions = from node in nodes
                let steamId = node.GetAttribute("steamID")
                let unBanDate = node.GetAttribute("unbandate")
                let reason = node.GetAttribute("reason")
                where steamId != null && unBanDate != null && reason != null
                select new BlackListPermissionInfo
                {
                    SteamId = steamId.Value,
                    UnBanDate = unBanDate.Value,
                    Reason = reason.Value
                };

            return permissions;
        }

        private static void CreatePermissions<T>(ICollection<T> collection, IEnumerable<T> enumerable, Action<T> initializer) where T : PermissionBase, new()
        {
            foreach (var permissionInfo in enumerable)
            {
                collection.Add(permissionInfo);
            }
            AssertTextChangedCommand(collection, initializer);
            AddDummyItem(collection, initializer);
        }

        private static void AssertTextChangedCommand<T>(ICollection<T> collection, Action<T> initializer) where T : PermissionBase, new()
        {
            foreach (var permissionInfo in collection)
            {
                permissionInfo.AddDummyAction = () => AddDummyItem(collection, initializer);
            }
        }

        private static void AddDummyItem<T>(ICollection<T> collection, Action<T> initializer) where T : PermissionBase, new()
        {
            var info = new T
            {
                ItemType = PermissionBase.PermissionItemType.Dummy,
                AddDummyAction = () => AddDummyItem(collection, initializer)
            };
            initializer?.Invoke(info);
            collection.Add(info);
        }
    }
}

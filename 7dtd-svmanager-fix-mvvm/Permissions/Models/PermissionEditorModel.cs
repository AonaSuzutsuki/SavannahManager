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
using Prism.Mvvm;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_svmanager_fix_mvvm.Permissions.Models
{
    public abstract class PermissionBase : BindableBase
    {

        public enum PermissionItemType
        {
            Dummy,
            Real
        }

        private string steamId;
        private string name;
        private string permission;

        public PermissionItemType ItemType { get; set; } = PermissionItemType.Real;

        public string Name
        {
            get => name;
            set => SetProperty(ref name, value);
        }

        public string SteamId
        {
            get => steamId;
            set => SetProperty(ref steamId, value);
        }

        public string Permission
        {
            get => permission;
            set => SetProperty(ref permission, value);
        }

        public Action AddDummyAction { get; set; }
        public Action RemoveAction { get; set; }

        public abstract void ApplyNode(SavannahTagNode parent);

        public virtual bool CanRemove()
        {
            if (ItemType == PermissionItemType.Dummy)
                return false;

            if (!string.IsNullOrEmpty(Name))
                return false;
            if (!string.IsNullOrEmpty(steamId))
                return false;
            return string.IsNullOrEmpty(Permission);
        }
    }
    public class PermissionInfo : PermissionBase
    {
        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("permission");
            node.AppendAttribute("cmd", Name);
            node.AppendAttribute("permission_level", Permission.ToString());
        }
    }

    public class AdminPermissionInfo : PermissionBase
    {
        public int? DefaultPermission { get; set; }
        public int? ModeratorPermission { get; set; }

        public bool IsGroup { get; set; }

        public override void ApplyNode(SavannahTagNode parent)
        {
            if (IsGroup)
            {
                var node = parent.CreateChildElement("group");
                node.AppendAttribute("steamID", SteamId);
                node.AppendAttribute("name", Name);
                node.AppendAttribute("permission_level_default", DefaultPermission.ToString());
                node.AppendAttribute("permission_level_mod", ModeratorPermission.ToString());
            }
            else
            {
                var node = parent.CreateChildElement("user");
                node.AppendAttribute("steamID", SteamId);
                node.AppendAttribute("name", Name);
                node.AppendAttribute("permission_level", Permission);
            }
        }
    }

    public class WhitelistPermissionInfo : PermissionBase
    {
        public bool IsGroup { get; set; }

        public override void ApplyNode(SavannahTagNode parent)
        {
            if (IsGroup)
            {
                var node = parent.CreateChildElement("group");
                node.AppendAttribute("steamID", SteamId);
                node.AppendAttribute("name", Name);
            }
            else
            {
                var node = parent.CreateChildElement("user");
                node.AppendAttribute("steamID", SteamId);
                node.AppendAttribute("name", Name);
            }
        }
    }

    public class BlackListPermissionInfo : PermissionBase
    {
        private string unBanDate;

        public string UnBanDate
        {
            get => unBanDate;
            set => SetProperty(ref unBanDate, value);
        }
        public string Reason { get; set; }


        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("blacklisted");
            node.AppendAttribute("steamID", SteamId);
            node.AppendAttribute("name", Name);
            node.AppendAttribute("unbandate", UnBanDate);
            node.AppendAttribute("reason", Reason);
        }
    }

    public class PermissionEditorModel : ModelBase
    {
        #region Fields

        private bool canSave;
        private string openedFilePath;
        private string declaration;

        private ObservableCollection<PermissionInfo> commandPermissions = new ObservableCollection<PermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> adminPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> adminGroupPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<WhitelistPermissionInfo> whitelistPermissions = new ObservableCollection<WhitelistPermissionInfo>();
        private ObservableCollection<WhitelistPermissionInfo> whitelistGroupPermissions = new ObservableCollection<WhitelistPermissionInfo>();
        private ObservableCollection<BlackListPermissionInfo> blacklistPermissions = new ObservableCollection<BlackListPermissionInfo>();

        #endregion

        #region Properties

        public bool CanSave
        {
            get => canSave;
            set => SetProperty(ref canSave, value);
        }

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
        public ObservableCollection<WhitelistPermissionInfo> WhitelistPermissions
        {
            get => whitelistPermissions;
            set => SetProperty(ref whitelistPermissions, value);
        }

        public ObservableCollection<WhitelistPermissionInfo> WhitelistGroupPermissions
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
            NewFile();
        }

        public PermissionEditorModel(string adminFilePath)
        {
            LoadFile(adminFilePath);
            openedFilePath = adminFilePath;
            CanSave = true;
        }

        private void LoadFile(string path)
        {
            if (!File.Exists(path))
                return;

            var reader = new SavannahXmlReader(path);
            declaration = reader.Declaration;
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

        public void ClearPermissions()
        {
            CommandPermissions.Clear();
            AdminPermissions.Clear();
            AdminGroupPermissions.Clear();
            WhitelistPermissions.Clear();
            WhitelistGroupPermissions.Clear();
            BlacklistPermissions.Clear();
        }

        public void OpenFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            ClearPermissions();
            LoadFile(filePath);
            openedFilePath = filePath;
            CanSave = true;
        }

        public void NewFile()
        {
            OpenFile($"{ConstantValues.AppDirectoryPath}\\Settings\\Permissions\\serveradmin.xml");
            openedFilePath = string.Empty;
            CanSave = false;
        }

        public void Save()
        {
            Save(openedFilePath);
        }

        public void Save(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
                return;

            var root = new SavannahTagNode
            {
                TagName = "adminTools"
            };

            var permissions = root.CreateChildElement("permissions");
            var admins = root.CreateChildElement("admins");
            var whitelist = root.CreateChildElement("whitelist");
            var blacklist = root.CreateChildElement("blacklist");

            ApplyNodes(CommandPermissions, permissions);
            ApplyNodes(AdminPermissions, admins);
            ApplyNodes(AdminGroupPermissions, admins);
            ApplyNodes(WhitelistPermissions, whitelist);
            ApplyNodes(WhitelistGroupPermissions, whitelist);
            ApplyNodes(BlacklistPermissions, blacklist);

            var writer = new SavannahXmlWriter(declaration);
            writer.Write("test.xml", root);

            CanSave = true;
            openedFilePath = filePath;
        }

        public static void ApplyNodes<T>(ICollection<T> collection, SavannahTagNode node) where T : PermissionBase
        {
            var selectItems = collection.Where(item => item.ItemType == PermissionBase.PermissionItemType.Real).Select(item => item);
            foreach (var permission in selectItems)
                permission.ApplyNode(node);
        }

        private static IEnumerable<PermissionInfo> LoadCommand(SavannahXmlReader reader)
        {
            var nodes = (reader.GetNode("/adminTools/permissions") as SavannahTagNode)?.ChildNodes;
            var children = (nodes ?? Array.Empty<AbstractSavannahXmlNode>()).OfType<SavannahTagNode>();
            var permissions = from node in children
                let cmd = node.GetAttribute("cmd")
                let permission = node.GetAttribute("permission_level")
                where cmd != null && permission != null
                select new PermissionInfo
                {
                    Name = cmd.Value,
                    Permission = permission.Value
                };

            return permissions;
        }

        private static (IEnumerable<AdminPermissionInfo>, IEnumerable<AdminPermissionInfo>) LoadAdmin(SavannahXmlReader reader)
        {
            if (!(reader.GetNode("/adminTools/admins") is SavannahTagNode admin))
                return (new AdminPermissionInfo[0], new AdminPermissionInfo[0]);

            var groups = new List<AdminPermissionInfo>();
            var players = new List<AdminPermissionInfo>();
            var children = admin.ChildNodes.OfType<SavannahTagNode>();
            foreach (var node in children)
            {
                var info = new AdminPermissionInfo
                {
                    SteamId = node.GetAttribute("steamID")?.Value,
                    Name = node.GetAttribute("name")?.Value
                };

                if (node.TagName == "user")
                {
                    info.Permission = node.GetAttribute("permission_level")?.Value;
                    players.Add(info);
                }
                else
                {
                    info.IsGroup = true;
                    info.DefaultPermission = node.GetAttribute("permission_level_default")?.Value.ToInt();
                    info.ModeratorPermission = node.GetAttribute("permission_level_mod")?.Value.ToInt();
                    groups.Add(info);
                }
            }

            return (players, groups);
        }

        private static (IEnumerable<WhitelistPermissionInfo>, IEnumerable<WhitelistPermissionInfo>) LoadWhitelist(SavannahXmlReader reader)
        {
            var admin = reader.GetNode("/adminTools/whitelist") as SavannahTagNode;
            if (admin == null)
                return (new WhitelistPermissionInfo[0], new WhitelistPermissionInfo[0]);

            var groups = new List<WhitelistPermissionInfo>();
            var players = new List<WhitelistPermissionInfo>();
            var children = admin.ChildNodes.OfType<SavannahTagNode>();
            foreach (var node in children)
            {
                var info = new WhitelistPermissionInfo
                {
                    SteamId = node.GetAttribute("steamID")?.Value,
                    Name = node.GetAttribute("name")?.Value
                };

                if (node.TagName == "user")
                {
                    players.Add(info);
                }
                else
                {
                    info.IsGroup = true;
                    groups.Add(info);
                }
            }

            return (players, groups);
        }

        private static IEnumerable<BlackListPermissionInfo> LoadBlacklist(SavannahXmlReader reader)
        {
            var nodes = (reader.GetNode("/adminTools/blacklist") as SavannahTagNode)?.ChildNodes;
            var children = nodes.OfType<SavannahTagNode>();
            var permissions = from node in children
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
            var permissionBases = enumerable as T[] ?? enumerable.ToArray();
            foreach (var permissionInfo in permissionBases)
            {
                collection.Add(permissionInfo);
            }
            AssertTextChangedCommand(collection, initializer);
            AddDummyItemAction(collection, initializer);
        }

        private static void AssertTextChangedCommand<T>(ICollection<T> collection, Action<T> initializer) where T : PermissionBase, new()
        {
            foreach (var permissionInfo in collection)
            {
                permissionInfo.AddDummyAction = () => AddDummyItemAction(collection, initializer);
                permissionInfo.RemoveAction = () => collection.Remove(permissionInfo);
            }
        }

        private static void AddDummyItemAction<T>(ICollection<T> collection, Action<T> initializer) where T : PermissionBase, new()
        {
            var info = new T
            {
                ItemType = PermissionBase.PermissionItemType.Dummy,
                AddDummyAction = () => AddDummyItemAction(collection, initializer)
            };
            info.RemoveAction = () => collection.Remove(info);
            initializer?.Invoke(info);
            collection.Add(info);
        }
    }
}

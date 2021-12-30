using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CommonStyleLib.Models;
using Prism.Mvvm;
using SavannahXmlLib.XmlWrapper;
using SavannahXmlLib.XmlWrapper.Nodes;

namespace _7dtd_svmanager_fix_mvvm.Models.Permissions
{
    public abstract class PermissionBase : BindableBase
    {

        public enum PermissionItemType
        {
            Dummy,
            Real
        }

        private string _steamId;
        private string _name;
        private string _permission;

        public PermissionItemType ItemType { get; set; } = PermissionItemType.Real;

        public string Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        public string SteamId
        {
            get => _steamId;
            set => SetProperty(ref _steamId, value);
        }

        public string Permission
        {
            get => _permission;
            set => SetProperty(ref _permission, value);
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
            if (!string.IsNullOrEmpty(_steamId))
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
            node.AppendAttribute("permission_level", Permission);
        }
    }

    public class AdminPermissionInfo : PermissionBase
    {
        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("user");
            node.AppendAttribute("steamID", SteamId);
            if (!string.IsNullOrEmpty(Name))
                node.AppendAttribute("name", Name);
            node.AppendAttribute("permission_level", Permission);
        }
    }

    public class AdminGroupPermissionInfo : PermissionBase
    {
        private string _defaultPermission;
        private string _moderatorPermission;

        public string DefaultPermission
        {
            get => _defaultPermission;
            set => SetProperty(ref _defaultPermission, value);
        }

        public string ModeratorPermission
        {
            get => _moderatorPermission;
            set => SetProperty(ref _moderatorPermission, value);
        }

        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("group");
            node.AppendAttribute("steamID", SteamId);
            if (!string.IsNullOrEmpty(Name))
                node.AppendAttribute("name", Name);
            node.AppendAttribute("permission_level_default", DefaultPermission);
            node.AppendAttribute("permission_level_mod", ModeratorPermission);
        }
    }

    public class WhitelistPermissionInfo : PermissionBase
    {
        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("user");
            node.AppendAttribute("steamID", SteamId);
            if (!string.IsNullOrEmpty(Name))
                node.AppendAttribute("name", Name);
        }
    }

    public class WhitelistGroupPermissionInfo : PermissionBase
    {
        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("group");
            node.AppendAttribute("steamID", SteamId);
            if (!string.IsNullOrEmpty(Name))
                node.AppendAttribute("name", Name);
        }
    }

    public class BlackListPermissionInfo : PermissionBase
    {
        private string _unBanDate;

        public string UnBanDate
        {
            get => _unBanDate;
            set => SetProperty(ref _unBanDate, value);
        }
        public string Reason { get; set; }


        public override void ApplyNode(SavannahTagNode parent)
        {
            var node = parent.CreateChildElement("blacklisted");
            node.AppendAttribute("steamID", SteamId);
            if (!string.IsNullOrEmpty(Name))
                node.AppendAttribute("name", Name);
            node.AppendAttribute("unbandate", UnBanDate);
            node.AppendAttribute("reason", Reason);
        }
    }

    public class PermissionEditorModel : ModelBase
    {
        #region Fields

        private bool _isEdited;
        private bool _canSave;
        private string _openedFilePath;
        private string _declaration;

        private ObservableCollection<PermissionInfo> _commandPermissions = new ObservableCollection<PermissionInfo>();
        private ObservableCollection<AdminPermissionInfo> _adminPermissions = new ObservableCollection<AdminPermissionInfo>();
        private ObservableCollection<AdminGroupPermissionInfo> _adminGroupPermissions = new ObservableCollection<AdminGroupPermissionInfo>();
        private ObservableCollection<WhitelistPermissionInfo> _whitelistPermissions = new ObservableCollection<WhitelistPermissionInfo>();
        private ObservableCollection<WhitelistGroupPermissionInfo> _whitelistGroupPermissions = new ObservableCollection<WhitelistGroupPermissionInfo>();
        private ObservableCollection<BlackListPermissionInfo> _blacklistPermissions = new ObservableCollection<BlackListPermissionInfo>();

        #endregion

        #region Properties

        public bool IsEdited
        {
            get => _isEdited;
            set => SetProperty(ref _isEdited, value);
        }

        public bool CanSave
        {
            get => _canSave;
            set => SetProperty(ref _canSave, value);
        }

        public ObservableCollection<PermissionInfo> CommandPermissions
        {
            get => _commandPermissions;
            set => SetProperty(ref _commandPermissions, value);
        }
        public ObservableCollection<AdminPermissionInfo> AdminPermissions
        {
            get => _adminPermissions;
            set => SetProperty(ref _adminPermissions, value);
        }

        public ObservableCollection<AdminGroupPermissionInfo> AdminGroupPermissions
        {
            get => _adminGroupPermissions;
            set => SetProperty(ref _adminGroupPermissions, value);
        }
        public ObservableCollection<WhitelistPermissionInfo> WhitelistPermissions
        {
            get => _whitelistPermissions;
            set => SetProperty(ref _whitelistPermissions, value);
        }

        public ObservableCollection<WhitelistGroupPermissionInfo> WhitelistGroupPermissions
        {
            get => _whitelistGroupPermissions;
            set => SetProperty(ref _whitelistGroupPermissions, value);
        }
        public ObservableCollection<BlackListPermissionInfo> BlacklistPermissions
        {
            get => _blacklistPermissions;
            set => SetProperty(ref _blacklistPermissions, value);
        }

        #endregion

        public PermissionEditorModel()
        {
            NewFile();
        }

        public PermissionEditorModel(string adminFilePath)
        {
            LoadFile(adminFilePath);
            _openedFilePath = adminFilePath;
            CanSave = true;
        }

        private void LoadFile(string path)
        {
            if (!File.Exists(path))
                return;

            var reader = new SavannahXmlReader(path);
            _declaration = reader.Declaration;
            var cmdPermissions = LoadCommand(reader);
            var (adminPlayers, adminGroups) = LoadAdmin(reader);
            var (whitelistPlayers, whitelistGroups) = LoadWhitelist(reader);
            var blacklistPlayers = LoadBlacklist(reader);

            CreatePermissions(CommandPermissions, cmdPermissions, info => info.Permission = null);
            CreatePermissions(AdminPermissions, adminPlayers, info => info.Permission = null);
            CreatePermissions(AdminGroupPermissions, adminGroups, info =>
            {
                info.DefaultPermission = string.Empty;
                info.ModeratorPermission = string.Empty;
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
            _openedFilePath = filePath;
            CanSave = true;
            _isEdited = false;
        }

        public void NewFile()
        {
            OpenFile($"{ConstantValues.AppDirectoryPath}\\Settings\\Permissions\\serveradmin.xml");
            _openedFilePath = string.Empty;
            CanSave = false;
            _isEdited = false;
        }

        public void Save()
        {
            Save(_openedFilePath);
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

            var writer = new SavannahXmlWriter(_declaration);
            writer.Write(filePath, root);

            CanSave = true;
            _openedFilePath = filePath;
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

        private static (IEnumerable<AdminPermissionInfo>, IEnumerable<AdminGroupPermissionInfo>) LoadAdmin(SavannahXmlReader reader)
        {
            if (!(reader.GetNode("/adminTools/admins") is SavannahTagNode admin))
                return (new AdminPermissionInfo[0], new AdminGroupPermissionInfo[0]);

            var groups = new List<AdminGroupPermissionInfo>();
            var players = new List<AdminPermissionInfo>();
            var children = admin.ChildNodes.OfType<SavannahTagNode>();
            foreach (var node in children)
            {
                if (node.TagName == "user")
                {
                    var info = new AdminPermissionInfo
                    {
                        SteamId = node.GetAttribute("steamID")?.Value,
                        Name = node.GetAttribute("name")?.Value,
                        Permission = node.GetAttribute("permission_level")?.Value
                    };

                    players.Add(info);
                }
                else
                {
                    var info = new AdminGroupPermissionInfo
                    {
                        SteamId = node.GetAttribute("steamID")?.Value,
                        Name = node.GetAttribute("name")?.Value,
                        DefaultPermission = node.GetAttribute("permission_level_default")?.Value,
                        ModeratorPermission = node.GetAttribute("permission_level_mod")?.Value
                    };

                    groups.Add(info);
                }
            }

            return (players, groups);
        }

        private static (IEnumerable<WhitelistPermissionInfo>, IEnumerable<WhitelistGroupPermissionInfo>) LoadWhitelist(SavannahXmlReader reader)
        {
            var admin = reader.GetNode("/adminTools/whitelist") as SavannahTagNode;
            if (admin == null)
                return (new WhitelistPermissionInfo[0], new WhitelistGroupPermissionInfo[0]);

            var groups = new List<WhitelistGroupPermissionInfo>();
            var players = new List<WhitelistPermissionInfo>();
            var children = admin.ChildNodes.OfType<SavannahTagNode>();
            foreach (var node in children)
            {
                if (node.TagName == "user")
                {
                    var info = new WhitelistPermissionInfo
                    {
                        SteamId = node.GetAttribute("steamID")?.Value,
                        Name = node.GetAttribute("name")?.Value
                    };
                    players.Add(info);
                }
                else
                {

                    var info = new WhitelistGroupPermissionInfo
                    {
                        SteamId = node.GetAttribute("steamID")?.Value,
                        Name = node.GetAttribute("name")?.Value
                    };
                    groups.Add(info);
                }
            }

            return (players, groups);
        }

        private static IEnumerable<BlackListPermissionInfo> LoadBlacklist(SavannahXmlReader reader)
        {
            var nodes = (reader.GetNode("/adminTools/blacklist") as SavannahTagNode)?.ChildNodes;
            var children = (nodes ?? Array.Empty<AbstractSavannahXmlNode>()).OfType<SavannahTagNode>();
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

using System;

namespace SvManagerLibrary.Steam
{
    public class SteamIdValidator
    {
        public enum Universe : ulong
        {
            Individual,
            Public,
            Beta,
            Internal,
            Dev,
            RC,
        }

        public enum AccountType : ulong
        {
            Invalid,
            Individual,
            Multiseat,
            GameServer,
            AnonGameServer,
            Pending,
            ContentServer,
            Clan,
            Chat,
            P2PSuperSeeder,
            AnonUser
        }

        public const ulong UniverseMask = 0xFF00000000000000;
        public const ulong TypeMask = 0x00F0000000000000;
        public const ulong InstanceMask = 0x000FFFFF00000000;
        public const ulong AccountNumberMask = 0x00000000FFFFFFFE;
        public const ulong LsbMask = 0x0000000000000001;

        public static bool ValidateSteamId(ulong steamId)
        {
            var universe = (steamId & UniverseMask) >> 56;
            var type = (steamId & TypeMask) >> 52;
            //var instance = (steamId & InstanceMask) >> 32;
            //var accountNumber = (steamId & AccountNumberMask) >> 1;
            //var lsb = steamId & LsbMask;

            if (!Enum.IsDefined(typeof(Universe), universe))
                return false;
            if (!Enum.IsDefined(typeof(AccountType), type))
                return false;
            return true;
        }
    }
}

using System;

namespace SvManagerLibrary.Steam
{
    public class SteamIdConverter
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

        public static string ToSteamId(ulong steam64Id)
        {
            var universe = (steam64Id & UniverseMask) >> 56;
            var type = (steam64Id & TypeMask) >> 52;
            var instance = (steam64Id & InstanceMask) >> 32;
            var accountNumber = (steam64Id & AccountNumberMask) >> 1;
            var lsb = steam64Id & LsbMask;

            if (!Enum.IsDefined(typeof(Universe), universe))
                return string.Empty;
            if (!Enum.IsDefined(typeof(AccountType), type))
                return string.Empty;

            var steamId = $"STEAM_{universe}:{lsb}:{accountNumber}";

            return steamId;
        }
    }
}

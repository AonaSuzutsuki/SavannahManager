using System.Collections.Generic;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// user's information.
    /// </summary>
    public class PlayerInfo
    {
        public string Id { get; set; }
        public string Level { get; set; }
        public string Name { get; set; }
        public string Health { get; set; }
        public string ZombieKills { get; set; }
        public string PlayerKills { get; set; }
        public string Deaths { get; set; }
        public string Score { get; set; }
        public string Coord { get; set; }
        public string SteamId { get; set; }

        public override bool Equals(object obj)
        {
            return obj is PlayerInfo info &&
                   Id == info.Id &&
                   Level == info.Level &&
                   Name == info.Name &&
                   Health == info.Health &&
                   ZombieKills == info.ZombieKills &&
                   PlayerKills == info.PlayerKills &&
                   Deaths == info.Deaths &&
                   Score == info.Score &&
                   Coord == info.Coord &&
                   SteamId == info.SteamId;
        }

        public override int GetHashCode()
        {
            var hashCode = -730630918;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Id);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Level);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Health);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ZombieKills);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PlayerKills);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Deaths);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Score);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Coord);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(SteamId);
            return hashCode;
        }
    }
}

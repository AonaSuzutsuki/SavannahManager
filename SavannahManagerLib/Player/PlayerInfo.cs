using System.Collections.Generic;

namespace SvManagerLibrary.Player
{
    /// <summary>
    /// Provides an information of player.
    /// </summary>
    public class PlayerInfo
    {
        /// <summary>
        /// Player id in 7dtd server.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Player level.
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// Player name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Player health.
        /// </summary>
        public string Health { get; set; }

        /// <summary>
        /// Number of player's zombie kills.
        /// </summary>
        public string ZombieKills { get; set; }

        /// <summary>
        /// Number of player's player kills.
        /// </summary>
        public string PlayerKills { get; set; }

        /// <summary>
        /// Player death.
        /// </summary>
        public string Deaths { get; set; }

        /// <summary>
        /// Player score.
        /// </summary>
        public string Score { get; set; }

        /// <summary>
        /// Player position.
        /// </summary>
        public string Coord { get; set; }

        /// <summary>
        /// Player's Steam64ID.
        /// </summary>
        public string SteamId { get; set; }

        /// <summary>
        /// Check the equivalence of this object and the argument object.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <returns>It returns True if equivalent, False otherwise.</returns>
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

        /// <summary>
        /// Object.GetHashCode()
        /// </summary>
        /// <returns>The hash value.</returns>
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

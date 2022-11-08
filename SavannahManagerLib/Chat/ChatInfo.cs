using System.Collections.Generic;

namespace SvManagerLibrary.Chat
{
    /// <summary>
    /// Provides an information of chat.
    /// </summary>
    public class ChatInfo
    {
        /// <summary>
        /// Id of the player who sent you the message.
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// Steam ID of the player who sent you the message.
        /// </summary>
        public string SteamId { get; set; } = string.Empty;

        /// <summary>
        /// Name of the player who sent you the message.
        /// </summary>
        public string Name { set; get; } = string.Empty;

        /// <summary>
        /// The message that was sent.
        /// </summary>
        public string Message { set; get; } = string.Empty;

        /// <summary>
        /// Date of sending message.
        /// </summary>
        public string Date { get; set; } = string.Empty;

        /// <summary>
        /// Check the equivalence of this object and the argument object.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <returns>It returns True if equivalent, False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is ChatInfo info &&
                   Name == info.Name &&
                   Message == info.Message;
        }

        /// <summary>
        /// Object.GetHashCode()
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            var hashCode = -835697798;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            return hashCode;
        }

        /// <summary>
        /// Check the null or empty of player name and message.
        /// </summary>
        /// <param name="chatInfo">ChatInfo object.</param>
        /// <returns>It returns True if null or empty, False otherwise.</returns>
        public static bool IsNullOrEmpty(ChatInfo chatInfo)
        {
            if (chatInfo == null)
                return true;
            if (string.IsNullOrEmpty(chatInfo.Name) && string.IsNullOrEmpty(chatInfo.Message))
                return true;
            return false;
        }
    }
}

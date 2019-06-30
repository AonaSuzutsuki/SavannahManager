using System.Collections.Generic;

namespace SvManagerLibrary.Chat
{
    public class ChatInfo
    {
        public string Name { set; get; } = string.Empty;
        public string Message { set; get; } = string.Empty;

        public override bool Equals(object obj)
        {
            return obj is ChatInfo info &&
                   Name == info.Name &&
                   Message == info.Message;
        }

        public override int GetHashCode()
        {
            var hashCode = -835697798;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Message);
            return hashCode;
        }

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

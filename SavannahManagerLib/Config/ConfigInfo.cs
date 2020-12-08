using System.Collections.Generic;

namespace SvManagerLibrary.Config
{
    public class ConfigInfo
    {
        public string PropertyName { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is ConfigInfo info &&
                   PropertyName == info.PropertyName &&
                   Value == info.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -295901811;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PropertyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}

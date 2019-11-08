using System.Collections.Generic;

namespace SvManagerLibrary.XmlWrapper
{
    public class AttributeInfo
    {
        public string Name { get; set; }
        public string Value { get; set; }

        public override bool Equals(object obj)
        {
            return obj is AttributeInfo info &&
                   Name == info.Name &&
                   Value == info.Value;
        }

        public override int GetHashCode()
        {
            var hashCode = -244751520;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }

        public override string ToString()
        {
            return $"{Name}=\"{Value}\"";
        }
    }
}

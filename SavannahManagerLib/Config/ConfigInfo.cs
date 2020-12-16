using System.Collections.Generic;

namespace SvManagerLibrary.Config
{
    /// <summary>
    /// Provides an information of config.
    /// </summary>
    public class ConfigInfo
    {
        /// <summary>
        /// Property name.
        /// Value of property attribute.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Value of value attribute.
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// Check the equivalence of this object and the argument object.
        /// </summary>
        /// <param name="obj">Target object.</param>
        /// <returns>It returns True if equivalent, False otherwise.</returns>
        public override bool Equals(object obj)
        {
            return obj is ConfigInfo info &&
                   PropertyName == info.PropertyName &&
                   Value == info.Value;
        }

        /// <summary>
        /// Object.GetHashCode()
        /// </summary>
        /// <returns>The hash value.</returns>
        public override int GetHashCode()
        {
            var hashCode = -295901811;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PropertyName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
            return hashCode;
        }
    }
}

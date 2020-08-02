using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeyTap
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TapKey : IComparable, IComparable<TapKey>, IEquatable<TapKey>
    {
        #region Data

        [JsonProperty]
        public string Provider { get; }

        [JsonProperty]
        public string Icon { get; }

        [JsonProperty]
        public string Device { get; }

        [JsonProperty]
        public string Id { get; }

        [JsonProperty]
        public string Name { get; }

        #endregion

        #region Constructors

        public TapKey(
            string provider,
            string icon,
            string device = "Local",
            string id = "Default",
            string name = "Default")
        {
            Provider = provider;
            Icon = icon;
            Device = device;
            Id = id;
            Name = name;
        }

        #endregion

        #region Methods

        public int CompareTo(TapKey other)
        {
            if (other is null) return 1;
            return GetHashCode().CompareTo(other.GetHashCode());
        }

        public bool Equals(TapKey other)
        {
            return other != null && GetHashCode() == other.GetHashCode();
        }

        public override string ToString()
        {
            return $"{Provider}.{Device}.{Id}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public int CompareTo(object obj)
        {
            if (obj is null) return 1;
            return GetHashCode().CompareTo(obj.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetHashCode() == obj.GetHashCode();
        }

        #endregion
    }
}

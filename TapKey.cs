using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace KeyTap
{
    [JsonObject(MemberSerialization.OptIn)]
    public class TapKey
    {
        #region Data

        [JsonProperty]
        public string Provider { get; }

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
            string device = "Local",
            string id = "Default",
            string name = "Default")
        {
            Provider = provider;
            Device = device;
            Id = id;
            Name = name;
        }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Provider}.{Device}.{Id}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return obj != null && GetHashCode() == obj.GetHashCode();
        }

        #endregion
    }
}

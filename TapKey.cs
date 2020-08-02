using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyTap
{
    public class TapKey
    {
        #region Data

        string Type { get; }

        string Device { get; }

        string Id { get; }

        string Name { get; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Type}.{Device}.{Id}";
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return GetHashCode() == obj.GetHashCode();
        }

        #endregion
    }
}

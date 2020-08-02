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

        public string Device { get; }

        public string Id { get; }

        public string Name { get; }

        public IKeyTapProvider Provider { get; }

        #endregion

        #region Methods

        public override string ToString()
        {
            return $"{Provider.Name}.{Device}.{Id}";
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

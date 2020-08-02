using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyTap.Providers
{
    public sealed class USBProvider : IKeyTapProvider
    {
        #region Const Data

        public string Name { get; } = "USB";

        public string Icon { get; } = "ConnectPlugged";

        #endregion

        #region Key Events

        public event EventHandler<TapKey> KeyDown;
        public event EventHandler<TapKey> KeyUp;

        #endregion

        #region Dispose

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

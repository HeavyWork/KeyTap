using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyTap
{
    public interface IKeyTapEventProvider
    {

        public event EventHandler<TapKey> KeyDown;

        public event EventHandler<TapKey> KeyUp;

    }
}

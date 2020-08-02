using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KeyTap
{
    public interface IKeyTapProvider : IDisposable, IKeyTapEventProvider
    {
        string Name { get; }
        string Icon { get; }
    }
}

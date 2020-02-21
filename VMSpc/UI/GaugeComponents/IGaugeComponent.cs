using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VMSpc.UI.GaugeComponents
{
    public interface IGaugeComponent
    {
        void Update();
        void Enable();
        void Disable();
    }
}

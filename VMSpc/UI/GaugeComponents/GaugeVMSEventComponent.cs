using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VMSpc.Common;

namespace VMSpc.UI.GaugeComponents
{
    public class GaugeVMSEventComponent : GaugeComponent
    {
        protected bool enabled;
        protected uint eventID;

        public GaugeVMSEventComponent(uint eventID)
        {

        }
        public override void Disable()
        {
            enabled = false;
        }

        public override void Enable()
        {
            enabled = true;
        }

        public void Hide()
        {
            Visibility = Visibility.Hidden;
        }

        public void Show()
        {
            Visibility = Visibility.Visible;
        }

        public override void Draw()
        {
            throw new NotImplementedException();
        }

        public override void Update()
        {
            throw new NotImplementedException();
        }

        protected override void HandleNewData(VMSEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}

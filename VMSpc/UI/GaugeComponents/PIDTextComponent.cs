using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.Extensions.UI;

namespace VMSpc.UI.GaugeComponents
{
    public class PIDTextComponent : GaugePIDComponent
    {
        private TextBlock textBlock;
        public PIDTextComponent(ushort pid) : base(pid)
        {
            textBlock = new TextBlock();
            textBlock.Height = Height;
            Children.Add(textBlock);
        }

        public override void Draw()
        {
            textBlock.Width = Width;
            textBlock.Height = Height;
            textBlock.Text = "No Data";
            textBlock.ScaleText();
        }

        public override void Update()
        {
            if (pidValue != STALE_DATA && !double.IsNaN(pidValue))
                textBlock.Text = pidValue.ToString();
            else
                textBlock.Text = "NO DATA";
        }
    }
}

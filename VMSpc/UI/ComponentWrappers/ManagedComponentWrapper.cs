using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using VMSpc.Extensions.UI;
using VMSpc.UI.CustomComponents;
using VMSpc.UI.GaugeComponents;

namespace VMSpc.UI.ComponentWrappers
{
    public class ManagedComponentWrapper : ComponentCanvas
    {
        private StackPanel
            parentStack;
        private TextBlock
            titleBlock,
            unitTextBlock;
        private ManagedComponent 
            managedComponent;
        public string
            title,
            unitText;
        public bool
            showTitle,
            showUnit;
        public Orientation 
            orientation;
        private ulong
            eventID;
        private byte
            publisherInstance;
        private const int BorderDimension = 2;
        public ManagedComponentWrapper(ulong eventID, byte publisherInstance)
            : base()
        {
            this.eventID = eventID;
            this.publisherInstance = publisherInstance;
        }

        public void Draw()
        {
            Children.Clear();
            parentStack = new StackPanel()
            {
                Width = this.Width,
                Height = this.Height,
                Orientation = this.orientation,
            };
            Children.Add(parentStack);
            if (showTitle)
                AddTitleText();
            AddValueText();
            if (showUnit)
                AddUnitText();
            UIHelpers.BalanceTextblocks(titleBlock, unitTextBlock, managedComponent.valueText);
        }

        protected void AddTitleText()
        {
            titleBlock = new TextBlock();
            int denominator = 2;
            if (showUnit)
                denominator++;
            titleBlock.Text = title;
            titleBlock.Width = ((orientation == Orientation.Horizontal) ? (parentStack.Width / denominator) : parentStack.Width) - (BorderDimension * 2);
            titleBlock.Height = ((orientation == Orientation.Vertical) ? (parentStack.Height / denominator) : parentStack.Height) - (BorderDimension * 2);
            //Border border = new Border();
            //border.Child = titleBlock;
            //parentStack.Children.Add(border);
            parentStack.Children.Add(titleBlock);
            titleBlock.ScaleText();
        }

        protected void AddUnitText()
        {
            unitTextBlock = new TextBlock();
            int denominator = 2;
            if (showTitle)
                denominator++;
            unitTextBlock.Text = unitText;
            unitTextBlock.Width = ((orientation == Orientation.Horizontal) ? (parentStack.Width / denominator) : parentStack.Width ) - (BorderDimension * 2);
            unitTextBlock.Height = ((orientation == Orientation.Vertical) ? (parentStack.Height / denominator) : parentStack.Height) - (BorderDimension * 2);
            //Border border = new Border();
            //border.Child = unitTextBlock;
            //parentStack.Children.Add(border);
            parentStack.Children.Add(unitTextBlock);
            unitTextBlock.ScaleText();
        }

        protected void AddValueText()
        {
            managedComponent = new ManagedComponent(eventID, publisherInstance);
            int denominator = 1;
            if (showTitle)
                denominator++;
            if (showUnit)
                denominator++;
            managedComponent.Width =  ((orientation == Orientation.Horizontal) ? (parentStack.Width / denominator) : parentStack.Width) - (BorderDimension * 2);
            managedComponent.Height = ((orientation == Orientation.Vertical) ? (parentStack.Height / denominator) : parentStack.Height) - (BorderDimension * 2);
            //Border border = new Border();
            //border.Child = managedComponent;
            //parentStack.Children.Add(border);
            parentStack.Children.Add(managedComponent);
            managedComponent.Draw();
        }
    }
}

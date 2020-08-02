using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
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
        private Label
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
        public double FontSize
        {
            get => titleBlock.FontSize;
            set => titleBlock.FontSize = unitTextBlock.FontSize = managedComponent.FontSize = value;
        }
        private HorizontalAlignment horizontalContentAlignment;
        public HorizontalAlignment HorizontalContentAlignment
        {
            get => horizontalContentAlignment;
            set
            {
                horizontalContentAlignment = value;
                titleBlock.HorizontalContentAlignment =
                unitTextBlock.HorizontalContentAlignment =
                managedComponent.HorizontalContentAlignment = value;
            }
        }
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
            AddTitleText();
            AddValueText();
            AddUnitText();
            double min = UIHelpers.Min(titleBlock.FontSize, unitTextBlock.FontSize, managedComponent.FontSize);
            FontSize = min;
        }

        protected void AddTitleText()
        {
            titleBlock = new Label()
            {
                FontWeight = FontWeights.Bold,
            };
            if (!showTitle)
            {
                titleBlock.FontSize = 1000;
                return;
            }
            int denominator = 2;
            if (showUnit)
                denominator++;
            titleBlock.Content = title;
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
            unitTextBlock = new Label()
            {
                FontWeight = FontWeights.Bold,
            };
            if (!showUnit)
            {
                unitTextBlock.FontSize = 1000;
                return;
            }
            int denominator = 2;
            if (showTitle)
                denominator++;
            unitTextBlock.Content = unitText;
            unitTextBlock.Width = ((orientation == Orientation.Horizontal) ? (parentStack.Width / denominator) : parentStack.Width ) - (BorderDimension * 2);
            unitTextBlock.Height = ((orientation == Orientation.Vertical) ? (parentStack.Height / denominator) : parentStack.Height) - (BorderDimension * 2);
            //Border border = new Border();
            //border.Child = unitTextBlock;
            //parentStack.Children.Add(border);
            parentStack.Children.Add(unitTextBlock);
            unitTextBlock.ScaleText(5);
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

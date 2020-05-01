using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using VMSpc.Communication;
using static VMSpc.Constants;
using VMSpc.Helpers;
using VMSpc.DevHelpers;
using VMSpc.Panels;
using System.Timers;
using System.Reflection;
using System.ComponentModel;
using VMSpc.Enums;
using VMSpc.UI.CustomComponents;

namespace VMSpc.UI.DlgWindows
{
    public class VMSDialog : Window
    {
        protected VPanel panel;
        private Timer updateTimer;
        private List<DlgBind> BindingList;
        public const int ONE_WAY = 0;
        public const int TWO_WAY = 1;
        public const int ONE_WAY_WITH_TIMER = 2;
        public const int TWO_WAY_WITH_TIMER = 3;

        protected virtual void ApplyBindings()
        {
            DataContext = this;
        }

        /// <summary>
        /// Opens a Color Picker window and returns the newly-selected color. Returns true if the color was changed. Returs false if the user cancelled the operation
        /// </summary>
        /// <returns></returns>
        protected virtual bool ChangeColor(ref Color SelectedColor)
        {
            var colorPicker = new ColorPicker(SelectedColor);
            if (colorPicker.ShowDialog() == true)
            {
                SelectedColor = colorPicker.SelectedColor;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds all possible choices of the specified EnumType as ComboBoxItems to the passed comboBox
        /// </summary>
        protected void PopulateComboBox<EnumType>(ComboBox comboBox)
        {
            var types = EnumUtils.GetDescriptions<EnumType>();
            foreach (string type in types)
            {
                ComboBoxItem item = new ComboBoxItem()
                {
                    Content = type,
                };
                comboBox.Items.Add(item);
            }
        }

        public bool? ShowDialog(VPanel panel)
        {
            this.panel = panel;
            return ShowDialog();
        }
        /// <summary> 
        /// Creates a binding between data and UI elements. It allows for either ONE_WAY binding (the UI is updated by data) or
        /// TWO_WAY binding (the UI is still updated by the data, but the data source is updated by the UI property on close).
        /// </summary>
        protected void CreateBinding<T1, T2>(object UIObject, string UIPropName, object DataSource, string DataSourcePropName, int BindingType, bool UpdateOnTimer)
        {
            if (UIObject.GetType() != typeof(T1))
                return;
            FieldPointer<T1> UIPointer = new FieldPointer<T1>(UIObject, UIPropName);
            FieldPointer<T2> DataSourcePointer = new FieldPointer<T2>(DataSource, DataSourcePropName);
            if (UpdateOnTimer) BindingType += 2;  // <-- Converts ONE_WAY to ONE_WAY_WITH_TIMER and same for TWO_WAY
            if (BindingList == null)
                BindingList = new List<DlgBind>();
            BindingList.Add(new GenericDlgBind<T1, T2>(UIPointer, DataSourcePointer, BindingType));
            if (UpdateOnTimer && (updateTimer == null || !updateTimer.Enabled))
                updateTimer = CREATE_TIMER(OnUpdateDialog, 250);
            else
                OnUpdateDialog();
        }

        private void OnUpdateDialog()
        {
            if (BindingList != null)
            {
                foreach (var bind in BindingList)
                {
                    switch (bind.BindingType)
                    {
                        case ONE_WAY:
                        case ONE_WAY_WITH_TIMER:
                            bind.ProcessDataToUI();
                            break;
                        case TWO_WAY:
                        case TWO_WAY_WITH_TIMER:
                            bind.ProcessUIToData();
                            break;
                    }
                }
            }
        }
    }

    public abstract class DlgBind
    {
        public abstract void ProcessDataToUI();
        public abstract void ProcessUIToData();
        public int BindingType;
    }

    public class GenericDlgBind<T1, T2> : DlgBind
    {
        SharpPointer<T1> UIPointer;
        SharpPointer<T2> DataPointer;
        public GenericDlgBind(SharpPointer<T1> UIPointer, SharpPointer<T2> DataPointer, int BindingType)
        {
            this.UIPointer = UIPointer;
            this.DataPointer = DataPointer;
            this.BindingType = BindingType;
        }

        public override void ProcessDataToUI()
        {
            if (typeof(T1) == typeof(string) && typeof(T2) != typeof(string))
                UIPointer.Value = (T1)(object)DataPointer.Value.ToString();
        }

        public override void ProcessUIToData()
        {
            if (BindingType == VMSDialog.TWO_WAY)
            {
                //DataPointer.Value = (T2)UIPointer.Value; //TODO
            }
        }
    }
}

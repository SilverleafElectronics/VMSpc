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
using VMSpc.JsonFileManagers;
using static VMSpc.JsonFileManagers.ConfigurationManager;

namespace VMSpc.UI.DlgWindows.Advanced
{
    /// <summary>
    /// Interaction logic for ParameterItemEditor.xaml
    /// </summary>
    public partial class ParameterItemEditor : VMSDialog
    {
        protected JParameter parameter;
        private bool IsNewParameter = false;
        private static List<JParameter> Parameters = ConfigManager.ParamData.Contents.Parameters;
        public ParameterItemEditor(JParameter parameter)
        {
            this.parameter = parameter;
            InitializeComponent();
            if (parameter == null)
                ApplyDefaults();
            ApplyBindings();
        }

        protected void ApplyDefaults()
        {
            IsNewParameter = true;
            parameter = new JParameter
            {
                ParamName = "New Parameter",
                Abbreviation = "PARAM",
                Unit = "Unit",
                MetricUnit = "Metric Unit",
                Offset = 0,
                Multiplier = 1,
                GaugeMin = 0,
                GaugeMax = 0,
                LowYellow = 0,
                HighYellow = 0,
                LowRed = 0,
                HighRed = 0,
                Pid = 0,
                Format = "{0:0.#}",
            };
        }

        protected override void ApplyBindings()
        {
            base.ApplyBindings();
            GaugeNameEditBox.Text = parameter.ParamName;
            GaugeAbbreviationEditBox.Text = parameter.Abbreviation;
            UnitEditBox.Text = parameter.Unit;
            MetricUnitEditBox.Text = parameter.MetricUnit;
            ValueOffsetEditBox.Text = parameter.Offset.ToString();
            ValueMultiplierEditBox.Text = parameter.Multiplier.ToString();
            GaugeMinimumEditBox.Text = parameter.GaugeMin.ToString();
            GaugeMaximumEditBox.Text = parameter.GaugeMax.ToString();
            LowRedLineEditBox.Text = parameter.LowRed.ToString();
            HighRedLineEditBox.Text = parameter.HighRed.ToString();
            LowYellowLineEditBox.Text = parameter.LowYellow.ToString();
            HighYellowLineEditBox.Text = parameter.HighYellow.ToString();
            PIDEditBox.Text = parameter.Pid.ToString();
            NumDecimalsTextbox.Text = FormatConverter.ConvertFormatToDecimalPositions(parameter.Format).ToString();
        }

        private bool IsValidStringField(TextBox field)
        {
            return !(string.IsNullOrEmpty(field.Text));
        }

        private bool IsValidDoubleField(TextBox field)
        {
            return (double.TryParse(field.Text, out double _));
        }

        private bool GaugeMaxIsValid()
        {
            if (!double.TryParse(GaugeMaximumEditBox.Text, out double gaugeMax))
            {
                return false;
            }
            return (gaugeMax > double.Parse(GaugeMinimumEditBox.Text));
        }

        private bool HighYellowIsValid()
        {
            if (!double.TryParse(HighYellowLineEditBox.Text, out double highYellow))
            {
                return false;
            }
            return (highYellow > double.Parse(LowYellowLineEditBox.Text));
        }

        private bool HighRedIsValid()
        {
            if (!double.TryParse(HighRedLineEditBox.Text, out double highRed))
            {
                return false;
            }
            return (highRed > double.Parse(LowRedLineEditBox.Text));
        }

        private string GetPIDValidatorMessage()
        {
            if (!ushort.TryParse(PIDEditBox.Text, out ushort pid) && !(pid > 0 && pid < 65535))
            {
                return "PID must have a numeric value between 1 and 65535";
            }
            if (pid == parameter.Pid)
            {
                return null;
            }
            foreach (var parameter in Parameters)
            {
                if (pid == parameter.Pid)
                {
                    return
                        $"The PID {pid} is already used by {parameter.ParamName}. PIDs cannot be duplicated across " +
                        $"parameters. If you want to use {pid} for this parameter, first change the PID associated with " +
                        $"{parameter.ParamName}.";
                }
            }
            return null;
        }

        private string ValidateFields()
        {
            if (!IsValidStringField(GaugeNameEditBox))
                return "Gauge Name must not be empty";
            if (!IsValidStringField(GaugeAbbreviationEditBox))
                return "Gauge Abbreviation must not be empty";
            if (!IsValidDoubleField(ValueOffsetEditBox))
                return "Unit must have a numeric value";
            if (!IsValidDoubleField(ValueMultiplierEditBox))
                return "Value Multiplier must have a numeric value";
            if (!IsValidDoubleField(GaugeMinimumEditBox))
                return "Gauge Minimum must have a numeric value";
            if (!GaugeMaxIsValid())
                return "Gauge Maximum must have a numeric value greater than Gauge Minimum";
            if (!IsValidDoubleField(LowRedLineEditBox))
                return "Low Red Line must have a numeric value";
            if (!HighRedIsValid())
                return "High Red Line must have a numeric value greater than Low Red Line";
            if (!IsValidDoubleField(LowYellowLineEditBox))
                return "Low Yellow Line must have a numeric value";
            if (!HighYellowIsValid())
                return "High Yellow Line must have a numeric value greater than Low Yellow Line";
            //var result = 
            if (!int.TryParse(NumDecimalsTextbox.Text, out int result) || result < 0)
                return "Decimal Positions is not in the correct format. Please enter a whole number greater than or equal to 0";
            return GetPIDValidatorMessage();
        }

        private void SaveFields()
        {
            parameter.ParamName = GaugeNameEditBox.Text;
            parameter.Abbreviation = GaugeAbbreviationEditBox.Text;
            parameter.Unit = UnitEditBox.Text;
            parameter.MetricUnit = MetricUnitEditBox.Text;
            parameter.Offset = double.Parse(ValueOffsetEditBox.Text);
            parameter.Multiplier = double.Parse(ValueMultiplierEditBox.Text);
            parameter.GaugeMin = double.Parse(GaugeMinimumEditBox.Text);
            parameter.GaugeMax = double.Parse(GaugeMaximumEditBox.Text);
            parameter.LowRed = double.Parse(LowRedLineEditBox.Text);
            parameter.HighRed = double.Parse(HighRedLineEditBox.Text);
            parameter.LowYellow = double.Parse(LowYellowLineEditBox.Text);
            parameter.HighYellow = double.Parse(HighYellowLineEditBox.Text);
            parameter.Format = FormatConverter.ConvertDecimalPositionsToFormat(int.Parse(NumDecimalsTextbox.Text));
            var pid = ushort.Parse(PIDEditBox.Text);
            if (pid != parameter.Pid)
            {
                MessageBoxResult result = MessageBox.Show(
                    $"Are you sure you want to change the PID associated with {parameter.ParamName} " +
                    $"from {parameter.Pid} to {pid}? You should only do so if you are certain that " +
                    $"{parameter.ParamName} can be parsed from the PID {pid}. This action cannot be undone, so " +
                    $"take note of the previous PID, {parameter.Pid}, before proceeding",
                    "Verify PID Change",
                    MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Yes)
                {
                    parameter.Pid = pid;
                }
            }
            ConfigManager.ParamData.ProcessUpdates(parameter);
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            var InvalidMessage = ValidateFields();
            if (string.IsNullOrEmpty(InvalidMessage))
            {
                if (IsNewParameter)
                {
                    ConfigManager.ParamData.AddParam(parameter);
                }
                DialogResult = true;
                SaveFields();
                Close();
            }
            else
            {
                MessageBox.Show(InvalidMessage);
                return; 
            }
        }
    }

    public static class FormatConverter
    { 
        public static int ConvertFormatToDecimalPositions(string format)
        {
            if (format == null || !format.Contains("#"))
            {
                return 0;
            }
            else
            {
                return format.Count(x => x == '#');
            }
        }

        public static string ConvertDecimalPositionsToFormat(int numberOfDecimals)
        {
            if (numberOfDecimals == 0)
            {
                return "{0:0}";
            }
            else
            {
                string format = "{0:0.";
                for (int i = 0; i < numberOfDecimals; i++)
                {
                    format += "#";
                }
                format += "}";
                return format;
            }
        }
    }
}



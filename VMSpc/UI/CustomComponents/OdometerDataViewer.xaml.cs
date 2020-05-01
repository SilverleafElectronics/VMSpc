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
using System.Windows.Navigation;
using System.Windows.Shapes;
using VMSpc.JsonFileManagers;

namespace VMSpc.UI.CustomComponents
{
    /// <summary>
    /// Interaction logic for OdometerDataViewer.xaml
    /// </summary>
    public partial class OdometerDataViewer : Window
    {
        protected OdometerDataContents OdometerDataContents;
        public OdometerDataViewer(OdometerDataContents OdometerDataContents)
        {
            InitializeComponent();
            DG_OdometerDataViewer.DataContext = OdometerDataContents.OdometerDataEntries;
        }
    }
}

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

namespace VMSpc.UI.DlgWindows
{
    /// <summary>
    /// Interaction logic for About.xaml
    /// </summary>
    public partial class AboutDlg : VMSDialog
    {
        public AboutDlg()
        {
            InitializeComponent();
            version.Text = "VMSpc Version: " + About.version;
            copyright.Text = "Copyright (C) 2002-" + DateTime.Now.Year.ToString() + " SilverLeaf Electronics, Inc.";
            moreInfo.Text = "Please visit our web forum at www.silverleafelectronics.com.";
            phone.Text = "Phone: 888-741-0259";
            address.Text = "Mail: 2490 Ferry St. SW, Albany, OR 9732";
        }
    }
}

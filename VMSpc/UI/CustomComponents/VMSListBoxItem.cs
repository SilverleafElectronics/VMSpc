﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace VMSpc.UI.CustomComponents
{
    class VMSListBoxItem : ListBoxItem
    {
        public ushort ID { get; set; }
        public VMSListBoxItem() : base()
        {
        }
      
    }
}

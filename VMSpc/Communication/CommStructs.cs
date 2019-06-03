using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using VMSpc.DevHelpers;
using System.Windows;
using System.Timers;

namespace VMSpc.Communication
{
    public struct CanMessage
    {
        public byte address;
        public uint pgn;
        public byte[] data;
    }
}
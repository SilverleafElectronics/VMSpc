using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VMSpc.XmlFileManagers
{
    public class OdometerManager : XmlFileManager
    {
        public double startFuel,
                      startHours,
                      startMiles,
                      startLiters,
                      startKilometers;
        public OdometerManager(string filename) : base (filename)
        {
            startFuel       = Double.Parse(getNodeByTagName("Start-Fuel").InnerText);
            startHours      = Double.Parse(getNodeByTagName("Start-Hours").InnerText);
            startMiles      = Double.Parse(getNodeByTagName("Start-Miles").InnerText);
            startLiters     = Double.Parse(getNodeByTagName("Start-Liters").InnerText);
            startKilometers = Double.Parse(getNodeByTagName("Start-Km").InnerText);
        }

        protected override void CreateTemplate()
        {
            base.CreateTemplate();
            OverwriteFile(
                "<Root>" +
                "   < Version > V4.0 </ Version >" +
                "   < Odo-Trip-Data >" +
                "       < Start-Fuel > 0 </Start-Fuel >" +
                "       < Start-Hours > 0 </Start-Hours >" +
                "       < Start-Miles > 0 </Start-Miles >" +
                "       <Start-Liters> 0 </Start-Liters>" +
                "       <Start-Km> 0 </Start-Km>" +
                "   </ Odo-Trip-Data >" +
                "</ Root > "
                );
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VMSpc.XmlFileManagers
{
    public sealed class ParamDataManager : XmlFileManager
    {
        public Dictionary <ushort, VParameter> parameters;
        static ParamDataManager() { }
        public static ParamDataManager ParamData { get; set; } = new ParamDataManager();
        public ParamDataManager() : base ("ParamData.xml")
        {
            parameters = new Dictionary<ushort, VParameter>();
        }
        private void Load()
        {
            foreach (XmlNode node in xmlDoc.GetElementsByTagName("Param"))
            {
                VParameter param = new VParameter(node);
                parameters.Add(param.Pid, param);
            }
        }

        public static bool SEEN(ushort pid)
        {
            return ParamData.parameters[pid].Seen;
        }

        public override void Activate()
        {
            base.Activate();
            Load();
        }
    }
}

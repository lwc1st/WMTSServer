using SharpMap.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WMTSServer.Models
{
    public class LayerData
    {
        public string LabelColumn { get; set; }
        public IStyle Style { get; set; }
    }
}

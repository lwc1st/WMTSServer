using SharpMap.Styles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WMTSServer.Models
{
    /// <summary>
    /// Defines the <see cref="VectorLayerModel" />
    /// </summary>
    public class VectorLayerModel
    {
        /// <summary>
        /// Gets or Sets the SQL Server Column
        /// </summary>
        public string Table { get; set; }

        /// <summary>
        /// Gets or sets the Vector Layer Style
        /// </summary>
        public VectorStyle Style { get; set; }
    }

}

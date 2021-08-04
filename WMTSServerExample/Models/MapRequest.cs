using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WMTSServerWeb
{
    public class MapRequest
    {
        public string Layer { get; set; }//	图层名称
        public int TileMatrix { get; set; }//瓦片层号
        public int TileRow { get; set; }//瓦片行号
        public int TileCol { get; set; }//瓦片列号
        public string Service { get; set; } = "WMTS";//服务类型
        public string Request { get; set; } = "GetMap";//请求类型
        public string Version { get; set; } = "1.1.1";//WMTS服务的版本，默认值为1.0.0
        public string Style { get; set; }//图层的风格
        public string TileMatrixSet { get; set; } = "EPSG:4326";//矩阵集标识
        public string Format { get; set; } = "image/png";//	请求瓦片的格式
        public int Width { get; set; } //宽
        public int Height { get; set; }//高
        public string Authority
        {
            get
            {
                var authority = "EPSG";
                if (TileMatrixSet != null && TileMatrixSet.Contains(":"))
                {
                    authority = TileMatrixSet.Substring(0, TileMatrixSet.IndexOf(':') - 1);
                }
                return authority;
            }
        }
        public int SRID
        {
            get
            {
                var srid = 4326;
                if (TileMatrixSet != null && TileMatrixSet.Contains(":"))
                {
                    var identifier = TileMatrixSet[(TileMatrixSet.IndexOf(':') + 1)..];
                    srid = Int32.Parse(identifier);
                }
                return srid;
            }
        }
    }
}
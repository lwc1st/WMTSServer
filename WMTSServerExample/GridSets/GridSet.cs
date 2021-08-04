using System;

namespace TileGrid
{
    public class GridSet
    {
        /// <summary>
        /// Create tile gridset
        /// </summary>
        /// <param name="srs">Spatial reference system</param>
        /// <param name="schema">Y axis schema,default TMS,y increase from down to top</param>
        /// <param name="minZoom">Min zoom,default 4</param>
        /// <param name="maxZoom">Max zoom,default 18</param>
        /// <param name="dpi">DPI,default 90.714</param>
        /// <param name="tileWidthPixel">Tile width(px),default 256 px</param>
        /// <param name="tielHeightPixel">Tile height(px),default 256 px</param>
        /// <returns>GridSet</returns>
        public static GridSet Create(SRS srs, YAxisSchema schema = YAxisSchema.Tms, int minZoom = 4, int maxZoom = 18, double dpi = 90.71428571428572, int tileWidthPixel = 256, int tielHeightPixel = 256)
        {
            return GridSetFactory.CreateGridSet(srs, schema, minZoom, maxZoom, dpi, tileWidthPixel, tielHeightPixel);
        }
        public SRS SRS { get; set; }
        public int EPSG { get; set; }
        public double DPI { get; set; }
        public int TileWidthPixel { get; set; }
        public int TileHeightPixel { get; set; }
        public YAxisSchema YAxisSchema { get; set; }
        public int MinZoom { get; set; }
        public int MaxZoom { get; set; }
        public Grid[] GridLevels { get; set; }
        public int LevelCount => MaxZoom - MinZoom + 1;
        public double[] BoundingBox { get; set; }
        public double Res { get; set; }
        internal Func<int, Grid> GridFunc { get; set; }
        internal GridSet() { }

        internal GridSet(int ePSG, double dPI, double res, int tileWidthPixel, int tileHeightPixel, YAxisSchema yAxisSchema, int minZoom, int maxZoom, Grid[] gridLevels, Func<int, Grid> gridFunc)
        {
            EPSG = ePSG;
            DPI = dPI;
            Res = res;
            TileWidthPixel = tileWidthPixel;
            TileHeightPixel = tileHeightPixel;
            YAxisSchema = yAxisSchema;
            MinZoom = minZoom;
            MaxZoom = maxZoom;
            GridLevels = gridLevels;
            GridFunc = gridFunc;
        }

        /// <summary>
        /// Calculate tile index about one give point
        /// </summary>
        /// <param name="xCoord">x of point</param>
        /// <param name="yCoord">y of point</param>
        /// <param name="zoom">Zoom of tile index </param>
        /// <returns>Tile index</returns>
        public int[] GetTileIndex(double xCoord, double yCoord, int zoom)
        {
            int xIndex = 0;
            int yIndex = 0;

            var grid = GridFunc(zoom);
            var numX = grid.NumTilesWidth;
            var numY = grid.NumTilesHeight;

            var tileLength = grid.Resolution * TileWidthPixel;
            var tileHeight = grid.Resolution * TileHeightPixel;

            var minx = BoundingBox[0];
            var miny = BoundingBox[1];
            var maxy = BoundingBox[3];

            xIndex = (int)((xCoord - minx) / tileLength);

            if (YAxisSchema == YAxisSchema.Tms)
            {
                yIndex = (int)((yCoord - miny) / tileHeight);
            }
            else if (YAxisSchema == YAxisSchema.Xyz)
            {
                yIndex = (int)((maxy - yCoord) / tileHeight);
            }
            else
            {
                throw new NotSupportedException();
            }

            return new[] { xIndex, yIndex };
        }

        public double[] GetTileBBox(int x, int y, int z)
        {
            var grid = GetGrid(z);
            var gridBBox = BoundingBox;
            var tileBBox = new double[4];

            var tileLength = grid.Resolution * TileWidthPixel;
            var tileHeight = grid.Resolution * TileHeightPixel;

            var minx = gridBBox[0] + x * tileLength;
            var maxx = minx + tileLength;
            tileBBox[0] = minx;
            tileBBox[2] = maxx;

            if (YAxisSchema == YAxisSchema.Tms)
            {
                var miny = gridBBox[1] + y * tileHeight;
                var maxy = miny + tileHeight;
                tileBBox[1] = miny;
                tileBBox[3] = maxy;
            }
            else if (YAxisSchema == YAxisSchema.Xyz)
            {
                var maxy = gridBBox[3] - y * tileHeight;
                var miny = maxy - tileHeight;
                tileBBox[1] = miny;
                tileBBox[3] = maxy;
            }

            return tileBBox;
        }
        public Grid GetGrid(int z) => GridFunc(z);
    }
}

using System;

namespace TileGrid
{
    internal static  class GridSetFactory
    {
        internal static GridSet CreateGridSet(SRS srs,YAxisSchema schema,int minZoom,int maxZoom,double dpi,int tileWidthPixel,int tielHeightPixel)
        {
            if(schema != YAxisSchema.Tms && schema != YAxisSchema.Xyz)
            {
                throw new NotSupportedException();
            }

            int tileNumberWide,tileNumberHigh;

            //maxx - minx   minx miny maxx maxy 0 1 2 3 
            var extentWidth = srs.BBox[2] - srs.BBox[0];
            var extentHeight = srs.BBox[3] - srs.BBox[1];

            var xRes = extentWidth / tileWidthPixel;
            var yRes = extentHeight / tielHeightPixel;

            if(xRes <= yRes)
            {
                tileNumberWide = 1;
                tileNumberHigh = (int)Math.Round(yRes / xRes);

                yRes = yRes / tileNumberHigh;
            }
            else
            {// N tile wide but one tile high
                tileNumberHigh = 1;
                tileNumberWide = (int)Math.Round(xRes / yRes);

                xRes = xRes / tileNumberWide; //xres == extent / (256 * N) ==  (extentX / 256) * ( 1  /  N) ==  xRes / N
            }

            var res = Math.Max(xRes,yRes);
            var exWidth = tileNumberWide * tileWidthPixel * res;
            var exHeight = tileNumberHigh * tielHeightPixel * res;

            var exBBox = new double[]{srs.BBox[0],srs.BBox[1],srs.BBox[2],srs.BBox[3]};
            
            exBBox[2] = exBBox[0] + exWidth;
            if (schema == YAxisSchema.Tms)//y from down to top
            {
                exBBox[3] = exBBox[1] + exHeight;
            }
            else if (schema == YAxisSchema.Xyz)
            {//y from top to down
                exBBox[1] = exBBox[3] - exHeight;
            }

            var levelNumber = maxZoom - minZoom + 1;
            
            var gs = new GridSet();
            gs.EPSG = srs.EPSG;
            gs.SRS = srs;
            gs.DPI = dpi;
            gs.Res = res;
            gs.TileHeightPixel = tielHeightPixel;
            gs.TileWidthPixel = tileWidthPixel;
            gs.YAxisSchema = schema;
            gs.MinZoom = minZoom;
            gs.MaxZoom = maxZoom;
            gs.GridLevels = new Grid[gs.LevelCount];
            gs.BoundingBox = exBBox;

            gs.GridFunc = new Func<int, Grid>(z =>
            {
                var grid = new Grid();
                grid.ZoomLevel = z;

                var resolution = res / Math.Pow(2,z);
                var scaleDenom = (resolution * srs.MetersPerUnit) / (0.0254/dpi);
                grid.Resolution = resolution;
                grid.ScaleDenom = scaleDenom;

                grid.NumTilesWidth = (int)(tileNumberWide * Math.Pow(2,z));
                grid.NumTilesHeight = (int)(tileNumberHigh * Math.Pow(2,z));

                return grid;
            });

            for (int i = 0; i < levelNumber; i++)
            {
                var zoomLevel = minZoom + i;
                var grid = new Grid();
                grid.ZoomLevel = zoomLevel;

                var resolution = res / Math.Pow(2,zoomLevel);
                var scaleDenom = (resolution * srs.MetersPerUnit) / (0.0254/dpi);
                grid.Resolution = resolution;
                grid.ScaleDenom = scaleDenom;

                grid.NumTilesWidth = (int)(tileNumberWide * Math.Pow(2,zoomLevel));
                grid.NumTilesHeight = (int)(tileNumberHigh * Math.Pow(2,zoomLevel));
                gs.GridLevels[i] = grid;
            }

            // 0    res / 2^0 == res / 1
            // 1    res / 2^1 == res / 2.0
            // 2    res / 2^2 == res / 4.0

            //at zoom 0 ,m * n ==  tileNumberWide,tileNumberHigh;
            //at zoom 1 , m * n == tileNumberWide * 2,tileNumberHigh * 2
            //at zoom 3 , m * n == tileNumberWide * 2 * 2,tileNumberHigh * 2 * 2
            //at zoom x , m * n == tileNumberWide * 2^(x),tileNumberHigh * 2^(x)

            return gs;
        }
    }
}

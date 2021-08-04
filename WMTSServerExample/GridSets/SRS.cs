namespace TileGrid
{
    public class SRS
    {
        private static double MetersPerDegree =  111319.49079327358;
        public static double GetMetersPerUnit(Unit unit)
        {
            double result = 0.0;
            switch (unit)
            {
                case Unit.Degree:
                    result = MetersPerDegree;
                    break;
                case Unit.Meter:
                    result = 1.0;
                    break;
                case Unit.Kilometer:
                    result = 0.001;
                    break;
                case Unit.Feet:
                    result = 0.3048;
                    break;
                case Unit.Inche:
                    result = 0.0254;
                    break;
                case Unit.Centimeter:
                    result = 0.01;
                    break;
                case Unit.Millimeter:
                    result = 0.001;
                    break;
            }
            return result;
        }
        public static SRS Epsg4326 => new SRS
                    (
                        4326,
                        Unit.Degree,
                        111319.49079327358,
                        new double[] { -180.0,-90.0,180.0,90.0 }
                    );
        public static SRS Epsg3857 => new SRS
                    (
                        3857,
                        Unit.Meter,
                        1,
                        new double[] {
                            -20037508.34,
                            -20037508.34,
                            20037508.34,
                            20037508.34}
                    );
        public static SRS Epsg4490 => new SRS
                    (
                        4490,
                        Unit.Degree,
                        111319.49079327358,
                        new double[]{
                            73.62,
                            16.7,
                            134.77 ,
                            53.56
                        }
                    );

        public SRS(int ePSG, Unit unit, double metersPerUnit, double[] bBox)
        {
            EPSG = ePSG;
            Unit = unit;
            MetersPerUnit = metersPerUnit;
            BBox = bBox;
        }

        public int EPSG { get; set; }
        public Unit Unit { get; set; }
        public double MetersPerUnit { get; set; }
        public double[] BBox { get; set; }// minx miny maxx maxy
    }
}

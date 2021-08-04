namespace TileGrid
{
    public class Grid
    {
        public int ZoomLevel { get; set; }
        public long NumTilesWidth { get; set; }
        public long NumTilesHeight { get; set; }
        public double Resolution { get; set; }
        public double ScaleDenom { get; set; }
    }
}

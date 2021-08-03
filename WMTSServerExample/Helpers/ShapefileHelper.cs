using ProjNet.CoordinateSystems;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMap.Styles;
using SharpMap;
using NetTopologySuite;
using ProjNet.CoordinateSystems.Transformations;
using WMTSServer.Models;

namespace WMTSServer.Helpers
{
    public static class ShapefileHelper
    {
        private static readonly IDictionary<string, LayerData> Nyc;

        static ShapefileHelper()
        {
            LayerData landmarks = new LayerData
            {
                LabelColumn = "LANAME",
                Style = new VectorStyle
                {
                    EnableOutline = true,
                    Fill = new SolidBrush(Color.FromArgb(192, Color.LightBlue))
                }
            };
            LayerData roads = new LayerData
            {
                LabelColumn = "NAME",
                Style = new VectorStyle
                {
                    EnableOutline = false,
                    Fill = new SolidBrush(Color.FromArgb(192, Color.LightGray)),
                    Line = new Pen(Color.FromArgb(200, Color.DarkBlue), 0.5f)
                }
            };
            LayerData pois = new LayerData
            {
                LabelColumn = "NAME",
                Style = new VectorStyle
                {
                    PointColor = new SolidBrush(Color.FromArgb(200, Color.DarkGreen)),
                    PointSize = 10
                }
            };

            Nyc = new Dictionary<string, LayerData>
            {
                { "poly_landmarks", landmarks },
                { "tiger_roads", roads },
                { "poi", pois }
            };
        }
        public static Map Spherical(Size map_size)
        {
            var gss = new NtsGeometryServices();
            var css = new SharpMap.CoordinateSystems.CoordinateSystemServices(
                new CoordinateSystemFactory(),
                new CoordinateTransformationFactory(),
                SharpMap.Converters.WellKnownText.SpatialReference.GetAllReferenceSystems());

            GeoAPI.GeometryServiceProvider.Instance = gss;
            SharpMap.Session.Instance
                .SetGeometryServices(gss)
                .SetCoordinateSystemServices(css)
                .SetCoordinateSystemRepository(css);

            Map map = new Map(map_size);

            IDictionary<string, LayerData> dict = Nyc;
            var ctFac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            var pos = ctFac.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, ProjectedCoordinateSystem.WebMercator);
            var neg = ctFac.CreateFromCoordinateSystems(ProjectedCoordinateSystem.WebMercator, GeographicCoordinateSystem.WGS84);

            foreach (string layer in dict.Keys)
            {
                string path = Path.Combine(Environment.CurrentDirectory, "data\\nyc", $"{layer}.shp");
                if (!File.Exists(path))
                    throw new FileNotFoundException("file not found", path);
                string name = Path.GetFileNameWithoutExtension(layer);
                LayerData data = dict[layer];
                ShapeFile source = new ShapeFile(path, true);
                VectorLayer lyr = new VectorLayer(name, source)
                {
                    //SRID = 4326,
                    //TargetSRID = 900913,
                     CoordinateTransformation = pos,
                     ReverseCoordinateTransformation = neg,
                    Style = (VectorStyle)data.Style,
                    SmoothingMode = SmoothingMode.AntiAlias,
                    IsQueryEnabled = true,
                    Enabled = true
                };
                map.Layers.Add(lyr);
            }
            return map;
        }
        public static Map Default()
        {
            Map map = new Map(new Size(500,500));

            IDictionary<string, LayerData> dict = Nyc;
            var ctFac = new ProjNet.CoordinateSystems.Transformations.CoordinateTransformationFactory();
            var pos = ctFac.CreateFromCoordinateSystems(GeographicCoordinateSystem.WGS84, ProjectedCoordinateSystem.WebMercator);
            var neg = ctFac.CreateFromCoordinateSystems(ProjectedCoordinateSystem.WebMercator, GeographicCoordinateSystem.WGS84);

            foreach (string layer in dict.Keys)
            {
                string path = Path.Combine(Environment.CurrentDirectory, "data\\nyc", $"{layer}.shp");
                if (!File.Exists(path))
                    throw new FileNotFoundException("file not found", path);

                string name = Path.GetFileNameWithoutExtension(layer);
                LayerData data = dict[layer];
                ShapeFile source = new ShapeFile(path, true);
                VectorLayer item = new VectorLayer(name, source)
                {
                    //SRID = 4326,
                    //TargetSRID = 900913,
                    CoordinateTransformation = pos,
                    ReverseCoordinateTransformation = neg,
                    Style = (VectorStyle)data.Style,
                    SmoothingMode = SmoothingMode.AntiAlias,
                    IsQueryEnabled = true
                };
                map.Layers.Add(item);
            }
            return map;
        }
    }
}

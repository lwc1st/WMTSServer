// Copyright 2005, 2006 - Morten Nielsen (www.iter.dk)
//
// This file is part of SharpMap.
// SharpMap is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as publishedusing System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Microsoft.AspNetCore.Http;
using WMTSServerWeb;
using Mapstache;
using GeoAPI.Geometries;
using GeoAPI.CoordinateSystems.Transformations;
using WMTSServer.Helpers;
using System;
using System.Threading.Tasks;
using static SharpMap.Web.Wms.Capabilities;
using SharpMap;
using WMTSServer.Helpers;

namespace Handlers
{
    public static class WMTSServer
    {
        private static readonly ICoordinateTransformation projection = ProjHelper.LatLonToGoogle();

        public static async Task ParseQueryStringAsync(WmsServiceDescription description, HttpContext context)
        {
            var request = new MapRequest();
            if (context.Request.Query.ContainsKey(nameof(request.Request)))
            {
                request.Request = context.Request.Query[nameof(request.Request)];
            }
            if (context.Request.Query.ContainsKey(nameof(request.Version)))
            {
                request.Version = context.Request.Query[nameof(request.Version)];
            }
            if (context.Request.Query.ContainsKey(nameof(request.Service)))
            {
                request.Service = context.Request.Query[nameof(request.Service)];
            }
            if (context.Request.Query.ContainsKey(nameof(request.Layer)))
            {
                request.Layer = context.Request.Query[nameof(request.Layer)];
            }
            if (context.Request.Query.ContainsKey(nameof(request.Format)))
            {
                request.Format = context.Request.Query[nameof(request.Format)];
            }
            if (context.Request.Query.ContainsKey(nameof(request.TileCol)) && int.TryParse(context.Request.Query[nameof(request.TileCol)], out var tileCol))
            {
                request.TileCol = tileCol;
            }
            if (context.Request.Query.ContainsKey(nameof(request.TileRow)) && int.TryParse(context.Request.Query[nameof(request.TileRow)], out var tileRow))
            {
                request.TileRow = tileRow;
            }
            if (context.Request.Query.ContainsKey(nameof(request.TileMatrix)) && int.TryParse(context.Request.Query[nameof(request.TileMatrix)], out var tileMatrix))
            {
                request.TileMatrix = tileMatrix;
            }
            if (context.Request.Query.ContainsKey(nameof(request.Width)) && int.TryParse(context.Request.Query[nameof(request.Width)], out var width))
            {
                request.Width = width;
            }
            if (context.Request.Query.ContainsKey(nameof(request.Height)) && int.TryParse(context.Request.Query[nameof(request.Height)], out var height))
            {
                request.Height = height;
            }
            Envelope bbox = GetBoundingBoxInLatLngWithMargin(request.TileCol, request.TileRow, request.TileMatrix);
            Map map = ShapefileHelper.Spherical(new Size(request.Width, request.Height));
            map.ZoomToBox(bbox);
            var mapImage = map.GetMap();
            using var ms = new MemoryStream();
            mapImage.Save(ms, ImageFormat.Png);
            mapImage.Dispose();
            var buffer = ms.ToArray();
            context.Response.Clear();
            context.Response.ContentType = GetEncoderInfo(request.Format).MimeType;
            await context.Response.Body.WriteAsync(buffer, 0, buffer.Length);
            await context.Response.Body.FlushAsync();

        }

        public static ImageCodecInfo GetEncoderInfo(String mimeType)
        {
            foreach (var encoder in ImageCodecInfo.GetImageEncoders())
                if (encoder.MimeType == mimeType)
                    return encoder;
            return null;
        }

        private static Envelope GetBoundingBoxInLatLngWithMargin(int tileX, int tileY, int zoom)
        {
            System.Drawing.Point px1 = new System.Drawing.Point((tileX * 256), (tileY * 256));
            System.Drawing.Point px2 = new System.Drawing.Point(((tileX + 1) * 256), ((tileY + 1) * 256));

            PointF ll1 = TileSystemHelper.PixelXYToLatLong(px1, zoom);
            PointF ll2 = TileSystemHelper.PixelXYToLatLong(px2, zoom);

            double[] prj1 = projection.MathTransform.Transform(new double[] { ll1.X, ll1.Y });
            double[] prj2 = projection.MathTransform.Transform(new double[] { ll2.X, ll2.Y });
            Envelope bbox = new Envelope();
            bbox.ExpandToInclude(prj1[0], prj1[1]);
            bbox.ExpandToInclude(prj2[0], prj2[1]);
            return bbox;
        }
    }
}

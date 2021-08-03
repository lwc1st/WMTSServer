﻿using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace  WMTSServer.Helpers
{
    public static class ExtensionMethods
    {
        public static string GetAbsoluteUri(this HttpRequest request)
        {
            return new StringBuilder()
                .Append(request.Scheme)
                .Append("://")
                .Append(request.Host)
                .Append(request.PathBase)
                .Append(request.Path)
                .Append(request.QueryString)
                .ToString();
        }
        public static string Join(this IEnumerable<string> strings)
        {
            return Join(strings, ',');
        }

        public static string Join(this IEnumerable<string> strings, char separator)
        {
            if (strings == null) 
                return String.Empty;

            var sb = new StringBuilder();
            foreach (var s in strings) 
                sb.Append(s).Append(separator);
            return sb.ToString().TrimEnd(separator);
        }

        public static string Fix(this string bbox)
        {
            if (String.IsNullOrEmpty(bbox))
                throw new ArgumentNullException("bbox");

            var arr = bbox.Split(',');
            Flip(arr, 0, 1);
            Flip(arr, 2, 3);
            return arr.Join(',');
        }

        private static void Flip(IList<string> arr, int first, int second) 
        {
            var temp = arr[first];
            arr[first] = arr[second];
            arr[second] = temp;
        }
    }
}

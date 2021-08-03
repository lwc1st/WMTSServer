using System;
using System.Threading.Tasks;
using GeoAPI;
using Microsoft.AspNetCore.Http;
using NetTopologySuite;
using WMTSServer.Helpers;
using static SharpMap.Web.Wms.Capabilities;

namespace Handlers
{
    public abstract class AbstractMapMiddleware : IMiddleware
    {
        private static readonly object SyncLock = new object();

        static AbstractMapMiddleware()
        {
            GeometryServiceProvider.SetInstanceIfNotAlreadySetDirectly(NtsGeometryServices.Instance);
        }

        protected string GetFixedUrl(HttpRequest request)
        {
            Uri uri = new Uri(request.GetAbsoluteUri());
            string absoluteUri = request.GetAbsoluteUri();
            return uri.Query.Length <= 0 ? absoluteUri : absoluteUri.Replace(uri.Query, String.Empty);
        }

        protected WmsServiceDescription GetDescription(string url)
        {
            var description = new WmsServiceDescription("Acme Corp. Map Server", url);
            description.MaxWidth = 500;
            description.MaxHeight = 500;
            description.Abstract = "Map Server maintained by Acme Corporation. Contact: webmaster@wmt.acme.com. High-quality maps showing roadrunner nests and possible ambush locations.";
            description.Keywords = new[] { "bird", "roadrunner", "ambush" };
            description.ContactInformation.PersonPrimary.Person = "John Doe";
            description.ContactInformation.PersonPrimary.Organisation = "Acme Inc";
            description.ContactInformation.Address.AddressType = "postal";
            description.ContactInformation.Address.Country = "Neverland";
            description.ContactInformation.VoiceTelephone = "1-800-WE DO MAPS";
            return description;
        }
        public abstract Task InvokeAsync(HttpContext context, RequestDelegate next);

        public bool IsReusable
        {
            get { return false; }
        }
    }
}

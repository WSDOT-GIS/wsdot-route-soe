using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Location;
using ESRI.Server.SOESupport;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WsdotRouteSoe
{
    public class LocationResult
    {
        public IRouteLocation2 RouteLocation { get; set; }
        public IGeometry Geometry { get; set; }
        public esriLocatingError LocatingError { get; set; }

        public JsonObject ToJsonObject()
        {
            var output = new JsonObject();
            output.AddJsonObject("geometry", Conversion.ToJsonObject(Geometry));
            output.AddJsonObject("routeLocation", RouteLocation.ToJsonObject());
            output.AddString("locatingError", Enum.GetName(typeof(esriLocatingError), LocatingError));
            return output;
        }
    }
}

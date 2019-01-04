using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.SOESupport;
using System;
using System.Collections.Generic;

namespace WsdotRouteSoe
{
    public static class Extensions
    {
        // TODO: make routeIdFieldName configurable instead of hard-coded.

        public static IRouteLocator2 GetRouteLocator(this IServerObjectHelper serverObjectHelper, long layerId, string routeIdFieldName)
        {
            var server = (IMapServer4)serverObjectHelper.ServerObject;
            var serverDataAccess = (IMapServerDataAccess)server;
            var dataSet = (IDataset)serverDataAccess.GetDataSource(server.DefaultMapName, Convert.ToInt32(layerId));
            var name = dataSet.FullName;
            IRouteLocatorName routeLocatorName = new RouteMeasureLocatorNameClass
            {
                RouteFeatureClassName = name,
                RouteIDFieldName = routeIdFieldName,
                RouteMeasureUnit = esriUnits.esriMiles
            };
            name = (IName)routeLocatorName;
            var routeLocator = (IRouteLocator2)name.Open();
            return routeLocator;
        }

        public static JsonObject ToJsonObject(this IRouteLocation2 routeLocation)
        {
            var output = new JsonObject();
            output.AddString("routeId", $"{routeLocation.RouteID}");
            if (routeLocation is IRouteMeasurePointLocation pointLoc)
            {
                output.AddDouble("measure", pointLoc.Measure);
            } else if (routeLocation is IRouteMeasureLineLocation lineLoc)
            {
                output.AddDouble("fromMeasure", lineLoc.FromMeasure);
                output.AddDouble("toMeasure", lineLoc.ToMeasure);
            }
            return output;
        }

        public static IEnumerable<IRouteLocation2> ToRouteLocations(this IEnumerable<JsonObject> jArray)
        {
            int elementNo = -1;
            foreach (var jToken in jArray)
            {
                elementNo++;
                bool hasRouteId = jToken.TryGetString("RouteID", out string routeId);
                bool hasMeasure = jToken.TryGetAsDouble("Measure", out double? measure);

                var hasFromMeasure = jToken.TryGetAsDouble("FromMeasure", out double? fromMeasure);
                var hasToMeasure = jToken.TryGetAsDouble("ToMeasure", out double? toMeasure);

                if (hasMeasure)
                {
                    var location = new RouteMeasurePointLocationClass
                    {
                        RouteID = routeId,
                        Measure = measure.Value
                    };
                    yield return location;
                }
                else if (fromMeasure.HasValue && toMeasure.HasValue)
                {
                    var location = new RouteMeasureLineLocationClass
                    {
                        RouteID = routeId,
                        FromMeasure = fromMeasure.Value,
                        ToMeasure = toMeasure.Value
                    };
                    yield return location;
                }
                else
                {
                    throw new ArgumentException($"Input JArray element #{elementNo} did not have valid measure value(s): {jToken.ToString()}", nameof(jArray));
                }

            }
        }

        public static IEnumerable<IFeatureLayer2> EnumerateFeatureLayers(this IMap map)
        {
            // UID for "IFeatureLayer"
            UID uid = new UIDClass { Value = "{40A9E885-5533-11d0-98BE-00805F7CED21}" };
            var enumLayers = map.Layers[uid, true];
            var layer = enumLayers.Next();
            while (layer != null)
            {
                yield return layer as IFeatureLayer2;
            }
        }

        public static IRouteLocator2 CreateRouteLocator(this IDataset featureClass, string routeIdFieldName, esriUnits routeMeasureUnit)
        {
            IRouteLocatorName routeLocatorName = new RouteMeasureLocatorNameClass
            {
                RouteFeatureClassName = featureClass.FullName,
                RouteIDFieldName = routeIdFieldName,
                RouteMeasureUnit = routeMeasureUnit
            };
            var rtLocator = (IRouteLocator2)((IName)routeLocatorName).Open();
            return rtLocator;
        }
    }
}

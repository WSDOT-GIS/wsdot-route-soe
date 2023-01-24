//using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Location;
using ESRI.ArcGIS.Server;
using ESRI.Server.SOESupport;
using System;
using System.Collections.Generic;

namespace Wsdot.Lrs.Location
{
    /// <summary>
    /// A <see langword="static"/> <see langword="class"/> that provides extension methods.
    /// </summary>
    public static class Extensions
    {
        // TODO: make routeIdFieldName configurable instead of hard-coded.

        /// <summary>
        /// Gets an <see cref="IRouteLocator2{T}"/> for the route layer of the map service.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="serverObjectHelper"></param>
        /// <param name="layerId"></param>
        /// <param name="routeIdFieldName"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IRouteLocator2<T> GetRouteLocator<T>(this IServerObjectHelper serverObjectHelper, long layerId, string routeIdFieldName) where T: notnull
        {
            var server = (IMapServer)serverObjectHelper.ServerObject;
            var serverDataAccess = (IMapServerDataAccess)server;
            var dataSet = (IDataset)serverDataAccess.GetDataSource(server.DefaultMapName, Convert.ToInt32(layerId));
            var name = dataSet.FullName;
            throw new NotImplementedException();
            //IRouteLocatorName routeLocatorName = new RouteMeasureLocatorNameClass
            //{
            //    RouteFeatureClassName = name,
            //    RouteIDFieldName = routeIdFieldName,
            //    RouteMeasureUnit = esriUnits.esriMiles
            //};
            //name = (IName)routeLocatorName;
            //var routeLocator = (IRouteLocator2)name.Open();
            //return routeLocator;
        }

        /// <summary>
        /// Converts the route location to a <see cref="JsonObject"/>
        /// </summary>
        /// <typeparam name="T">The data type of the RouteID field. For WSDOT, this will always be a <see cref="string"/>.</typeparam>
        /// <param name="routeLocation"></param>
        /// <returns></returns>
        public static JsonObject ToJsonObject<T>(this IRouteLocation2<T> routeLocation) where T : notnull
        {
            var output = new JsonObject();
            output.AddString("routeId", $"{routeLocation.RouteID}");
            if (routeLocation is IRouteMeasurePointLocation<T> pointLoc)
            {
                output.AddDouble("measure", pointLoc.Measure);
            } else if (routeLocation is IRouteMeasureLineLocation<T> lineLoc)
            {
                output.AddDouble("fromMeasure", lineLoc.FromMeasure);
                output.AddDouble("toMeasure", lineLoc.ToMeasure);
            }
            return output;
        }

        /// <summary>
        /// Converts a collection of <see cref="JsonObject">JsonObjects</see> to <see cref="IRouteLocation2{T}"/> objects.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="jArray"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException">
        /// Thrown if any of the route location objects in the array do not meet one of the following requirements:
        /// <list type="bullet">
        /// <item>have a <see cref="IRouteMeasurePointLocation{T}.Measure"/></item>
        /// <item>have both <see cref="IRouteMeasureLineLocation{T}.ToMeasure"/> and <see cref="IRouteMeasureLineLocation{T}.FromMeasure"/> properties.</item>
        /// </list>
        /// </exception>
        public static IEnumerable<IRouteLocation2<string>> ToRouteLocations<T>(this IEnumerable<JsonObject> jArray) where T : notnull
        {
            int elementNo = -1;
            foreach (var jToken in jArray)
            {
                elementNo++;
                bool hasRouteId = jToken.TryGetString("RouteID", out string routeId);
                bool hasMeasure = jToken.TryGetAsDouble("Measure", out double? measure);

                var hasFromMeasure = jToken.TryGetAsDouble("FromMeasure", out double? fromMeasure);
                var hasToMeasure = jToken.TryGetAsDouble("ToMeasure", out double? toMeasure);

                if (hasMeasure && measure.HasValue)
                {
                    var location = new RouteMeasurePointLocation<string>(routeId, measure.Value);
                    yield return location;
                }
                else if (hasFromMeasure && fromMeasure.HasValue && hasToMeasure && toMeasure.HasValue)
                {
                    var location = new RouteMeasureLineLocation<string>(routeId, fromMeasure.Value, toMeasure.Value);
                    yield return location;
                }
                else
                {
                    throw new ArgumentException($"Input JArray element #{elementNo} did not have valid measure value(s): {jToken}", nameof(jArray));
                }

            }
        }

        /// <summary>
        /// Creates a <see cref="IRouteLocator2{T}"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="featureClass"></param>
        /// <param name="routeIdFieldName"></param>
        /// <param name="routeMeasureUnit"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public static IRouteLocator2<T> CreateRouteLocator<T>(this IDataset featureClass, string routeIdFieldName, esriUnits routeMeasureUnit) where T: notnull
        {
            throw new NotImplementedException();
            //IRouteLocatorName routeLocatorName = new RouteMeasureLocatorNameClass
            //{
            //    RouteFeatureClassName = featureClass.FullName,
            //    RouteIDFieldName = routeIdFieldName,
            //    RouteMeasureUnit = routeMeasureUnit
            //};
            //var rtLocator = (IRouteLocator2)((IName)routeLocatorName).Open();
            //return rtLocator;
        }
    }
}

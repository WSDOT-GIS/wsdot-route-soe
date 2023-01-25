using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace ESRI.ArcGIS.Location
{

    public interface IRouteLocator2<T> where T : notnull
    {
        /// <summary>Read-only property  Extent The route locator's extent (same a route feature class' extent).</summary>
        string Extent { get; }
        //Method GetRouteGeometry    The route(s) corresponding to the route location.
        /// <summary>Read-only property  HasSpatialIndex Indicates if the route feature class has a spatial index.</summary>
        string HasSpatialIndex { get; }
        //Method Locate  Locates a point or line route location.
        void Locate(IRouteLocation2<T> loc, out IGeometry result, out esriLocatingError locatingError);

        //Method LocateRow   Locates an event table row containing a point or line route location.
        /// <summary>Read-only property  MeasureUnit The units of the route measures.</summary>
        string MeasureUnit { get; }
        /// <summary>Read-only property  RouteFeatureClass The route feature class (Polyline with M feature class).</summary>
        string RouteFeatureClass { get; }
        /// <summary>Read-only property  RouteIDFieldIndex The field index of the route identifier.</summary>
        string RouteIDFieldIndex { get; }
        /// <summary>Read-only property  RouteIDFieldName The route identifier field from the route feature class.</summary>
        string RouteIDFieldName { get; }
        /// <summary>Read-only property  RouteIDFieldNameDelimited The delimited route identifier field of the route feature class.</summary>
        string RouteIDFieldNameDelimited { get; }
        /// <summary>Read-only property  RouteIDIsString Indicates if the route identifier field type is string.</summary>
        string RouteIDIsString { get; }
        ///// <summary>Read-only property  RouteIDIsUnique Indicates whether the route ID is unique(Obsolete).</summary>
        //[Obsolete("The original Esri version had this present but marked as obsolete. Doing so here so they will match.")]
        //string RouteIDIsUnique { get; set; }
        /// <summary>Read/write property     RouteWhereClause The where clause that limits the routes events can be located on.</summary>
        string RouteWhereClause { get; }
        /// <summary>Read-only property  SpatialReference The route locator's spatial reference (same as route feature class' spatial reference).</summary>
        string SpatialReference { get; }
    }
}

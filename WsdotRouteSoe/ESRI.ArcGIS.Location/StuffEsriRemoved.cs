using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.ComponentModel;
using System.Xml.Linq;

namespace ESRI.ArcGIS.Location
{
    /// <summary>
    /// Provides access to route location properites. Note: the <see cref="IRouteLocation{T}"/>
    /// interface has been superseded by <see cref="IRouteLocation2{T}"/>.
    /// Please consider using the more recent version.
    /// </summary>
    /// <typeparam name="T">
    /// The type of the <see cref="RouteID"/> property. 
    /// Can be one of the following: <see cref="string"/>, <see cref="int"/>, <see cref="double"/>.
    /// </typeparam>
    public interface IRouteLocation<T> where T : notnull
    {
        /// <summary>
        /// The route location's lateral offset ([ArcObjects object would] default [to] 0.0).
        /// </summary>
        public double LateralOffset { get; set; }
        /// <summary>Read/write property     measureUnit The route location's measure units.</summary>
        public esriUnits MeasureUnit { get; set; }
        /// <summary>Read/write property     routeID The route identifier(string, integer or double).</summary>
        public T RouteID { get; set; }
    }

    /// <summary>
    /// Provides access to route location properites.
    /// </summary>
    /// <typeparam name="T"><see cref="IRouteLocation{T}"/></typeparam>
    public interface IRouteLocation2<T> : IRouteLocation<T> where T : notnull
    {

        /// <summary>
        /// Indicates if the offset should based on 
        /// the M direction (<see langword="true"/>)
        /// or the digitized direction (<see langword="false"/>).
        /// </summary>
        public bool MDirectionOffsetting { get; set; }
    }

    /// <summary>
    /// Provides access to point specific route-measure location properties.
    /// </summary>
    /// <typeparam name="T"><see cref="IRouteLocation{T}"/></typeparam>
    interface IRouteMeasureLineLocation<T> : IRouteLocation2<T> where T : notnull
    {
        /// <summary>The 'from' measure value.</summary>
        public double FromMeasure { get; set; }
        /// <summary>The 'to' measure value.</summary>
        public double ToMeasure { get; set; }

    }

    /// <summary>
    /// Provides access to point specific route-measure location properties.
    /// </summary>
    /// <typeparam name="T"><see cref="IRouteLocation{T}"/>
    /// </typeparam>
    public interface IRouteMeasurePointLocation<T> : IRouteLocation2<T> where T : notnull
    {
        /// <summary>
        /// The measure value.
        /// </summary>
        public double Measure { get; set; }
    }


    /// <summary>
    /// A route measure point location.
    /// </summary>
    /// <typeparam name="T"><see cref="IRouteLocation{T}"/></typeparam>
    public record RouteMeasurePointLocation<T> : IRouteMeasurePointLocation<T> where T : notnull
    {
        public double Measure { get; set; }
        public bool MDirectionOffsetting { get; set; }
        public double LateralOffset { get; set; }
        public esriUnits MeasureUnit { get; set; }
        public T RouteID { get; set; }

        public RouteMeasurePointLocation(T routeID, double measure, esriUnits measureUnit = esriUnits.esriFeet, bool mDirectionOffsetting = default, double lateralOffset = default)
        {
            this.RouteID = routeID;
            this.Measure = measure;
            this.MeasureUnit = measureUnit;
            this.MDirectionOffsetting = mDirectionOffsetting;
            this.LateralOffset = lateralOffset;
        }
    }

    /// <summary>
    /// A route measure line location.
    /// </summary>
    /// <typeparam name="T"><see cref="IRouteLocation{T}"/></typeparam>
    public record RouteMeasureLineLocation<T> : IRouteMeasureLineLocation<T> where T : notnull
    {
        public double FromMeasure { get; set; }
        public double ToMeasure { get; set; }
        public bool MDirectionOffsetting { get; set; }
        public double LateralOffset { get; set; }
        public esriUnits MeasureUnit { get; set; }
        public T RouteID { get; set; }

        public RouteMeasureLineLocation(T routeId, double fromMeasure, double toMeasure, esriUnits measureUnit = esriUnits.esriFeet, double lateralOffset = default, bool mDirectionOffsetting = default)
        {
            RouteID = routeId;
            FromMeasure = fromMeasure;
            ToMeasure = toMeasure;
            MeasureUnit = measureUnit;
            LateralOffset = lateralOffset;
            MDirectionOffsetting = mDirectionOffsetting;
        }
    }

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

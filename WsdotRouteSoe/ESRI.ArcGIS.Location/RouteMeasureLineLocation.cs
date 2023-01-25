using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Location
{
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
}

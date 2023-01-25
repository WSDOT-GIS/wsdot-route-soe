using ESRI.ArcGIS.esriSystem;

namespace ESRI.ArcGIS.Location
{
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
}

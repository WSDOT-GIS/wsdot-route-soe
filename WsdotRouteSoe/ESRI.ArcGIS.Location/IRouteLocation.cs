using ESRI.ArcGIS.esriSystem;

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
}

namespace ESRI.ArcGIS.Location
{
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
}

namespace ESRI.ArcGIS.Location
{
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
}

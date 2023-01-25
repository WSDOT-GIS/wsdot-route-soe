namespace ESRI.ArcGIS.Location
{
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
}

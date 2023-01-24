// Copyright 2018 ESRI
// 
// All rights reserved under the copyright laws of the United States
// and applicable international laws, treaties, and conventions.
// 
// You may freely redistribute and use this sample code, with or
// without modification, provided you include the original copyright
// notice and use restrictions.
// 
// See the use restrictions at <your Enterprise SDK install location>/userestrictions.txt.
// 

using System;
using System.Linq;
using System.Text;

using System.Collections.Specialized;
using System.Runtime.InteropServices;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.Server.SOESupport;
using ESRI.ArcGIS.Location;

//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace Wsdot.Lrs.Location
{
    /// <summary>
    /// Defines the Server Object Extension (SOE)
    /// </summary>
    [ComVisible(true)]
    [Guid("356fe3bf-067f-4aac-b81b-0bd803e480be")]
    [ClassInterface(ClassInterfaceType.None)]
    [ServerObjectExtension("MapServer",//use "MapServer" if SOE extends a Map service and "ImageServer" if it extends an Image service.
        AllCapabilities = "",
        DefaultCapabilities = "",
        Description = "",
        DisplayName = "WsdotRouteSoe",
        Properties = "",
        SupportsREST = true,
        SupportsSOAP = false)]
    public class WsdotRouteSoe : IServerObjectExtension, IObjectConstruct, IRESTRequestHandler
    {

        const string routeIdFieldName = "RouteIdentifier";


        private readonly string soe_name;

#pragma warning disable IDE0052 // Remove unread private members
        private IPropertySet configProps;
        private readonly ServerLogger logger;
#pragma warning restore IDE0052 // Remove unread private members
        private IServerObjectHelper serverObjectHelper;
        private readonly IRESTRequestHandler reqHandler;

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. Will be set in Construct.
        /// <summary>
        /// Creates a new instance of this class.
        /// </summary>
        public WsdotRouteSoe()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable. Will be set in Construct.
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema());
        }

        #region IServerObjectExtension Members

        /// <summary>
        /// Performs SOE initialization.
        /// </summary>
        /// <param name="pSOH"></param>
        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
        }

        /// <summary>
        /// Run on SOE shutdown.
        /// </summary>
        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        /// <summary>
        /// Handles setting of user-defined property settings.
        /// </summary>
        /// <param name="props"></param>
        public void Construct(IPropertySet props)
        {
            configProps = props;
        }

        #endregion

        #region IRESTRequestHandler Members

        /// <summary>
        /// Handles the user request for SOE schema.
        /// </summary>
        /// <returns></returns>
        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        /// <summary>
        /// Handles rest requests and returns response as <see cref="byte"/> <see cref="System.Array"/>
        /// </summary>
        /// <param name="Capabilities"></param>
        /// <param name="resourceName"></param>
        /// <param name="operationName"></param>
        /// <param name="operationInput"></param>
        /// <param name="outputFormat"></param>
        /// <param name="requestProperties"></param>
        /// <param name="responseProperties"></param>
        /// <returns></returns>
        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new(soe_name, false, RootResHandler);

            RestOperation sampleOper = new("findRouteLocations",
                                                      new string[] { "layer", "locations" },
                                                      new string[] { "json" },
                                                      FindRouteLocationsHandler);

            rootRes.operations.Add(sampleOper);

            return rootRes;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string? responseProperties)
        {
            responseProperties = null;

            JsonObject result = new();
            //result.AddString("hello", "world");

            return Encoding.UTF8.GetBytes(result.ToJson());
        }

        private byte[] FindRouteLocationsHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string? responseProperties)
        {
            responseProperties = null;

            if (!operationInput.TryGetAsLong("layer", out long? layerId))
            {
                throw new ArgumentException("Layer ID not provided");
            }


            bool hasLocations = operationInput.TryGetArray("locations", out object[] locationsArray);
            if (!hasLocations)
            {
                throw new ArgumentException($"Expected \"locations\" to be a JSON array: {operationInput.ToJson()}", nameof(operationInput));
            }

            var locations = locationsArray.Cast<JsonObject>().ToRouteLocations<string>();

            IRouteLocator2<string> routeLocator = serverObjectHelper.GetRouteLocator<string>(layerId.GetValueOrDefault(0), routeIdFieldName);

            var located = locations.Select(loc =>
            {
                routeLocator.Locate(loc, out IGeometry result, out esriLocatingError locatingError);
                return new LocationResult<string>
                {
                    RouteLocation = loc,
                    Geometry = result,
                    LocatingError = locatingError
                };
            });

            var output = new JsonObject();
            output.AddArray("results", located.Select(l => l.ToJsonObject()).ToArray());
            return Encoding.UTF8.GetBytes(output.ToJson());

            //byte[] bytes;
            //var serializer = JsonSerializer.CreateDefault();
            //using (var memoryStream = new MemoryStream())
            //using (var textWriter = new StreamWriter(memoryStream, Encoding.UTF8))
            //{
            //    serializer.Serialize(textWriter, located);
            //    bytes = memoryStream.GetBuffer();
            //}
            //return bytes;
        }
    }
}

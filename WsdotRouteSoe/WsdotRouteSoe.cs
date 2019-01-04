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
using ESRI.ArcGIS.SOESupport;
using ESRI.ArcGIS.Location;

//TODO: sign the project (project properties > signing tab > sign the assembly)
//      this is strongly suggested if the dll will be registered using regasm.exe <your>.dll /codebase


namespace WsdotRouteSoe
{
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


        private string soe_name;

        private IPropertySet configProps;
        private IServerObjectHelper serverObjectHelper;
        private ServerLogger logger;
        private IRESTRequestHandler reqHandler;

        public WsdotRouteSoe()
        {
            soe_name = this.GetType().Name;
            logger = new ServerLogger();
            reqHandler = new SoeRestImpl(soe_name, CreateRestSchema()) as IRESTRequestHandler;
        }

        #region IServerObjectExtension Members

        public void Init(IServerObjectHelper pSOH)
        {
            serverObjectHelper = pSOH;
        }

        public void Shutdown()
        {
        }

        #endregion

        #region IObjectConstruct Members

        public void Construct(IPropertySet props)
        {
            configProps = props;
        }

        #endregion

        #region IRESTRequestHandler Members

        public string GetSchema()
        {
            return reqHandler.GetSchema();
        }

        public byte[] HandleRESTRequest(string Capabilities, string resourceName, string operationName, string operationInput, string outputFormat, string requestProperties, out string responseProperties)
        {
            return reqHandler.HandleRESTRequest(Capabilities, resourceName, operationName, operationInput, outputFormat, requestProperties, out responseProperties);
        }

        #endregion

        private RestResource CreateRestSchema()
        {
            RestResource rootRes = new RestResource(soe_name, false, RootResHandler);

            RestOperation sampleOper = new RestOperation("findRouteLocations",
                                                      new string[] { "layer", "locations" },
                                                      new string[] { "json" },
                                                      FindRouteLocationsHandler);

            rootRes.operations.Add(sampleOper);

            return rootRes;
        }

        private byte[] RootResHandler(NameValueCollection boundVariables, string outputFormat, string requestProperties, out string responseProperties)
        {
            responseProperties = null;

            JsonObject result = new JsonObject();
            //result.AddString("hello", "world");

            return Encoding.UTF8.GetBytes(result.ToJson());
        }

        private byte[] FindRouteLocationsHandler(NameValueCollection boundVariables,
                                                  JsonObject operationInput,
                                                      string outputFormat,
                                                      string requestProperties,
                                                  out string responseProperties)
        {
            responseProperties = null;

            if (!operationInput.TryGetAsLong("layer", out long? layerId))
            {
                throw new ArgumentException("Layer ID not provided");
            }


            bool hasLocations = operationInput.TryGetArray("locations", out object[] locationsArray);
            if (!hasLocations)
            {
                throw new ArgumentException($"Expected \"locations\" to be a JSON array: {operationInput.ToJson()}", "locations");
            }

            var locations = locationsArray.Cast<JsonObject>().ToRouteLocations();

            IRouteLocator2 routeLocator = serverObjectHelper.GetRouteLocator(layerId.GetValueOrDefault(0), routeIdFieldName);

            var located = locations.Select(loc =>
            {
                routeLocator.Locate(loc, out IGeometry result, out esriLocatingError locatingError);
                return new LocationResult
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

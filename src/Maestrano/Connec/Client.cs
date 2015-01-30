using Maestrano.Api;
using Maestrano.Helpers;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Maestrano.Connec
{
    public class Client
    {
        public string ConnecBasePath { get; set; }
        public string ConnecScopedPath { get; set; }
        public string ConnecHost { get; set; }
        public string ApiId { get; set; }
        public string ApiKey { get; set; }

        public RestClient _client;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="groupId">The customer group id</param>
        public Client (string groupId)
        {
            ConnecBasePath = MnoHelper.Connec.BasePath;
            ConnecScopedPath = ConnecBasePath + "/" + groupId;
            ConnecHost = MnoHelper.Connec.Host;
            ApiKey = MnoHelper.Api.Key;
            ApiId = MnoHelper.Api.Id;

            // silverlight friendly way to get current version
            var assembly = Assembly.GetExecutingAssembly();
            AssemblyName assemblyName = new AssemblyName(assembly.FullName);
            var version = assemblyName.Version;

            _client = new RestClient();
            _client.UserAgent = "maestrano-dotnet/" + version;
            _client.Authenticator = new HttpBasicAuthenticator(ApiId, ApiKey);
            _client.BaseUrl = new Uri(String.Format("{0}{1}", ConnecHost, ConnecScopedPath));
            _client.AddDefaultHeader("Accept", "application/vnd.api+json");
            _client.AddDefaultHeader("Content-Type", "application/vnd.api+json");
        }

        /// <summary>
        /// Return a configured RestClient
        /// </summary>
        /// <param name="groupId">The customer group id</param>
        public static RestClient GetRestClient(string groupId) {
            return (RestClient)(new Client(groupId))._client;
        }

        private IRestRequest PrepareRequest(Method method, String path, NameValueCollection parameters, String body)
        {
            var request = new RestRequest();
            request.Resource = path;
            request.Method = method;
            request.AddHeader("Accept", "application/vnd.api+json");
            request.AddHeader("Content-Type", "application/vnd.api+json");
            request.Parameters.Clear();

            if (body != null)
                request.AddParameter("application/vnd.api+json", body, ParameterType.RequestBody);

            if (parameters != null)
            {
                foreach (var k in parameters.AllKeys)
                    request.AddParameter(StringExtensions.ToSnakeCase(k), parameters[k]);
            }

            return request;
        }

        public RestResponse Get(string path)
        {
            return (RestResponse)_client.Execute(PrepareRequest(Method.GET, path, null, null));
        }

        public IRestResponse Get(string path, NameValueCollection parameters)
        {
            return (RestResponse)_client.Execute(PrepareRequest(Method.GET, path, parameters, null));
        }

        public RestResponse<T> Get<T>(string path) where T : new()
        {
            return (RestResponse<T>)_client.Execute<T>(PrepareRequest(Method.GET, path, null, null));
        }

        public RestResponse<T> Get<T>(string path, NameValueCollection parameters) where T : new()
        {
            return (RestResponse<T>)_client.Execute<T>(PrepareRequest(Method.GET, path, parameters, null));
        }



        public IRestResponse Post(string path, string jsonBody)
        {
            return (RestResponse)_client.Execute(PrepareRequest(Method.POST, path, null, jsonBody));
        }

        public RestResponse<T> Post<T>(string path, string jsonBody) where T : new()
        {
            return (RestResponse<T>)_client.Execute<T>(PrepareRequest(Method.POST, path, null, jsonBody));
        }


        public IRestResponse Put(string path, string jsonBody)
        {
            return (RestResponse)_client.Execute(PrepareRequest(Method.PUT, path, null, jsonBody));
        }

        public RestResponse<T> Put<T>(string path, string jsonBody) where T : new()
        {
            return (RestResponse<T>)_client.Execute<T>(PrepareRequest(Method.PUT, path, null, jsonBody));
        }
    }
}

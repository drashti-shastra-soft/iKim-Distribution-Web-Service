using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class HttpClientService : IHttpClientService
    {
        private readonly HttpClient _client;

        public HttpClientService()
        {
            _client = HttpClientProvider.Client;
        }

        public async Task<TR> PostAsync<T, TR>(string url, T obj, string token = null, string apiKey = null)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var json = JsonConvert.SerializeObject(obj);

            using (var request = new HttpRequestMessage(HttpMethod.Post, url))
            using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
            {
                request.Content = content;

                if (token.IsNotNullOrEmpty())
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
                }

                if (apiKey.IsNotNullOrEmpty())
                {
                    request.Headers.Add("X-Api-Key", apiKey);
                }

                var response = await _client.SendAsync(request).ConfigureAwait(false);
                var contentString = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return JsonConvert.DeserializeObject<TR>(contentString);
                }
                else if (response.StatusCode == HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Authorization has been denied for this request.");
                }
                else if (response.StatusCode == HttpStatusCode.BadRequest)
                {
                    throw new Exception(JsonConvert.DeserializeObject<string>(contentString) ?? "Bad Request!");
                }
                else
                {
                    throw new Exception("Internal server error!");
                }
            }
        }
    }
}

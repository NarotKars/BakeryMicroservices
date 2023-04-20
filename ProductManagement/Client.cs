using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ProductManagement
{
    public class Client
    {
        private readonly JsonSerializerOptions options;
        private HttpClient httpClient;
        public Client()
        {
            this.options = new JsonSerializerOptions()
            {
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                PropertyNameCaseInsensitive = true,
            };

            if(httpClient is null)
            {
                this.httpClient = new HttpClient();
            }
        }

        public async Task<Stream> GetStreamAsync(HttpMethod method, string url, object requestObject = null)
        {
            using var request = CreateRequest(method, url, requestObject);
            var response = await this.httpClient.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = response.Content;
                return await content.ReadAsStreamAsync();
            }
            else
            {
                throw new FileNotFoundException();
            }
        }

        private HttpRequestMessage CreateRequest(HttpMethod method, string url, object requestObject, string accept = "application/json", IEnumerable<KeyValuePair<string, string>> headers = null)
        {
            var request = new HttpRequestMessage();
            if (requestObject != null)
            {
                string body = JsonSerializer.Serialize(requestObject, this.options);
                var content = new StringContent(body);
                content.Headers.ContentType.MediaType = "application/json";
                content.Headers.ContentType.CharSet = "utf-8";
                request.Content = content;
            }
            request.Method = method;
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(accept));
            request.RequestUri = new Uri(url);
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }
            return request;
        }
    }
}

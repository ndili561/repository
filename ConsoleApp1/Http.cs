using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApp1
{
  
        /// <inheritdoc />
        public class HttpClientWrapper : IHttpClient
        {
            private HttpClient _httpClient;

            private HttpClient HttpClient
            {
                get
                {
                    if (_httpClient == null)
                    {
                        throw new NullReferenceException("Use CreateHttpClient method before using wrapper.");
                    }

                    return _httpClient;
                }

                set { _httpClient = value; }
            }

            public Uri BaseAddress
            {
                get { return HttpClient.BaseAddress; }
                set { HttpClient.BaseAddress = value; }
            }

            public HttpRequestHeaders DefaultRequestHeaders => HttpClient.DefaultRequestHeaders;
            public long MaxResponseContentBufferSize
            {
                get { return HttpClient.MaxResponseContentBufferSize; }
                set { HttpClient.MaxResponseContentBufferSize = value; }
            }
            public TimeSpan Timeout
            {
                get { return HttpClient.Timeout; }
                set { HttpClient.Timeout = value; }
            }

            public void Dispose()
            {
                if (_httpClient == null)
                {
                    return;
                }

                HttpClient?.Dispose();
                HttpClient = null;
            }

            public IHttpClient CreateHttpClient()
            {
                HttpClient = new HttpClient();

                return this;
            }

            public void CancelPendingRequests()
            {
                HttpClient.CancelPendingRequests();
            }

            public Task<HttpResponseMessage> DeleteAsync(string requestUri)
            {
                return HttpClient.DeleteAsync(requestUri);
            }

            public Task<HttpResponseMessage> DeleteAsync(Uri requestUri)
            {
                return HttpClient.DeleteAsync(requestUri);
            }

            public Task<HttpResponseMessage> DeleteAsync(string requestUri, CancellationToken cancellationToken)
            {
                return HttpClient.DeleteAsync(requestUri, cancellationToken);
            }

            public Task<HttpResponseMessage> DeleteAsync(Uri requestUri, CancellationToken cancellationToken)
            {
                return HttpClient.DeleteAsync(requestUri, cancellationToken);
            }

            public Task<HttpResponseMessage> GetAsync(string requestUri)
            {
                return HttpClient.GetAsync(requestUri);
            }

            public Task<HttpResponseMessage> GetAsync(Uri requestUri)
            {
                return HttpClient.GetAsync(requestUri);
            }

            public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption)
            {
                return HttpClient.GetAsync(requestUri, completionOption);
            }

            public Task<HttpResponseMessage> GetAsync(string requestUri, CancellationToken cancellationToken)
            {
                return HttpClient.GetAsync(requestUri, cancellationToken);
            }

            public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption)
            {
                return HttpClient.GetAsync(requestUri, completionOption);
            }

            public Task<HttpResponseMessage> GetAsync(Uri requestUri, CancellationToken cancellationToken)
            {
                return HttpClient.GetAsync(requestUri, cancellationToken);
            }

            public Task<HttpResponseMessage> GetAsync(string requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
            {
                return HttpClient.GetAsync(requestUri, completionOption, cancellationToken);
            }

            public Task<HttpResponseMessage> GetAsync(Uri requestUri, HttpCompletionOption completionOption, CancellationToken cancellationToken)
            {
                return HttpClient.GetAsync(requestUri, completionOption, cancellationToken);
            }

            public Task<byte[]> GetByteArrayAsync(string requestUri)
            {
                return HttpClient.GetByteArrayAsync(requestUri);
            }

            public Task<byte[]> GetByteArrayAsync(Uri requestUri)
            {
                return HttpClient.GetByteArrayAsync(requestUri);
            }

            public Task<Stream> GetStreamAsync(string requestUri)
            {
                return HttpClient.GetStreamAsync(requestUri);
            }

            public Task<Stream> GetStreamAsync(Uri requestUri)
            {
                return HttpClient.GetStreamAsync(requestUri);
            }

            public Task<string> GetStringAsync(string requestUri)
            {
                return HttpClient.GetStringAsync(requestUri);
            }

            public Task<string> GetStringAsync(Uri requestUri)
            {
                return HttpClient.GetStringAsync(requestUri);
            }

            public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
            {
                return HttpClient.PostAsync(requestUri, content);
            }

            public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content)
            {
                return HttpClient.PostAsync(requestUri, content);
            }

            public Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, content, cancellationToken);
            }

            public Task<HttpResponseMessage> PostAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, content, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content)
            {
                return HttpClient.PostAsync(requestUri, content);
            }

            public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content)
            {
                return HttpClient.PostAsync(requestUri, content);
            }

            public Task<HttpResponseMessage> PutAsync(string requestUri, HttpContent content, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, content, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsync(Uri requestUri, HttpContent content, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, content, cancellationToken);
            }

            public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request)
            {
                return HttpClient.SendAsync(request);
            }

            public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption)
            {
                return HttpClient.SendAsync(request, completionOption);
            }

            public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                return HttpClient.SendAsync(request, cancellationToken);
            }

            public Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpCompletionOption completionOption, CancellationToken cancellationToken)
            {
                return HttpClient.SendAsync(request, completionOption, cancellationToken);
            }
            //
            public Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value)
            {
                return HttpClient.PostAsJsonAsync(requestUri, value);
            }

            public Task<HttpResponseMessage> PostAsJsonAsync<T>(string requestUri, T value, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsJsonAsync(requestUri, value, cancellationToken);
            }

            public Task<HttpResponseMessage> PostAsXmlAsync<T>(string requestUri, T value)
            {
                return HttpClient.PostAsXmlAsync(requestUri, value);
            }

            public Task<HttpResponseMessage> PostAsXmlAsync<T>(string requestUri, T value, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsXmlAsync(requestUri, value, cancellationToken);
            }

            public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, MediaTypeFormatter formatter)
            {
                return HttpClient.PostAsync(requestUri, value, formatter);
            }

            public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, value, formatter, cancellationToken);
            }

            public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, string mediaType)
            {
                return HttpClient.PostAsync(requestUri, value, formatter, mediaType);
            }

            public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType,
                CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, value, formatter, mediaType, cancellationToken);
            }

            public Task<HttpResponseMessage> PostAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, string mediaType,
                CancellationToken cancellationToken)
            {
                return HttpClient.PostAsync(requestUri, value, formatter, mediaType, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value)
            {
                return HttpClient.PutAsJsonAsync(requestUri, value);
            }

            public Task<HttpResponseMessage> PutAsJsonAsync<T>(string requestUri, T value, CancellationToken cancellationToken)
            {
                return HttpClient.PutAsJsonAsync(requestUri, value, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsXmlAsync<T>(string requestUri, T value)
            {
                return HttpClient.PutAsXmlAsync(requestUri, value);
            }

            public Task<HttpResponseMessage> PutAsXmlAsync<T>(string requestUri, T value, CancellationToken cancellationToken)
            {
                return HttpClient.PutAsXmlAsync(requestUri, value, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsync<T>(string requestUri, T value, MediaTypeFormatter formatter)
            {
                return HttpClient.PutAsync(requestUri, value, formatter);
            }

            public Task<HttpResponseMessage> PutAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, CancellationToken cancellationToken)
            {
                return HttpClient.PutAsync(requestUri, value, formatter, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, string mediaType)
            {
                return HttpClient.PutAsync(requestUri, value, formatter, mediaType);
            }

            public Task<HttpResponseMessage> PutAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, string mediaType,
                CancellationToken cancellationToken)
            {
                return HttpClient.PutAsync(requestUri, value, formatter, mediaType, cancellationToken);
            }

            public Task<HttpResponseMessage> PutAsync<T>(string requestUri, T value, MediaTypeFormatter formatter, MediaTypeHeaderValue mediaType,
                CancellationToken cancellationToken)
            {
                return HttpClient.PutAsync(requestUri, value, formatter, mediaType, cancellationToken);
            }
        }
    }

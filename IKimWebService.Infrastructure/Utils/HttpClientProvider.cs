using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.Utils
{
    public class HttpClientProvider
    {
        private static readonly HttpClient _client = new HttpClient();

        public static HttpClient Client => _client;
    }
}

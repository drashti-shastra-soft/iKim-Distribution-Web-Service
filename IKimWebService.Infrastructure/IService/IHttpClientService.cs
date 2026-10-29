using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IHttpClientService
    {
        Task<TR> PostAsync<T, TR>(string url, T obj, string token = null, string apiKey = null);
    }
}

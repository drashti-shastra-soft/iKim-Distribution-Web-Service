using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IKimWebService.Controllers
{
    [Authorize, RoutePrefix("api/StockUpdate")]
    public class StockUpdateController : ApiController
    {
        private readonly IStockUpdateService _service;

        public StockUpdateController(IStockUpdateService service)
        {
            _service = service;
        }

        [HttpPost]
        public HttpResponseMessage Upsert(Domain.StockUpdate mStockUpdate)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mStockUpdate));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

        [HttpGet, Route("GetByItem/{itemId:int}")]
        public HttpResponseMessage GetByItem(int itemId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetBy(itemId));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

    }
}

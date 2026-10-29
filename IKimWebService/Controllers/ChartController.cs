using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IService;
using System.Web.Http;
using System.Net.Http;
using System.Net;
using System;

namespace IKimWebService.Controllers
{
    [Authorize, RoutePrefix("api/Chart")]
    public class ChartController : ApiController
    {
        private readonly IChartService _service;

        public ChartController(IChartService service)
        {
            _service = service;
        }

        [HttpPost, Route("SalesChart/{storeId:int}")]
        public HttpResponseMessage SalesChart(int storeId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.SalesChart(storeId));
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


        [HttpPost, Route("PurchaseChart/{vendorId:int}")]
        public HttpResponseMessage PurchaseChart(int vendorId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.PurchaseChart(vendorId));
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

        [HttpPost, Route("IngredientConsumptionReport/{itemId:int}")]
        public HttpResponseMessage IngredientConsumptionReport(int itemId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.IngredientConsumptionReport(itemId));
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

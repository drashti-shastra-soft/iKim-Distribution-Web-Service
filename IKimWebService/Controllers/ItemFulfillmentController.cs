using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IService;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System;

namespace IKimWebService.Controllers
{
    [Authorize, RoutePrefix("api/ItemFulfillment")]
    public class ItemFulfillmentController : ApiController
    {
        private readonly IItemFulfillmentService _service;

        public ItemFulfillmentController(IItemFulfillmentService service)
        {
            _service = service;
        }

        [HttpPost]
        public HttpResponseMessage Upsert(Domain.ItemFulfillment mItemFulfillment)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mItemFulfillment));
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

        [HttpGet]
        [Route("OrderLineId/{orderLineId:int}")]
        public HttpResponseMessage GetByOrderLineId(int orderLineId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetByOrderLineId(orderLineId));
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

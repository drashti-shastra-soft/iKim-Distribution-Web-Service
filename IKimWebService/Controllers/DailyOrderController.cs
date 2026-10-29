using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http;
using System.Net;
using System;

namespace IKimWebService.Controllers
{
    [Authorize, RoutePrefix("api/DailyOrder")]
    public class DailyOrderController : ApiController
    {
        private readonly IDailyOrderService _service;

        public DailyOrderController(IDailyOrderService service)
        {
            _service = service;
        }

        [HttpPost, Route("DailyOrders")]
        public HttpResponseMessage DailyOrders(Domain.DailyOrder mDailyOrder)
        {
            HttpResponseMessage response = new HttpResponseMessage();

            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.DailyOrders(mDailyOrder));
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

        [HttpPost, Route("GenerateDailyOrderChallan")]
        public HttpResponseMessage GenerateDailyOrderChallan(List<Domain.DailyOrder> dailyOrders)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GenerateDailyOrderChallan(dailyOrders));
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
        // Selected Order Wise
        [HttpPost, Route("GetSelectedDailyOrders")]
        public HttpResponseMessage GetSelectedDailyOrders(List<int> mDailyOrder)
        {
            try
            {
                if (mDailyOrder == null || mDailyOrder.Count == 0)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest,"Please select at least one order." );
                }

                var result = _service.GetSelectedDailyOrders(mDailyOrder);

                return Request.CreateResponse(HttpStatusCode.OK,result);
            }
            catch (APIRequestFailedException ex)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest,ex.Message);
            }
            catch (Exception)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
        }
    }
}
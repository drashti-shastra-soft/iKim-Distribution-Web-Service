using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace IKimWebService.Controllers
{
    [Authorize, RoutePrefix("api/ItemPrice")]
    public class ItemPriceController : ApiController
    {
        private readonly IItemPriceService _service;

        public ItemPriceController(IItemPriceService service)
        {
            _service = service;
        }

        [HttpPost]
        public HttpResponseMessage Upsert(Domain.ItemPrice mItemPrice)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Upsert(mItemPrice));
            }
            catch (APIRequestFailedException ex)
            {
                response = Request.CreateResponse(HttpStatusCode.BadRequest, ex.Message);
            }
            catch (DbUpdateException ex)
            {
                var message = ex.InnerException?.InnerException?.Message ?? ex.Message;
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "DB Update Error: " + message);
            }
            catch (Exception ex)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError, "Unexpected Error: " + ex.Message);
            }
            return response;
        }

        [HttpGet, Route("{id:int}")]
        public HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.Get(id));
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

        [HttpGet, Route("ItemId/{itemId:int}")]
        public HttpResponseMessage GetItemPrices(int itemId)
        {
            HttpResponseMessage response = new HttpResponseMessage();
            try
            {
                response = Request.CreateResponse(HttpStatusCode.OK, _service.GetItemPrices(itemId));
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


        [HttpGet, Route("GetPrice")]
        public HttpResponseMessage GetPrice(int orderId, int itemId)
        {
            HttpResponseMessage response;
            try
            {
                var price = _service.GetByItemPrice(orderId, itemId);
                response = Request.CreateResponse(HttpStatusCode.OK, price);
            }
            catch (Exception)
            {
                response = Request.CreateResponse(HttpStatusCode.InternalServerError);
            }
            return response;
        }

    }
}

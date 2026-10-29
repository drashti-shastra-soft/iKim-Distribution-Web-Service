using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class ItemFulfillment : AuditTrail
    {
        private int _quantityFulfilled;

        public int Id { get; set; }
        public int FulfillmentId { get; set; }
        public int OrderLineId { get; set; }
        public int ItemId { get; set; }
        public int AuditId { get; set; }        
        public int QuantityFulfilled
        {
            get => _quantityFulfilled;
            set
            {
                _quantityFulfilled = value;
                TotalPrice = _quantityFulfilled > 0 ? _quantityFulfilled * Price : QuantityRequested * Price;
            }
        }
        public int QuantityRequested { get; set; }
        public decimal Price { get; set; }
        public decimal TotalPrice { get; private set; }
    }
}

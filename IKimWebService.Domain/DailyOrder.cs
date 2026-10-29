using System;
using System.Collections.Generic;

namespace IKimWebService.Domain
{
    public class DailyOrder
    {
        public int StoreId { get; set; }

        public string Name { get; set; }

        public string ItemName { get; set; }

        public int QuantityRequested { get; set; }

        public string UnitOfMeasure { get; set; }

        public int UnitOfMeasureId { get; set; }

        public DateTime? FromDate { get; set; }

        public DateTime? ToDate { get; set; }

        public int OrderId { get; set; }
    }
}

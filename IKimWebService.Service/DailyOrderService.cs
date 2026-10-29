using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IKimWebService.Service
{
    public class DailyOrderService : IDailyOrderService
    {
        private readonly IDailyOrderRepository _repo;
        private readonly IStoreRepository _Storerepo;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IPDFGeneratorService _pdfGeneratorService;
        private readonly IFileReaderService _fileReaderService;

        public DailyOrderService(IDailyOrderRepository repo, IStoreRepository storerepo, IOrderLineRepository orederlinerepo, IPDFGeneratorService pdfGeneratorService, IFileReaderService fileReaderService)
        {
            _repo = repo;
            _Storerepo = storerepo;
            _orderLineRepository = orederlinerepo;
            _pdfGeneratorService = pdfGeneratorService;
            _fileReaderService = fileReaderService;
        }

        public List<Domain.DailyOrder> DailyOrders(Domain.DailyOrder mDailyOrder)
        {
            return _repo.DailyOrders(mDailyOrder);
        }

        public List<Domain.DailyOrder> GetSelectedDailyOrders(List<int> mDailyOrder)
        {
            if (mDailyOrder == null || !mDailyOrder.Any()) 
                return new List<Domain.DailyOrder>();

            return _repo.GetSelectedDailyOrders(mDailyOrder);
        }

        public string GenerateDailyOrderChallan(List<Domain.DailyOrder> dailyOrders)
        {
            if (dailyOrders == null || !dailyOrders.Any())
                return "";

            string html = PrepareGridHtml(dailyOrders);

            return _pdfGeneratorService.GeneratePDF(html);
        }

        #region Private Methods

        private string PrepareGridHtml(List<Domain.DailyOrder> mDailyOrders)
        {
            string template = _fileReaderService.ReadFileContent("~/PDFTemplates/DailyOrder.html");

            var stores = mDailyOrders.GroupBy(x => new { x.OrderId, x.StoreId, x.Name })
            .Select(x => new
            {
                OrderId = x.Key.OrderId,
                StoreId = x.Key.StoreId,
                StoreName = x.Key.Name,
                Items = x.OrderBy(i => i.ItemName).ToList()
            }).ToList();

            const int storesPerTable = 4;

            StringBuilder finalHtml = new StringBuilder();

            for (int start = 0; start < stores.Count; start += storesPerTable)
            {
                var currentStores = stores.Skip(start).Take(storesPerTable).ToList();
                StringBuilder header = new StringBuilder();
                foreach (var store in currentStores)
                {
                    string storeName = store.StoreName.IsNullOrEmpty() ? string.Empty : store.StoreName;

                    header.Append($@" <th style='width:25%; border:1px solid #000; color:#000; padding:7px; font-family:Arial; font-size:11px; font-weight:bold; text-align:center; vertical-align:middle; box-sizing:border-box; '>
                    {storeName} </th>");
                }

                for (int i = currentStores.Count; i < storesPerTable; i++)
                {
                    header.Append(@" <th style='width:25%;border:none;padding:0;'></th>");
                }

                int maxRows = currentStores.Max(x => x.Items.Count);
                StringBuilder body = new StringBuilder();
                for (int r = 0; r < maxRows; r++)
                {
                    body.Append("<tr>");
                    foreach (var store in currentStores)
                    {
                        if (r < store.Items.Count)
                        {
                            var item = store.Items[r];
                            string itemName = item.ItemName.IsNullOrEmpty() ? string.Empty : item.ItemName;
                            body.Append($@"<td style='width:25%;border:1px solid #000;padding:4px;vertical-align:top;box-sizing:border-box;'>
                                           <table style='width:100%; border-collapse:collapse; table-layout:fixed;'><tr>
                                           <td style='width:72%; border:none; padding:0; font-family:Arial; font-size:10px; text-align:left; vertical-align:top; word-wrap:break-word; '> {itemName} </td>

                                    <td style='width:28%; border:none; padding:0; font-family:Arial; font-size:10px; font-weight:bold; text-align:right; vertical-align:top;white-space:nowrap;'> {item.QuantityRequested} {item.UnitOfMeasure}
                                    </td>
                                </tr>
                            </table>
                        </td>");
                        }
                        else
                        {
                            body.Append(@"<td style='width:25%;border:1px solid #000;padding:4px;'></td>");
                        }
                    }

                    for (int i = currentStores.Count; i < storesPerTable; i++)
                    {
                        body.Append(@"<td style='width:25%;border:none;padding:0;'></td>");
                    }
                    body.Append("</tr>");
                }
                finalHtml.Append($@" <table style='width:100%; border-collapse:collapse; table-layout:fixed; margin-bottom:20px;'>
                    <thead>
                        <tr>
                            {header}
                        </tr>
                    </thead>

                    <tbody>
                        {body}
                    </tbody>
            </table> ");
            }

            template = template.Replace("{{Tables}}", finalHtml.ToString());

            return template;
        }
        #endregion
    }
}
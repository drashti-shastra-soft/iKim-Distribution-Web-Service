using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System;
using IKimWebService.Infrastructure.Utils;

namespace IKimWebService.Service
{
    public class OrderService : IOrderService
    {
        #region Declaration

        private readonly IOrderRepository _repo;
        private readonly IOrderLineRepository _orderLineRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IAddressRepository _addressRepository;
        private readonly ICartItemRepository _cartItemRepository;
        private readonly IFileReaderService _fileReaderService;
        private readonly IPDFGeneratorService _pdfGeneratorService;

        public OrderService(IOrderRepository repo, IOrderLineRepository orderLineRepository, IAddressRepository addressRepository, IStoreRepository storeRepository, IFileReaderService fileReaderService, IPDFGeneratorService pdfGeneratorService, ICartItemRepository cartItemRepository)
        {
            _repo = repo;
            _orderLineRepository = orderLineRepository;
            _storeRepository = storeRepository;
            _addressRepository = addressRepository;
            _fileReaderService = fileReaderService;
            _pdfGeneratorService = pdfGeneratorService;
            _cartItemRepository = cartItemRepository;
        }

        #endregion

        #region Public Method
        public Domain.OrderLister GetAll(Domain.OrderLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.Order Upsert(Domain.Order mOrder)
        {
            return _repo.Upsert(mOrder);
        }

        public Domain.Order Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        #endregion

        #region Generate Delivery Challan
        public string GenerateDeliveryChallan(List<Domain.Order> orders)
        {
            var mBase64Strings = new List<string>();

            foreach (var order in orders)
            {
                // Get Order
                var mOrder = _repo.Get(order.Id);

                if (mOrder == null || mOrder.Id <= 0)
                    continue;

                // Get Order Line
                mOrder.Lines = _orderLineRepository.GetBy(order.Id);

                // Get Store
                var mStore = _storeRepository.Get(mOrder.StoreId);

                // Get Store Address
                var mStoreAddress = _addressRepository.GetByStoreId(mStore.AddressId ?? 0);

                // Replace HTML PlaceHolders
                var pdfContain = PrepareDeliveryChallanPDF(mStoreAddress, mStore, mOrder);

                // Generate individual PDF (Base64)
                var base64String = _pdfGeneratorService.GeneratePDF(pdfContain);
                mBase64Strings.Add(base64String);
            }

            // Merge all PDFs into one Base64 string
            if (mBase64Strings != null && mBase64Strings.Count > 0)
            {
                return _pdfGeneratorService.MergeBase64Pdfs(mBase64Strings);
            }
            else
            {
                throw new APIRequestFailedException("No valid orders found to generate delivery challan.");
            }
        }
        #endregion

        #region Private Methods
        private string PrepareDeliveryChallanPDF(Domain.Address mAddress, Domain.Store mStore, Domain.Order mOrder)
        {
            string content = _fileReaderService.ReadFileContent("~/PDFTemplates/DeliveryChallan.html");
            return PopulateDeliveryChallan(content, mAddress, mStore, mOrder);
        }

        private string PopulateDeliveryChallan(string template, Domain.Address mAddress, Domain.Store mStore, Domain.Order mOrder)
        {
            string lineItems = PrepareLineItems(mOrder.Lines);

            decimal totalAmount = 0;
            decimal totalGST5 = 0;
            decimal totalGST18 = 0;

            foreach (var line in mOrder.Lines)
            {
                //decimal quantity = line.FulfilledQuantity > 0 ? line.FulfilledQuantity : line.QuantityRequested;
                //decimal amount = quantity * line.Rate;
                //totalAmount += amount;

                decimal amount = line.FulfilledQuantity * line.Rate;
                totalAmount += amount;

                decimal gstAmount = amount * (line.CGST + line.SGST) / 100;
                if ((line.CGST + line.SGST) == 5)
                {
                    totalGST5 += gstAmount;
                }
                else if ((line.CGST + line.SGST) == 18)
                {
                    totalGST18 += gstAmount;
                }
            }

            decimal grandTotal = totalAmount + totalGST5 + totalGST18;
            var replacements = new Dictionary<string, string>
            {
                {"{{GeneratedDate}}", mOrder.Date.ToString("dd-MMM-yyyy")},
                {"{{StoreName}}", mStore.Name},
                {"{{Address}}", $"{mAddress.Line1}, {mAddress.Line2}, {mAddress.Line3}"},
                {"{{PhoneNumber}}", mStore.PhoneNumber},
                {"{{LineItems}}", lineItems},

                {"{{Amount}}", totalAmount.ToString("#,##0.00")},
                {"{{GST5}}", totalGST5.ToString("#,##0.00")},
                {"{{GST18}}", totalGST18.ToString("#,##0.00")},
                {"{{Total}}", grandTotal.ToString("#,##0.00")}
            };

            foreach (var pair in replacements)
            {
                template = template.Replace(pair.Key, pair.Value);
            }

            return template;
        }

        private string PrepareLineItems(List<Domain.OrderLine> mLines)
        {
            StringBuilder sb = new StringBuilder();

            int srNo = 1;

            foreach (var line in mLines)
            {
                //decimal quantity = line.FulfilledQuantity > 0 ? line.FulfilledQuantity : line.QuantityRequested;
                //decimal amount = quantity * line.Rate;
                decimal amount = line.FulfilledQuantity * line.Rate;
                sb.AppendLine("<tr>");
                sb.AppendLine(BuildCell(line.Item));
                sb.AppendLine(BuildCell($"{line.QuantityRequested:#,##0} {line.UnitOfMeasure}", "center"));
                sb.AppendLine(BuildCell($"{line.FulfilledQuantity:#,##0} {line.UnitOfMeasure}", "center"));
                sb.AppendLine(BuildCell(line.Rate.ToString("#,##0.00"), "right"));
                sb.AppendLine(BuildCell(amount.ToString("#,##0.00"), "right"));
                sb.AppendLine("</tr>");

                srNo++;
            }

            return sb.ToString();
        }

        private string BuildCell(string value, string align = "left")
        {
            return $@"<td style='border:1px solid #000; font-size:9px; font-family:Arial; text-align:{align}; vertical-align:middle;'> {value} </td>";
        }
        #endregion

        #region Place Order

        public Domain.Order PlaceOrder(Domain.Order mOrder)
        {
            if (mOrder == null)
                throw new APIRequestFailedException("Object must have a value!");

            var mCartItems = _cartItemRepository.GetAll(mOrder.StoreId);

            if (mCartItems == null || mCartItems.Count == 0)
            {
                throw new APIRequestFailedException("Cart is empty.");
            }

            mOrder.Lines = ConvertCartToLine(mCartItems);
            mOrder.Date = DateTime.Now;

            CalculateAmount(mOrder);

            mOrder = _repo.Upsert(mOrder);

            if (mOrder.Id != 0)
            {
                foreach (var mOrderLine in mOrder.Lines)
                {
                    mOrderLine.OrderId = mOrder.Id;
                    mOrderLine.CreatedBy = mOrder.CreatedBy;
                    mOrderLine.LastModifiedBy = mOrder.LastModifiedBy;
                    _orderLineRepository.Upsert(mOrderLine);
                    _cartItemRepository.Delete(mOrderLine.CartItemId);
                }
            }
            return mOrder;
        }

        private List<Domain.OrderLine> ConvertCartToLine(List<Domain.CartItem> mCartItems)
        {
            var mOrderLines = new List<Domain.OrderLine>();
            foreach (var item in mCartItems)
            {
                var mOrderLine = new Domain.OrderLine();
                mOrderLine.ItemId = item.ItemId;
                mOrderLine.UnitOfMeasureId = item.UnitOfMeasureId;
                mOrderLine.QuantityRequested = item.Quantity;
                mOrderLine.Rate = item.Rate;
                mOrderLine.Amount = item.Amount;
                mOrderLine.CartItemId = item.Id;

                mOrderLines.Add(mOrderLine);
            }
            return mOrderLines;
        }

        private void CalculateAmount(Domain.Order mOrder)
        {
            mOrder.Amount = mOrder.Lines.Sum(line => line.Amount);
        }

        #endregion
    }
}

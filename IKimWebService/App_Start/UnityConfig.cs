using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Repository;
using IKimWebService.Service;
using System.Web.Http;
using Unity;
using Unity.WebApi;

namespace IKimWebService
{
    public static class UnityConfig
    {
        public static void RegisterComponents()
        {
			var container = new UnityContainer();

            // register all your components with the container here
            // it is NOT necessary to register your controllers

            // e.g. container.RegisterType<ITestService, TestService>();

            //Service
            container.RegisterType<IAddressService, AddressService>();
            container.RegisterType<IAuthenticationService, AuthenticationService>();
            container.RegisterType<IBannerService, BannerService>();
            container.RegisterType<ICartItemService, CartItemService>();
            container.RegisterType<ICategoryService, CategoryService>();
            container.RegisterType<IChangePasswordService, ChangePasswordService>();
            container.RegisterType<IChartService, ChartService>();
            container.RegisterType<IDailySalesService, DailySalesService>();
            container.RegisterType<IDailyOrderService, DailyOrderService>();
            container.RegisterType<IEmailSenderService, EmailSenderService>();
            container.RegisterType<IFileReaderService, FileReaderService>();
            container.RegisterType<IForgotPasswordService, ForgotPasswordService>();
            container.RegisterType<IHttpClientService, HttpClientService>();
            container.RegisterType<IItemFulfillmentService, ItemFulfillmentService>();
            container.RegisterType<IItemPriceService, ItemPriceService>();
            container.RegisterType<IItemService, ItemService>();
            container.RegisterType<IJWTokenService, JWTokenService>();
            container.RegisterType<INotificationService, NotificationService>();
            container.RegisterType<IOrderLineService, OrderLineService>();
            container.RegisterType<IOrderService, OrderService>();
            container.RegisterType<IPartyOrderLineService, PartyOrderLineService>();
            container.RegisterType<IPartyOrderService, PartyOrderService>();
            container.RegisterType<IPDFGeneratorService, PDFGeneratorService>();
            container.RegisterType<IPricingTierService, PricingTierService>();
            container.RegisterType<IPurchaseItemService, PurchaseItemService>();
            container.RegisterType<IPurchaseService, PurchaseService>();
            container.RegisterType<IStockItemService, StockItemService>();
            container.RegisterType<IStockReportService, StockReportService>();
            container.RegisterType<IStockService, StockService>();
            container.RegisterType<IStockTransferService, StockTransferService>();
            container.RegisterType<IStockUpdateService, StockUpdateService>();
            container.RegisterType<IStoreService, StoreService>();
            container.RegisterType<IStoreTypeService, StoreTypeService>();
            container.RegisterType<IUnitOfMeasureService, UnitOfMeasureService>();
            container.RegisterType<IUserService, UserService>();
            container.RegisterType<IUserTypeService, UserTypeService>();
            container.RegisterType<IVendorService, VendorService>();


            //Repository
            container.RegisterType<IAddressRepository, AddressRepository>();
            container.RegisterType<IBannerRepository, BannerRepository>();
            container.RegisterType<ICartItemRepository, CartItemRepository>();
            container.RegisterType<ICategoryRepository, CategoryRepository>();
            container.RegisterType<IChartRepository, ChartRepository>();
            container.RegisterType<IDailyOrderRepository, DailyOrderRepository>();
            container.RegisterType<IDailySalesRepository, DailySalesRepository>();
            container.RegisterType<IItemFulfillmentRepository, ItemFulfillmentRepository>();
            container.RegisterType<IItemPriceRepository, ItemPriceRepository>();
            container.RegisterType<IItemRepository, ItemRepository>();
            container.RegisterType<INotificationRepository, NotificationRepository>();
            container.RegisterType<IOrderLineRepository, OrderLineRepository>();
            container.RegisterType<IOrderRepository, OrderRepository>();
            container.RegisterType<IPartyOrderLineRepository, PartyOrderLineRepository>();
            container.RegisterType<IPartyOrderRepository, PartyOrderRepository>();
            container.RegisterType<IPricingTierRepository, PricingTierRepository>();
            container.RegisterType<IPurchaseItemRepository, PurchaseItemRepository>();
            container.RegisterType<IPurchaseRepository, PurchaseRepository>();
            container.RegisterType<IStockItemRepository, StockItemRepository>();
            container.RegisterType<IStockReportRepository, StockReportRepository>();
            container.RegisterType<IStoreRepository, StoreRepository>();
            container.RegisterType<IStockTransferRepository, StockTransferRepository>();
            container.RegisterType<IStockItemRepository, StockItemRepository>();
            container.RegisterType<IStockUpdateRepository, StockUpdateRepository>();
            container.RegisterType<IStockRepository, StockRepository>();
            container.RegisterType<IStoreTypeRepository, StoreTypeRepository>();
            container.RegisterType<IUnitOfMeasureRepository, UnitOfMeasureRepository>();
            container.RegisterType<IUserRepository, UserRepository>();
            container.RegisterType<IUserTypeRepository, UserTypeRepository>();
            container.RegisterType<IVendorRepository, VendorRepository>();

            GlobalConfiguration.Configuration.DependencyResolver = new UnityDependencyResolver(container);

        }
    }
}
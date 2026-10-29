using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Web.Hosting;

namespace IKimWebService.Service
{
    public class CartItemService : ICartItemService
    {
        private readonly ICartItemRepository _repo;

        public CartItemService(ICartItemRepository repo)
        {
            _repo = repo;
        }

        public List<Domain.CartItem> GetAll(int storeId)
        {
            var mCartItems = _repo.GetAll(storeId);

            if (mCartItems != null && mCartItems.Count > 0)
            {
                foreach (var mCartItem in mCartItems)
                {
                    if (mCartItem.FileName.IsNullOrEmpty())
                        mCartItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                    else
                        mCartItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Item/{mCartItem.FileName}"));
                }
            }

            return mCartItems;
        }

        public void Upsert(List<Domain.CartItem> mCartItems)
        {
            foreach (var mCartItem in mCartItems)
            {
                mCartItem.Amount = mCartItem.Quantity * mCartItem.Rate;
                _repo.Upsert(mCartItem);
            }
        }

        public Domain.CartItem Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id)
        {
            _repo.Delete(id);
        }

        public int Count(int storeId)
        {
            return _repo.Count(storeId);
        }
    }
}

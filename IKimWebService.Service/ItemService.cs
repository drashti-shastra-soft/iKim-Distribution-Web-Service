using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System.Collections.Generic;
using System.Web.Hosting;
using System.IO;
using System;

namespace IKimWebService.Service
{
    public class ItemService : IItemService
    {
        private readonly IItemRepository _repo;
        private readonly IItemPriceRepository _priceRepository;

        public ItemService(IItemRepository repo, IItemPriceRepository priceRepository)
        {
            _repo = repo;
            _priceRepository = priceRepository;
        }

        public Domain.ItemLister GetAll(Domain.ItemLister mLister)
        {
            mLister = _repo.GetAll(mLister);

            if (mLister != null && mLister.List != null && mLister.List.Count > 0)
            {
                foreach (var mItem in mLister.List)
                {
                    if (mItem.FileName.IsNullOrEmpty())
                        mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Images/No-Image-Found.png"));
                    else
                        mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Item/{mItem.FileName}"));
                }
            }
            return mLister;
        }

        public Domain.Item Upsert(Domain.Item mItem)
        {
            try
            {
                if (mItem == null)
                    throw new APIRequestFailedException("Object must have the value");

                if (!string.IsNullOrEmpty(mItem.Base64String))
                {
                    string relativeFolder = "~/Uploads/Item/";
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(mItem.FileName);
                    mItem.FileName = uniqueFileName;
                    StaticMethods.SaveBase64Image( mItem.Base64String, relativeFolder, uniqueFileName );
                }
                mItem = _repo.Upsert(mItem);

                if (mItem.Id != 0 && mItem.Prices != null)
                {
                    foreach (var itemPrice in mItem.Prices)
                    {
                        itemPrice.ItemId = mItem.Id;
                        _priceRepository.Upsert(itemPrice);
                    }
                }
            }
            catch (Exception ex)
            {
                System.IO.File.AppendAllText(@"C:\temp\error.txt", DateTime.Now + "\n" + ex.ToString() + "\n\n");
                throw new APIRequestFailedException(ex.ToString());
            }
            return mItem;
        }

        public Domain.Item Get(int id)
        {
            var mItem = _repo.Get(id);

            mItem.Prices = _priceRepository.GetByItemId(id);

            if (mItem != null)
            {
                if (mItem.FileName.IsNullOrEmpty())
                {
                    mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                }
                else
                {
                    string imagePath = HostingEnvironment.MapPath($"~/Uploads/Item/{mItem.FileName}");
                    if (File.Exists(imagePath))
                    {
                        mItem.Base64String = StaticMethods.GetImageBase64(imagePath);
                    }
                    else
                    {
                        mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                    }
                }
            }
            return mItem;
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Item> GetAllActiveItems(int priceTierId, int categoryId)
        {
            var mItem = _repo.GetAllActiveItems(priceTierId, categoryId);

            if (mItem != null && mItem != null && mItem.Count > 0)
            {
                foreach (var mItems in mItem)
                {
                    if (mItems.FileName.IsNullOrEmpty())
                        mItems.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Images/No-Image-Found.png"));
                    else
                        mItems.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Item/{mItems.FileName}"));
                }
            }

            return mItem;
        }

        public List<Domain.Item> GetAll(int priceTierId)
        {
            var mItem = _repo.GetAll(priceTierId);

            if (mItem != null && mItem != null && mItem.Count > 0)
            {
                foreach (var mItems in mItem)
                {
                    if (mItems.FileName.IsNullOrEmpty())
                        mItems.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Images/No-Image-Found.png"));
                    else
                        mItems.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Item/{mItems.FileName}"));
                }
            }
            return mItem;
        }

        public List<Domain.Item> GetActiveByIsRawMaterial()
        {
            return _repo.GetActiveByIsRawMaterial();
        }
    }
}

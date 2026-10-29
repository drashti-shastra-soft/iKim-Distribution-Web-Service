using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace IKimWebService.Service
{
    public class BannerService : IBannerService
    {
        private readonly IBannerRepository _repo;

        public BannerService(IBannerRepository repo)
        {
            _repo = repo;
        }

        public Domain.BannerLister GetAll(Domain.BannerLister mLister)
        {
            mLister = _repo.GetAll(mLister);

            if (mLister != null && mLister.List != null && mLister.List.Count > 0)
            {
                foreach (var item in mLister.List)
                {
                    item.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Banner/{item.FileName}"));
                }
            }

            return mLister;
        }

        public Domain.Banner Create(Domain.Banner mBanner)
        {
            try
            {
                if (mBanner == null)
                    throw new Infrastructure.CustomException.APIRequestFailedException("Object must have the value");

                if (mBanner.Base64String.IsNullOrEmpty())
                    throw new Infrastructure.CustomException.APIRequestFailedException("Image is Required!");

                string relativeFolder = "~/Uploads/Banner/";
                string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(mBanner.FileName);
                mBanner.FileName = uniqueFileName;
                StaticMethods.SaveBase64Image(mBanner.Base64String, relativeFolder, uniqueFileName);
            }
            catch (Exception ex)
            {
                // Log full details including inner exception
                System.IO.File.AppendAllText(@"C:\temp\error.txt", DateTime.Now + "\n" + ex.ToString() + "\n\n");
                throw new APIRequestFailedException(ex.ToString());
            }

            return _repo.Create(mBanner);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Banner> GetAll()
        {
            var mBanners = _repo.GetAll();

            if (mBanners != null && mBanners != null && mBanners.Count > 0)
            {
                foreach (var item in mBanners)
                {
                    item.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Banner/{item.FileName}"));
                }
            }

            return mBanners;
        }

    }
}

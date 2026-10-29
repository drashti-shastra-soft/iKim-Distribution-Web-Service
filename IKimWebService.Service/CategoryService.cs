using IKimWebService.Infrastructure.CustomException;
using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using IKimWebService.Infrastructure.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Hosting;

namespace IKimWebService.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _repo;


        public CategoryService(ICategoryRepository repo)
        {
            _repo = repo;
        }

        public Domain.CategoryLister GetAll(Domain.CategoryLister mLister)
        {
            mLister = _repo.GetAll(mLister);

            if (mLister != null && mLister.List != null && mLister.List.Count > 0)
            {
                foreach (var mItem in mLister.List)
                {
                    if (mItem.FileName.IsNullOrEmpty())
                        mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Images/No-Image-Found.png"));
                    else
                        mItem.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Category/{mItem.FileName}"));
                }
            }

            return mLister;
        }

        public Domain.Category Upsert(Domain.Category mCategory)
        {
            try
            {
                if (mCategory == null)
                    throw new APIRequestFailedException("Object must have the value");

                if (mCategory.Id > 0 && string.IsNullOrEmpty(mCategory.Base64String))
                {
                    var efCategory = _repo.Get(mCategory.Id);

                    if (efCategory != null)
                    {
                        mCategory.FileName = efCategory.FileName;
                        mCategory.MimeType = efCategory.MimeType;
                    }
                }

                if (!string.IsNullOrEmpty(mCategory.Base64String))
                {
                    string relativeFolder = "~/Uploads/Category/";
                    string uniqueFileName = Guid.NewGuid().ToString() + Path.GetExtension(mCategory.FileName);
                    mCategory.FileName = uniqueFileName;
                    StaticMethods.SaveBase64Image( mCategory.Base64String, relativeFolder, uniqueFileName);
                }

                return _repo.Upsert(mCategory);
            }
            catch (Exception ex)
            {
                File.AppendAllText( @"C:\temp\error.txt", DateTime.Now + "\n" + ex + "\n\n");
                throw new APIRequestFailedException(ex.ToString());
            }
        }

        public Domain.Category Get(int id)
        {
            var mCategory = _repo.Get(id);

            if (mCategory != null)
            {
                if (string.IsNullOrEmpty(mCategory.FileName))
                {
                    mCategory.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                }
                else
                {
                    string path = HostingEnvironment.MapPath($"~/Uploads/Category/{mCategory.FileName}");

                    if (File.Exists(path))
                        mCategory.Base64String = StaticMethods.GetImageBase64(path);
                    else
                        mCategory.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath("~/Uploads/Images/No-Image-Found.png"));
                }
            }
            return mCategory;
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.Category> GetAll()
        {
            var mCategory = _repo.GetAll();

            if (mCategory != null && mCategory != null && mCategory.Count > 0)
            {
                foreach (var mCategories in mCategory)
                {
                    if (mCategories.FileName.IsNullOrEmpty())
                        mCategories.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Images/No-Image-Found.png"));
                    else
                        mCategories.Base64String = StaticMethods.GetImageBase64(HostingEnvironment.MapPath($"~/Uploads/Category/{mCategories.FileName}"));
                }
            }
            return mCategory;
        }
    }
}
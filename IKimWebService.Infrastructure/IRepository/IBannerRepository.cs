using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IRepository
{
    public interface IBannerRepository
    {
        Domain.BannerLister GetAll(Domain.BannerLister mLister);

        Domain.Banner Create(Domain.Banner mBanner);

        void Delete(int id, int loginUserId);

        List<Domain.Banner> GetAll();
    }
}

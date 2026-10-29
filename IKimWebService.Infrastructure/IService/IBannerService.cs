using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IBannerService
    {
        Domain.BannerLister GetAll(Domain.BannerLister mLister);

        Domain.Banner Create(Domain.Banner mBanner);

        void Delete(int id, int loginUserId);

        List<Domain.Banner> GetAll();
    }
}

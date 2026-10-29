using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IUnitOfMeasureService
    {
        Domain.UnitOfMeasureLister GetAll(Domain.UnitOfMeasureLister mLister);

        Domain.UnitOfMeasure Upsert(Domain.UnitOfMeasure mUnitOfMeasure);

        Domain.UnitOfMeasure Get(int id);

        void Delete(int id, int loginUserId);

        List<Domain.UnitOfMeasure> GetActiveUnitOfMeasure();
    }
}

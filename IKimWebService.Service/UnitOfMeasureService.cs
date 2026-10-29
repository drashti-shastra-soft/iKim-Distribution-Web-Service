using IKimWebService.Infrastructure.IRepository;
using IKimWebService.Infrastructure.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Service
{
    public class UnitOfMeasureService : IUnitOfMeasureService
    {
        private readonly IUnitOfMeasureRepository _repo;

        public UnitOfMeasureService(IUnitOfMeasureRepository repo)
        {
            _repo = repo;
        }

        public Domain.UnitOfMeasureLister GetAll(Domain.UnitOfMeasureLister mLister)
        {
            return _repo.GetAll(mLister);
        }

        public Domain.UnitOfMeasure Upsert(Domain.UnitOfMeasure mUnitOfMeasure)
        {
            return _repo.Upsert(mUnitOfMeasure);
        }

        public Domain.UnitOfMeasure Get(int id)
        {
            return _repo.Get(id);
        }

        public void Delete(int id, int loginUserId)
        {
            _repo.Delete(id, loginUserId);
        }

        public List<Domain.UnitOfMeasure> GetActiveUnitOfMeasure()
        {
            return _repo.GetActiveUnitOfMeasure();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Infrastructure.IService
{
    public interface IChartService
    {
        List<Domain.Data> SalesChart(int storeId);

        List<Domain.Data> PurchaseChart(int vendorId);

        List<Domain.Data> IngredientConsumptionReport(int itemId);

    }
}

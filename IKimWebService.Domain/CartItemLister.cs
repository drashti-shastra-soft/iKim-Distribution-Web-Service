using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IKimWebService.Domain
{
    public class CartItemLister
    {
        public List<CartItem> List { get; set; } = new List<CartItem>();

        public CartItem SearchCriteria { get; set; } = new CartItem();

        public Pagination Pagination { get; set; } = new Pagination();
    }
}

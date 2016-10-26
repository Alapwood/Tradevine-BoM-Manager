using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradevine_BoM_Manager
{
    class BoMItem
    {
        private string productCode;

        private int quantity;

        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; }
        }

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public BoMItem()
        {
            productCode = "";
            quantity = -1;
        }

        public BoMItem(string pc, int q)
        {
            productCode = pc;
            quantity = q;
        }

        public override string ToString()
        {
            return productCode+" ("+quantity+")";
        }
    }
}

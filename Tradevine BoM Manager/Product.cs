using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tradevine_BoM_Manager
{
    class Product
    {
        private string productCode;

        private List<BoMItem> bomItems;

        public string ProductCode
        {
            get { return productCode; }
            set { productCode = value; }
        }

        internal List<BoMItem> BomItems
        {
            get { return bomItems; }
            set { bomItems = value; }
        }

        public Product()
        {
            productCode = "";
            bomItems = new List<BoMItem>();
        }

        public Product(string pc)
        {
            productCode = pc;
            bomItems = new List<BoMItem>();
        }

        public Product(string pc, List<BoMItem> bom)
        {
            productCode = pc;
            bomItems = bom;
        }

        public override string ToString()
        {
            return productCode;
        }
    }
}

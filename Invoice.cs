using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XML_read
{
    class Invoice
    {
        public List<Product> products = null;
        public Details details = null;
        public Seller seller = null;
        internal Details ivoiceDate = null;
        internal Details paymentDueDate = null;
        internal Summary summary = null;
    }
}

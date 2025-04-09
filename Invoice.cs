using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infotols
{
    public class Invoice
    {
        public int InvoiceId { get; set; }
        public int ClientId { get; set; }
        public DateTime InvoiceDate { get; set; }
        public decimal Total { get; set; }
        public string Status { get; set; }

        public List<InvoiceLine> InvoiceLines { get; set; }
    }
}

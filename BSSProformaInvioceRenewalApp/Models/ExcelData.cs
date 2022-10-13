using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BSSProformaInvioceRenewalApp.Models
{
    public class ExcelData
    {

        public int RowNo { get; set; }

        public string? AccountId { get; set; }

        public string? AccountDescription { get; set; }

        public string? BillToAccount { get; set; }

        public string? Name { get; set; }

        public string? Status { get; set; }

        public double? Quantity { get; set; }

        public double? UnitPrice { get; set; }

        public string? BillingCycle { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? ProductName { get; set; }

        public string? ProductId { get; set; }

    }
}

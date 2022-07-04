using System.Linq;

namespace QNE.Models.ViewModel
{
    public class SubscriptionPlan
    {
        public string Title { get; set; }
        public string SubTitle { get; set; }

        public bool Active { get; set; }

        public decimal Price { get; set; }

        public int NOU { get; set; }

        public int InvoiceCount { get; set; }
        public int BillCount { get; set; }

        public int BankReconCount { get; set; }

        public int OcrCount { get; set; } = 30;


        public int[] ModuleValues1 { get; set; }
        public int[] ModuleValues2 { get; set; }
        public int[] ModuleValues3 { get; set; }

        public bool IsModuleEnabled(int[] modules, int activeModule)
        {
            if (modules == null) return false;
            return modules.Contains(activeModule);
        }
    }
}

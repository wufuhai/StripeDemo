using System;

namespace QNE.Models.ViewModel
{
    public class SubscriptionInfoViewModel
    {
        public string Status { get; set; } = "Active";
        public string Plan { get; set; } = "FREE";

        public string ProductKey { get; set; }
        public long Start { get; set; }

        public long End { get; set; }

        public int InvoiceCount { get; set; }
        public int BillCount { get; set; }

        public int BankReconCount { get; set; }

        public int OcrCount { get; set; }

        public int Nou { get; set; } = 1;

        public DateTime GetExpiry()
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(End).AddHours(8).Date;
        }
    }
}

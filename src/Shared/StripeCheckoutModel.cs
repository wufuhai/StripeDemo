using System.ComponentModel.DataAnnotations;

namespace StripeDemo.Shared
{
    public class StripeCheckoutModel
    {
        [Required]
        [StringLength(100)]
        public string PlanKey { get; set; } = "cloudaccounting-starter";
    }

    public class StripeCheckoutResultModel
    {
        public string RedirectUrl { get; set; }
    }
}

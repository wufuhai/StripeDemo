using Microsoft.AspNetCore.Components;
using QNE.Models.ViewModel;
using System;
using System.Threading.Tasks;

namespace QNE.App.Pages.Account
{
    public partial class SubscriptionDetails
    {
        private string _converedPeriod = "";
        private SubscriptionInfoViewModel _subscriptionInfo = new SubscriptionInfoViewModel();
        [Parameter]
        public CompanyListViewModel Company { get; set; }

        [Parameter]
        public SubscriptionPlan Plan { get; set; }

        [CascadingParameter(Name = "Parent")]
        public ManagePlan Parent { get; set; }

        protected async override Task OnInitializedAsync()
        {
            var info = new SubscriptionInfoViewModel() { };//await Service.Http.GetAsync<SubscriptionInfoViewModel>("api/Subscription/Details", new { tid = Company.TenantId }, true);
            if (info != null)
            {
                _subscriptionInfo = info;

                _converedPeriod = $"{DateTimeOffset.FromUnixTimeMilliseconds(info.Start):d} ~ {DateTimeOffset.FromUnixTimeMilliseconds(info.End):d}";
            }

            await base.OnInitializedAsync();
        }

    }
}
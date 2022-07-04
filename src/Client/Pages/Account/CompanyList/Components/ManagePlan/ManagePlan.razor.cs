using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using QNE.Models.Dto;
using QNE.Models.ViewModel;
using System.Threading.Tasks;

namespace QNE.App.Pages.Account
{
    public partial class ManagePlan
    {
        private bool _showCancelPopUp = false;
        private CompanyDto[] _biller_list = { };
        private SubscriptionPlan _currentPlan = new SubscriptionPlan
        {
            Title = "FREE",
            SubTitle = "no hidden charges",
            NOU = 1,
            InvoiceCount = 120,
            BillCount = 72,
            BankReconCount = 250,
            ModuleValues1 = new[] { 1, 2, 4, 512 }
        };

        [Parameter]
        public bool UpgradeMode { get; set; }
        [Parameter]
        public CompanyListViewModel Company { get; set; }

        [Inject] IJSRuntime JS { get; set; }

        public void SwithUpgradMode(bool show)
        {
            UpgradeMode = show;
            StateHasChanged();
        }
        public async Task TriggerCancelPopUp(bool show)
        {
            await JS.InvokeVoidAsync("dx.toggleBodyScroll", !show);
            _showCancelPopUp = show;
            StateHasChanged();
        }
    }
}
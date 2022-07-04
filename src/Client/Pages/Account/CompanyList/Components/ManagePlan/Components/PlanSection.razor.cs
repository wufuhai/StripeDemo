using Microsoft.AspNetCore.Components;
using QNE.App.Client.ClientServices;
using QNE.Models.Dto;
using QNE.Models.ViewModel;
using StripeDemo.Shared;
using System.Threading.Tasks;

namespace QNE.App.Pages.Account
{
    public partial class PlanSection
    {
        private string _selectedBillingAccount;
        [Parameter] public EventCallback<string> Callback { get; set; }
        [Parameter] public CompanyListViewModel Company { get; set; }
        [Parameter] public CompanyDto[] Companies { get; set; }
        [Parameter] public bool IsRegistration { get; set; } = false;

        [Inject] IHttpService Http { get; set; }

        [Inject] NavigationManager Nav { get; set; }

        private int CurrentManagePlanStep = 0;
        private string ManagePlanNextStepMessage = "Proceed to Add-ons";
        private string ManagePlanPreviousStepMessage = "";
        //private bool StarterPlanSelected = true;
        private AddOnPlan _plan { get; set; } = new AddOnPlan();

        private SubscriptionPlan[] _plans =
        {
            new SubscriptionPlan{Title="FREE", SubTitle="no hidden charges", NOU = 1, InvoiceCount = 120, BillCount = 72, BankReconCount = 250,
            ModuleValues1 = new []{1,2,4,512}
            },

            new SubscriptionPlan{Title="STARTER", SubTitle="available soon...", Active = true,
            ModuleValues1 = new []{1,2,4,512}
            },
            new SubscriptionPlan{Title="STANDARD", SubTitle="available soon...",
            ModuleValues1 = new []{1,2,4,128,256,512,4096}, ModuleValues2= new []{16384}},

            new SubscriptionPlan{Title="PROFESSIONAL", SubTitle="available soon...",
            ModuleValues1 = new []{1,2,4,8,128,256,512,4096}, ModuleValues2= new []{16384}, }
        };

        protected override async Task OnInitializedAsync()
        {
            _plan = new AddOnPlan();
            await base.OnInitializedAsync();
        }

        private void Recalculate()
        {
            _plan.SubTotal = _plan.BasePrice;

            if (_plan.CallInSupport)
                _plan.SubTotal += 2620.8m;

            if (_plan.AdditionalScan)
                _plan.SubTotal += 500m;

            _plan.Tax = _plan.SubTotal * 0.12m;
            StateHasChanged();
        }


        private void BackToParent(string args = "cancel")
        {
            CurrentManagePlanStep = 0;
            Callback.InvokeAsync(args);
        }

        private void ManagePlanUpgradeNextStep()
        {
            CurrentManagePlanStep++;
            if (CurrentManagePlanStep == 4 && IsRegistration) BackToParent("complete");

            SyncManagePlanUpgradeButtonMessages();
        }

        private void ManagePlanUpgradePreviousStep()
        {
            CurrentManagePlanStep--;
            SyncManagePlanUpgradeButtonMessages();
        }

        private void SyncManagePlanUpgradeButtonMessages()
        {
            switch (CurrentManagePlanStep)
            {
                case 0:
                    ManagePlanNextStepMessage = "Proceed to Add-ons";
                    ManagePlanPreviousStepMessage = "";
                    break;
                case 1:
                    ManagePlanNextStepMessage = "Continue to Billing Account";
                    ManagePlanPreviousStepMessage = "Back to Plan";
                    break;
                case 2:
                    ManagePlanNextStepMessage = "Continue to Review & Pay";
                    ManagePlanPreviousStepMessage = "Back to Add-ons";
                    break;
                case 3:
                    ManagePlanNextStepMessage = "Continue Purchase";
                    ManagePlanPreviousStepMessage = "Back to Billing Account";
                    break;
            }
        }

        private class AddOnPlan
        {
            public bool CallInSupport { get; set; } = false;
            public bool AdditionalScan { get; set; } = false;
            public decimal SubTotal { get; set; }
            public decimal Tax { get; set; }
            public decimal BasePrice { get; set; }

        }

        private async Task Checkout()
        {
            var resp = await Http.PostAsync<StripeCheckoutResultModel>("/api/stripe/checkout", new StripeCheckoutModel { PlanKey = "cloudaccounting-starter" });
            if (resp != null)
            {
                Nav.NavigateTo(resp.RedirectUrl);
            }
        }
    }
}

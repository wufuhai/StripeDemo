using QNE.Models.ViewModel;

namespace StripeDemo.Client.Pages
{
    public partial class Welcome
    {
        private CompanyListViewModel _selectedCompany = new CompanyListViewModel{CompanyName = "SAMPLE TRADING, CORP"};
        private bool UpgradeMode = true;
    }
}
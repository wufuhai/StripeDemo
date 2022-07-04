using Microsoft.AspNetCore.Components;
using QNE.Models.ViewModel;
using System.Threading.Tasks;

namespace StripeDemo.Client.Pages
{
    public partial class Cancel
    {
        [Parameter]
        public string SessionId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
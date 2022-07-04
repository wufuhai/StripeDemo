using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace StripeDemo.Client.Pages
{
    public partial class Success
    {
        [Parameter]
        public string SessionId { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }
    }
}
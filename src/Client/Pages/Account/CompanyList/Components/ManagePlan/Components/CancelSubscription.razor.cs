using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace QNE.App.Pages.Account
{
    public partial class CancelSubscription
    {
        private int _returnRating = 5;

        [CascadingParameter(Name = "Parent")]
        public ManagePlan Parent { get; set; }

        [CascadingParameter]
        public AntDesign.ProLayout.BasicLayout MainLayout { get; set; }

        [Parameter]
        public bool ShowCancelPopUp { get; set; }
    }
}
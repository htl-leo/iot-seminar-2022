﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Threading.Tasks;

namespace Wasm.Pages.Authentication
{
    public partial class RedirectToLogin
    {
        [Inject]
        private NavigationManager NavigationManager { get; set; }

        [CascadingParameter]
        private Task<AuthenticationState> AuthenticationState { get; set; }

        bool NotAuthorized { get; set; } = false;

        protected override async Task OnInitializedAsync()
        {
            // await Task.Delay(5000);
            var authState = await AuthenticationState;

            if (authState?.User?.Identity is null || !authState.User.Identity.IsAuthenticated)
            {
                var returnUrl = NavigationManager.ToBaseRelativePath(NavigationManager.Uri);
                if (string.IsNullOrEmpty(returnUrl))
                {
                    NavigationManager.NavigateTo("login", true);
                }
                else
                {
                    NavigationManager.NavigateTo($"login?returnUrl={returnUrl}", true);
                }
            }
            else
            {
                NotAuthorized = true;
            }


        }
    }
}

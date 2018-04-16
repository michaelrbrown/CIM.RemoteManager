﻿using CIM.RemoteManager.Core.ViewModels;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Pages
{
    public class BaseTabbedPage : TabbedPage
    {
        protected override void OnAppearing()
        {
            base.OnAppearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Resume();
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            var viewModel = BindingContext as BaseViewModel;
            viewModel?.Suspend();
        }
    }
}

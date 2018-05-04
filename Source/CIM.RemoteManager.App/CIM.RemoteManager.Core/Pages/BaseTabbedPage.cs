using CIM.RemoteManager.Core.ViewModels;
using SlideOverKit;
using System;
using Xamarin.Forms;

namespace CIM.RemoteManager.Core.Pages
{
    public class BaseTabbedPage : TabbedPage, IMenuContainerPage
    {
        public SlideMenuView SlideMenu { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action ShowMenuAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public Action HideMenuAction { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

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
